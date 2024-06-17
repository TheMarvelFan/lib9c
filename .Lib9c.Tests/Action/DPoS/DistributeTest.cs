namespace Lib9c.Tests.Action.DPoS
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Libplanet.Action.State;
    using Libplanet.Crypto;
    using Libplanet.Types.Assets;
    using Libplanet.Types.Blocks;
    using Libplanet.Types.Consensus;
    using Nekoyume.Action.DPoS.Control;
    using Nekoyume.Action.DPoS.Misc;
    using Nekoyume.Action.DPoS.Model;
    using Nekoyume.Model.State;
    using Nekoyume.Module;
    using Xunit;
    using Validator = Nekoyume.Action.DPoS.Model.Validator;

    public class DistributeTest : PoSTest
    {
        private readonly ImmutableHashSet<Currency> _nativeTokens;
        private IWorld _states;

        public DistributeTest()
            : base()
        {
            _states = InitialState;
            _nativeTokens = NativeTokens;
            OperatorPrivateKeys = new List<PrivateKey>();
            OperatorPublicKeys = new List<PublicKey>();
            OperatorAddresses = new List<Address>();
            ValidatorAddresses = new List<Address>();
            DelegatorAddress = CreateAddress();
            _states = _states.TransferAsset(
                new ActionContext(),
                GoldCurrencyState.Address,
                DelegatorAddress,
                GovernanceToken * 100000);
            for (int i = 0; i < 200; i++)
            {
                PrivateKey operatorPrivateKey = new PrivateKey();
                PublicKey operatorPublicKey = operatorPrivateKey.PublicKey;
                Address operatorAddress = operatorPublicKey.Address;
                _states = _states.TransferAsset(
                    new ActionContext(),
                    GoldCurrencyState.Address,
                    operatorAddress,
                    GovernanceToken * 1000);

                OperatorPrivateKeys.Add(operatorPrivateKey);
                OperatorPublicKeys.Add(operatorPublicKey);
                OperatorAddresses.Add(operatorAddress);
                _states = ValidatorCtrl.Create(
                    _states,
                    new ActionContext
                    {
                        PreviousState = _states,
                        BlockIndex = 1,
                    },
                    operatorAddress,
                    operatorPublicKey,
                    GovernanceToken * 1,
                    _nativeTokens);
                ValidatorAddresses.Add(Validator.DeriveAddress(operatorAddress));
            }
        }

        private List<PrivateKey> OperatorPrivateKeys { get; set; }

        private List<PublicKey> OperatorPublicKeys { get; set; }

        private List<Address> OperatorAddresses { get; set; }

        private List<Address> ValidatorAddresses { get; set; }

        private Address DelegatorAddress { get; set; }

        [Fact]
        public void ValidatorSetTest()
        {
            for (int i = 0; i < 200; i++)
            {
                _states = DelegateCtrl.Execute(
                    _states,
                    new ActionContext
                    {
                        PreviousState = _states,
                        BlockIndex = 1,
                    },
                    DelegatorAddress,
                    ValidatorAddresses[i],
                    GovernanceToken * (i + 1),
                    _nativeTokens);
            }

            Address validatorAddressA = ValidatorAddresses[3];
            Address validatorAddressB = ValidatorAddresses[5];

            _states = DelegateCtrl.Execute(
                _states,
                new ActionContext
                {
                    PreviousState = _states,
                    BlockIndex = 1,
                },
                DelegatorAddress,
                validatorAddressA,
                GovernanceToken * 200,
                _nativeTokens);

            _states = DelegateCtrl.Execute(
                _states,
                new ActionContext
                {
                    PreviousState = _states,
                    BlockIndex = 1,
                },
                DelegatorAddress,
                validatorAddressB,
                GovernanceToken * 300,
                _nativeTokens);

            _states = ValidatorSetCtrl.Update(
                _states,
                new ActionContext
                {
                    PreviousState = _states,
                    BlockIndex = 1,
                });

            Nekoyume.Action.DPoS.Model.ValidatorSet validatorSet;
            (_states, validatorSet) = ValidatorSetCtrl.FetchBondedValidatorSet(_states);
            var blockHash = BlockHash.FromString(
                "0000000000000000000000000000000000000000000000000000000000000001");

            List<Vote> votes = validatorSet.Set.Select(
                    validator =>
                    {
                        bool joined =
                            validator.OperatorPublicKey.Equals(OperatorPrivateKeys[3].PublicKey) ||
                            validator.OperatorPublicKey.Equals(OperatorPrivateKeys[5].PublicKey);
                        return new VoteMetadata(
                            default,
                            default,
                            blockHash,
                            default,
                            validator.OperatorPublicKey,
                            validator.ConsensusToken.RawValue,
                            joined ? VoteFlag.PreCommit : VoteFlag.Null).Sign(
                            joined ? OperatorPrivateKeys.First(
                                pk => pk.PublicKey.Equals(validator.OperatorPublicKey)) : null);
                    })
                .ToList();
            FungibleAssetValue blockReward = Asset.ConsensusFromGovernance(GovernanceToken * 50);
            _states = _states.MintAsset(
                new ActionContext
                {
                    PreviousState = _states,
                    BlockIndex = 1,
                },
                ReservedAddress.RewardPool,
                blockReward);
            _states = AllocateRewardCtrl.Execute(
                _states,
                new ActionContext
                {
                    PreviousState = _states,
                    BlockIndex = 1,
                },
                _nativeTokens,
                votes,
                new ProposerInfo(0, OperatorAddresses[3]));

            var (baseProposerReward, _)
                = (blockReward * AllocateRewardCtrl.BaseProposerRewardNumerator)
                .DivRem(AllocateRewardCtrl.BaseProposerRewardDenominator);
            var (bonusProposerReward, _)
                = (blockReward * (205 + 307)
                * AllocateRewardCtrl.BonusProposerRewardNumerator)
                .DivRem((100 + (101 + 200) * 50 - 101 - 102 + 204 + 306)
                * AllocateRewardCtrl.BonusProposerRewardDenominator);
            FungibleAssetValue proposerReward = baseProposerReward + bonusProposerReward;
            FungibleAssetValue validatorRewardSum = blockReward - proposerReward;

            var (validatorRewardA, _)
                    = (validatorRewardSum * 205)
                    .DivRem(100 + (101 + 200) * 50 - 101 - 102 + 204 + 306);
            var (commissionA, _)
                    = (validatorRewardA * Validator.CommissionNumerator)
                    .DivRem(Validator.CommissionDenominator);
            var (validatorRewardB, _)
                    = (validatorRewardSum * 307)
                    .DivRem(100 + (101 + 200) * 50 - 101 - 102 + 204 + 306);
            var (commissionB, _)
                    = (validatorRewardB * Validator.CommissionNumerator)
                    .DivRem(Validator.CommissionDenominator);

            Assert.Equal(
                Asset.ConsensusFromGovernance(GovernanceToken * 0),
                _states.GetBalance(ReservedAddress.RewardPool, Asset.ConsensusToken));

            Assert.Equal(
                GovernanceToken * (100 + (101 + 200) * 50 - 101 - 102 + 204 + 306),
                _states.GetBalance(ReservedAddress.BondedPool, GovernanceToken));

            Assert.Equal(
                Asset.ConsensusFromGovernance(GovernanceToken * 205),
                _states.GetBalance(validatorAddressA, Asset.ConsensusToken));

            Assert.Equal(
                Asset.ConsensusFromGovernance(GovernanceToken * 307),
                _states.GetBalance(validatorAddressB, Asset.ConsensusToken));

            Assert.Equal(
                proposerReward + commissionA,
                _states.GetBalance(
                    AllocateRewardCtrl.RewardAddress(OperatorAddresses[3]), Asset.ConsensusToken));

            Assert.Equal(
                commissionB,
                _states.GetBalance(
                    AllocateRewardCtrl.RewardAddress(OperatorAddresses[5]), Asset.ConsensusToken));

            Address delegationAddressA
                = Delegation.DeriveAddress(DelegatorAddress, validatorAddressA);

            Assert.Equal(
                Asset.ConsensusFromGovernance(GovernanceToken * 0),
                _states.GetBalance(
                    AllocateRewardCtrl.RewardAddress(DelegatorAddress), Asset.ConsensusToken));

            var (delegatorToken, _)
                = (_states.GetBalance(
                    ValidatorRewards.DeriveAddress(validatorAddressA, Asset.ConsensusToken),
                    Asset.ConsensusToken)
                * _states.GetBalance(
                    Delegation.DeriveAddress(DelegatorAddress, validatorAddressA),
                    Asset.Share)
                .RawValue)
                .DivRem(ValidatorCtrl.GetValidator(_states, validatorAddressA)!
                .DelegatorShares.RawValue);

            _states = DelegateCtrl.Distribute(
                _states,
                new ActionContext
                {
                    PreviousState = _states,
                    BlockIndex = 5,
                },
                _nativeTokens,
                delegationAddressA);

            Assert.Equal(
                delegatorToken,
                _states.GetBalance(
                    AllocateRewardCtrl.RewardAddress(DelegatorAddress), Asset.ConsensusToken));
        }
    }
}
