namespace Lib9c.Tests.Util
{
    using System.Collections.Generic;
    using System.Globalization;
    using Libplanet;
    using Libplanet.Action;
    using Nekoyume;
    using Nekoyume.Action;
    using Nekoyume.Model.Item;
    using Nekoyume.Model.State;
    using Nekoyume.TableData;
    using static Lib9c.SerializeKeys;

    public static class CraftUtil
    {
        public static IAccountStateDelta PrepareCombinationSlot(
            IAccountStateDelta state,
            Address avatarAddress,
            int slotIndex
        )
        {
            var slotAddress = avatarAddress.Derive(string.Format(
                CultureInfo.InvariantCulture,
                CombinationSlotState.DeriveFormat,
                slotIndex));
            var slotState = new CombinationSlotState(
                slotAddress,
                GameConfig.RequireClearedStageLevel.CombinationEquipmentAction
            );
            return state.SetState(slotAddress, slotState.Serialize());
        }

        public static IAccountStateDelta AddMaterialsToInventory(
            IAccountStateDelta state,
            TableSheets tableSheets,
            Address avatarAddress,
            IEnumerable<EquipmentItemSubRecipeSheet.MaterialInfo> materialList,
            IRandom random
        )
        {
            var avatarState = state.GetAvatarStateV2(avatarAddress);
            foreach (var material in materialList)
            {
                var materialRow = tableSheets.MaterialItemSheet[material.Id];
                var materialItem = ItemFactory.CreateItem(materialRow, random);
                avatarState.inventory.AddItem(materialItem, material.Count);
            }

            return state.SetState(
                avatarAddress.Derive(LegacyInventoryKey),
                avatarState.inventory.Serialize()
            );
        }
    }
}
