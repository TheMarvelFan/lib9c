namespace Lib9c.Tests.Model.Skill.Adventure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Bencodex.Types;
    using Lib9c.Tests.Action;
    using Libplanet.Crypto;
    using Nekoyume.Battle;
    using Nekoyume.Model;
    using Nekoyume.Model.Buff;
    using Nekoyume.Model.Skill;
    using Nekoyume.Model.Stat;
    using Nekoyume.Model.State;
    using Xunit;

    public class ShatterStrikeTest
    {
        private readonly TableSheets _tableSheets = new (TableSheetsImporter.ImportSheets());

        [Theory]
        // 1bp == 0.01%
        [InlineData(100, 10000, false, true)]
        [InlineData(100, 10000, false, false)]
        [InlineData(100, 1000, false, true)]
        [InlineData(100, 1000, false, false)]
        [InlineData(100, 3700, false, true)]
        [InlineData(100, 3700, false, false)]
        [InlineData(100, 100_000, true, true)]
        [InlineData(100, 100_000, true, false)]
        [InlineData(1_000_000, 100_000, false, true)]
        [InlineData(1_000_000, 100_000, false, false)]
        public void Use(int enemyHp, int ratioBp, bool expectedEnemyDead, bool copyCharacter)
        {
            var gameConfigState =
                new GameConfigState((Text)_tableSheets.GameConfigSheet.Serialize());
            Assert.True(
                _tableSheets.SkillSheet.TryGetValue(700011, out var skillRow)
            ); // 700011 is ShatterStrike
            var shatterStrike = new ShatterStrike(skillRow, 0, 0, ratioBp, StatType.NONE);

            var avatarState = new AvatarState(
                new PrivateKey().Address,
                new PrivateKey().Address,
                0,
                _tableSheets.GetAvatarSheets(),
                new GameConfigState(),
                new PrivateKey().Address
            );
            var worldRow = _tableSheets.WorldSheet.First;
            Assert.NotNull(worldRow);

            var random = new TestRandom();
            var simulator = new StageSimulator(
                random,
                avatarState,
                new List<Guid>(),
                null,
                new List<Nekoyume.Model.Skill.Skill>(),
                1,
                1,
                _tableSheets.StageSheet[1],
                _tableSheets.StageWaveSheet[1],
                false,
                20,
                _tableSheets.GetSimulatorSheets(),
                _tableSheets.EnemySkillSheet,
                _tableSheets.CostumeStatSheet,
                StageSimulator.GetWaveRewards(
                    random,
                    _tableSheets.StageSheet[1],
                    _tableSheets.MaterialItemSheet),
                new List<StatModifier>(),
                _tableSheets.DeBuffLimitSheet,
                copyCharacter,
                shatterStrikeMaxDamage: gameConfigState.ShatterStrikeMaxDamage
            );
            var player = new Player(avatarState, simulator);
            var enemyRow = _tableSheets.CharacterSheet.OrderedList
                .FirstOrDefault(e => e.Id > 200000);
            enemyRow.Set(new[]
            {
                "201000", "XS", "2", enemyHp.ToString(), "16", "6", "4", "90", "15", "3.2", "0.64",
                "0.24", "0", "3.6", "0.6", "0.8", "1.2",
            });
            Assert.NotNull(enemyRow);

            var enemy = new Enemy(player, enemyRow, 1);

            player.Targets.Add(enemy);
            var used = shatterStrike.Use(player, 0, new List<Buff>(), copyCharacter);
            Assert.NotNull(used);
            var skillInfo = Assert.Single(used.SkillInfos);
            Assert.Equal(
                Math.Clamp(
                    enemy.HP * ratioBp / 10000m - enemy.DEF + player.ArmorPenetration,
                    1,
                    gameConfigState.ShatterStrikeMaxDamage
                ),
                skillInfo.Effect
            );
            Assert.Equal(expectedEnemyDead, skillInfo.IsDead);
        }
    }
}
