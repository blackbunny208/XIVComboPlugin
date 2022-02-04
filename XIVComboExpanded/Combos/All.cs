using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedestPlugin.Combos
{
    internal static class All
    {
        public const byte JobID = 0;

        public const uint
            Swiftcast = 7561,
            Resurrection = 173,
            Verraise = 7523,
            Raise = 125,
            Ascend = 3603,
            Egeiro = 24287,
            SolidReason = 232,
            AgelessWords = 215,
            WiseToTheWorldMIN = 26521,
            WiseToTheWorldBTN = 26522,
            LowBlow = 7540,
            Interject = 7538,
            LucidDreaming = 7562;

        public static class Buffs
        {
            public const ushort
                Swiftcast = 167,
                EurekaMoment = 2765;
        }

        public static class Levels
        {
            public const byte
                Raise = 12;
        }
    }

    internal class AllSwiftcastFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.AllSwiftcastFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == All.Raise || actionID == All.Resurrection || actionID == All.Ascend || actionID == All.Verraise || actionID == All.Egeiro)
            {
                if ((IsActionOffCooldown(All.Swiftcast) && !HasEffect(RDM.Buffs.Dualcast))
                    || level <= All.Levels.Raise
                    || (level <= RDM.Levels.Verraise && actionID == All.Verraise))
                    return All.Swiftcast;
            }

            return actionID;
        }
    }

    internal class AllEurekaFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.AllEurekaFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == All.SolidReason || actionID == All.AgelessWords)
            {
                if (HasEffect(All.Buffs.EurekaMoment))
                {
                    if (actionID == All.SolidReason)
                        return All.WiseToTheWorldMIN;
                    return All.WiseToTheWorldBTN;
                }
            }

            return actionID;
        }
    }

    internal class AllTankInterruptFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.AllTankInterruptFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == All.LowBlow)
            {
                if (CanInterruptEnemy() && IsActionOffCooldown(All.Interject) && CanUseAction(All.Interject))
                    return All.Interject;
            }

            return actionID;
        }
    }
}