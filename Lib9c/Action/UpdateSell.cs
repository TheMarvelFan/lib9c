using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Bencodex.Types;
using Lib9c.Abstractions;
using Lib9c.Model.Order;
using Libplanet.Action;
using Libplanet.Action.State;
using Libplanet.Crypto;
using Nekoyume.Action.Extensions;
using Nekoyume.Action.Statics;
using Nekoyume.Battle;
using Nekoyume.Model;
using Nekoyume.Model.Exceptions;
using Nekoyume.Model.Mail;
using Nekoyume.Model.State;
using Nekoyume.Module;
using Nekoyume.TableData;
using Serilog;
using static Lib9c.SerializeKeys;
using BxDictionary = Bencodex.Types.Dictionary;
using BxList = Bencodex.Types.List;

namespace Nekoyume.Action
{
    /// <summary>
    /// Hard forked at https://github.com/planetarium/lib9c/pull/1640
    /// </summary>
    [Serializable]
    [ActionType("update_sell5")]
    [ActionObsolete(ActionObsoleteConfig.V200030ObsoleteIndex)]
    public class UpdateSell : GameAction, IUpdateSellV2
    {
        private const int UpdateCapacity = 100;
        public Address sellerAvatarAddress;
        public IEnumerable<UpdateSellInfo> updateSellInfos;

        Address IUpdateSellV2.SellerAvatarAddress => sellerAvatarAddress;
        IEnumerable<IValue> IUpdateSellV2.UpdateSellInfos =>
            updateSellInfos.Select(x => x.Serialize());

        protected override IImmutableDictionary<string, IValue> PlainValueInternal =>
            new Dictionary<string, IValue>
            {
                [SellerAvatarAddressKey] = sellerAvatarAddress.Serialize(),
                [UpdateSellInfoKey] = updateSellInfos.Select(info => info.Serialize()).Serialize(),
            }.ToImmutableDictionary();

        protected override void LoadPlainValueInternal(
            IImmutableDictionary<string, IValue> plainValue)
        {
            sellerAvatarAddress = plainValue[SellerAvatarAddressKey].ToAddress();
            updateSellInfos = plainValue[UpdateSellInfoKey]
                .ToEnumerable(info => new UpdateSellInfo((List)info));
        }

        public override IWorld Execute(IActionContext context)
        {
            context.UseGas(1);
            if (context.Rehearsal)
            {
                return context.PreviousState;
            }

            var world = context.PreviousState;
            var inventoryAddress = sellerAvatarAddress.Derive(LegacyInventoryKey);
            var worldInformationAddress = sellerAvatarAddress.Derive(LegacyWorldInformationKey);
            var questListAddress = sellerAvatarAddress.Derive(LegacyQuestListKey);
            var digestListAddress = OrderDigestListState.DeriveAddress(sellerAvatarAddress);

            CheckObsolete(ActionObsoleteConfig.V200030ObsoleteIndex, context);
            if (!(LegacyModule.GetState(world, Addresses.Market) is null))
            {
                throw new ActionObsoletedException(
                    "UpdateSell action is obsoleted. please use ReRegisterProduct.");
            }


            if (updateSellInfos.Count() > UpdateCapacity)
            {
                throw new ArgumentOutOfRangeException(
                    $"{nameof(updateSellInfos)} must be less than or equal 100.");
            }

            // common
            var addressesHex = GetSignerAndOtherAddressesHex(context, sellerAvatarAddress);
            var sw = new Stopwatch();
            sw.Start();
            var started = DateTimeOffset.UtcNow;
            Log.Debug("{AddressesHex} updateSell exec started", addressesHex);

            if (!updateSellInfos.Any())
            {
                throw new ListEmptyException($"{addressesHex} List - UpdateSell infos was empty.");
            }

            if (!AvatarModule.TryGetAvatarState(
                    world,
                    context.Signer,
                    sellerAvatarAddress,
                    out var avatarState))
            {
                throw new FailedLoadStateException(
                    $"{addressesHex} Aborted as the avatar state of the signer was failed to load.");
            }

            sw.Stop();
            Log.Verbose(
                "{AddressesHex} Sell Get AgentAvatarStates: {Elapsed}",
                addressesHex,
                sw.Elapsed);
            sw.Restart();
            if (!avatarState.worldInformation.IsStageCleared(
                    GameConfig.RequireClearedStageLevel.ActionsInShop))
            {
                avatarState.worldInformation.TryGetLastClearedStageId(out var current);
                throw new NotEnoughClearedStageLevelException(
                    addressesHex,
                    GameConfig.RequireClearedStageLevel.ActionsInShop,
                    current);
            }

            sw.Stop();
            Log.Verbose(
                "{AddressesHex} UpdateSell IsStageCleared: {Elapsed}",
                addressesHex,
                sw.Elapsed);

            avatarState.updatedAt = context.BlockIndex;
            avatarState.blockIndex = context.BlockIndex;

            var costumeStatSheet = LegacyModule.GetSheet<CostumeStatSheet>(world);

            if (!LegacyModule.TryGetState(world, digestListAddress, out Dictionary rawList))
            {
                throw new FailedLoadStateException(
                    $"{addressesHex} failed to load {nameof(OrderDigest)}({digestListAddress}).");
            }

            var digestList = new OrderDigestListState(rawList);

            foreach (var updateSellInfo in updateSellInfos)
            {
                var updateSellShopAddress = ShardedShopStateV2.DeriveAddress(
                    updateSellInfo.itemSubType,
                    updateSellInfo.updateSellOrderId);
                var updateSellOrderAddress = Order.DeriveAddress(updateSellInfo.updateSellOrderId);
                var itemAddress = Addresses.GetItemAddress(updateSellInfo.tradableId);
                world = Sell.Cancel(
                    world,
                    updateSellInfo,
                    addressesHex,
                    avatarState,
                    digestList,
                    context,
                    sellerAvatarAddress);

                // for updateSell
                var updateSellShopState =
                    LegacyModule.TryGetState(
                        world,
                        updateSellShopAddress,
                        out Dictionary serializedState)
                        ? new ShardedShopStateV2(serializedState)
                        : new ShardedShopStateV2(updateSellShopAddress);

                Log.Verbose(
                    "{AddressesHex} UpdateSell Get ShardedShopState: {Elapsed}",
                    addressesHex,
                    sw.Elapsed);
                sw.Restart();
                var newOrder = OrderFactory.Create(
                    context.Signer,
                    sellerAvatarAddress,
                    updateSellInfo.updateSellOrderId,
                    updateSellInfo.price,
                    updateSellInfo.tradableId,
                    context.BlockIndex,
                    updateSellInfo.itemSubType,
                    updateSellInfo.count
                );

                newOrder.Validate(avatarState, updateSellInfo.count);

                var tradableItem = newOrder.Sell(avatarState);
                var orderDigest = newOrder.Digest(avatarState, costumeStatSheet);
                updateSellShopState.Add(orderDigest, context.BlockIndex);

                digestList.Add(orderDigest);

                world = LegacyModule.SetState(world, itemAddress, tradableItem.Serialize());
                world = LegacyModule.SetState(world, updateSellOrderAddress, newOrder.Serialize());
                world = LegacyModule.SetState(
                    world,
                    updateSellShopAddress,
                    updateSellShopState.Serialize());
                sw.Stop();
                Log.Verbose(
                    "{AddressesHex} UpdateSell Set ShopState: {Elapsed}",
                    addressesHex,
                    sw.Elapsed);
            }

            sw.Restart();
            world = AvatarModule.SetAvatarState(
                world,
                sellerAvatarAddress,
                avatarState,
                true,
                true,
                true,
                true);
            world = LegacyModule.SetState(world, digestListAddress, digestList.Serialize());
            sw.Stop();
            Log.Verbose(
                "{AddressesHex} UpdateSell Set AvatarState: {Elapsed}",
                addressesHex,
                sw.Elapsed);

            var ended = DateTimeOffset.UtcNow;
            Log.Debug(
                "{AddressesHex} UpdateSell Total Executed Time: {Elapsed}",
                addressesHex,
                ended - started);

            return world;
        }

