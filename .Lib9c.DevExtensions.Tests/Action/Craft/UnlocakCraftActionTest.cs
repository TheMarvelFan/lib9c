using Bencodex.Types;
using Lib9c.DevExtensions.Action.Craft;
using Lib9c.Tests;
using Lib9c.Tests.Action;
using Lib9c.Tests.Util;
using Libplanet.Action;
using Libplanet.Action.State;
using Libplanet.Crypto;
using Nekoyume;
using Nekoyume.Action;
using Nekoyume.Model;
using Nekoyume.Module;
using Xunit;

namespace Lib9c.DevExtensions.Tests.Action.Craft
{
    public class UnlockCraftActionTest
    {
        private readonly TableSheets _tableSheets;
        private readonly Address _agentAddress;
        private readonly Address _avatarAddress;
        private readonly IWorld _initialStateV2;

        public UnlockCraftActionTest()
        {
            (_tableSheets, _agentAddress, _avatarAddress, _, _initialStateV2) =
                InitializeUtil.InitializeStates(isDevEx: true);
        }

        [Theory]
        [InlineData("combination_equipment",
            GameConfig.RequireClearedStageLevel.CombinationEquipmentAction)]
        [InlineData("combination_equipment14",
            GameConfig.RequireClearedStageLevel.CombinationEquipmentAction)]
        [InlineData("combination_consumable",
            GameConfig.RequireClearedStageLevel.CombinationConsumableAction)]
        [InlineData("combination_consumable8",
            GameConfig.RequireClearedStageLevel.CombinationConsumableAction)]
        [InlineData("item_enhancement",
            GameConfig.RequireClearedStageLevel.ItemEnhancementAction)]
        [InlineData("item_enhancement11",
            GameConfig.RequireClearedStageLevel.ItemEnhancementAction)]
        public void StageUnlockTest(string typeIdentifier, int expectedStage)
        {
            var action = new UnlockCraftAction
            {
                AvatarAddress = _avatarAddress,
                ActionType = new ActionTypeAttribute(typeIdentifier)
            };

            var state = action.Execute(new ActionContext
            {
                PreviousState = _initialStateV2,
                Signer = _agentAddress,
                BlockIndex = 0L
            });

            var avatarState = state.GetAvatarState(_avatarAddress);
            var worldInformation = avatarState.worldInformation;
            Assert.True(worldInformation.IsStageCleared(expectedStage));
        }
    }
}