        public static IWorld Cancel(IWorld world,
            UpdateSellInfo updateSellInfo, string addressesHex, AvatarState avatarState,
            OrderDigestListState digestList, IActionContext context, Address sellerAvatarAddress)
        {
            if (updateSellInfo.price.Sign < 0)
            {
                throw new InvalidPriceException($"{addressesHex} Aborted as the price is less than zero: {updateSellInfo.price}.");
            }

            var sw = new Stopwatch();
            var orderId = updateSellInfo.orderId;
            var tradableId = updateSellInfo.tradableId;
            var shopAddress = ShardedShopStateV2.DeriveAddress(updateSellInfo.itemSubType, orderId);

            // migration method
            avatarState.inventory.UnlockInvalidSlot(digestList, context.Signer, sellerAvatarAddress);
            avatarState.inventory.ReconfigureFungibleItem(digestList, tradableId);
            avatarState.inventory.LockByReferringToDigestList(digestList, tradableId, context.BlockIndex);

            // for sell cancel
            sw.Start();
            if (!LegacyModule.TryGetState(world, shopAddress, out BxDictionary shopStateDict))
            {
                throw new FailedLoadStateException($"{addressesHex}failed to load {nameof(ShardedShopStateV2)}({shopAddress}).");
            }

            sw.Stop();
            Log.Verbose("{AddressesHex} UpdateSell Sell Cancel Get ShopState: {Elapsed}", addressesHex, sw.Elapsed);
            sw.Restart();
            if (!LegacyModule.TryGetState(world, Order.DeriveAddress(orderId), out Dictionary orderDict))
            {
                throw new FailedLoadStateException($"{addressesHex} failed to load {nameof(Order)}({Order.DeriveAddress(updateSellInfo.orderId)}).");
            }

            var orderOnSale = OrderFactory.Deserialize(orderDict);
            orderOnSale.ValidateCancelOrder(avatarState, tradableId);
            orderOnSale.Cancel(avatarState, context.BlockIndex);
            if (context.BlockIndex < orderOnSale.ExpiredBlockIndex)
            {
                var shardedShopState = new ShardedShopStateV2(shopStateDict);
                shardedShopState.Remove(orderOnSale, context.BlockIndex);
                world = LegacyModule.SetState(world, shopAddress, shardedShopState.Serialize());
            }

            digestList.Remove(orderOnSale.OrderId);
            sw.Stop();

            var expirationMail = avatarState.mailBox.OfType<OrderExpirationMail>()
                .FirstOrDefault(m => m.OrderId.Equals(orderId));
            if (!(expirationMail is null))
            {
                avatarState.mailBox.Remove(expirationMail);
            }

            return LegacyModule.SetState(world, digestList.Address, digestList.Serialize());
        }
    }
}
