using System;
using System.Linq;

using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedestPlugin.Combos
{
    internal static class BRD
    {
        public const byte ClassID = 5;
        public const byte JobID = 23;

        public const uint
            HeavyShot = 97,
            StraightShot = 98,
            VenomousBite = 100,
            QuickNock = 106,
            Barrage = 107,
            Windbite = 113,
            Peloton = 7557,
            MagesBallad = 114,
            ArmysPaeon = 116,
            BattleVoice = 118,
            WanderersMinuet = 3559,
            IronJaws = 3560,
            Sidewinder = 3562,
            EmpyrealArrow = 3558,
            PitchPerfect = 7404,
            CausticBite = 7406,
            Stormbite = 7407,
            RefulgentArrow = 7409,
            Shadowbite = 16494,
            BurstShot = 16495,
            ApexArrow = 16496,
            Ladonsbite = 25783,
            Bloodletter = 110,
            RainOfDeath = 117,
            EmpyrealArrow = 3558,
            Sidewinder = 3562,
            RadiantFinale = 25785;

        public static class Buffs
        {
            public const ushort
                StraightShotReady = 122,
                ShadowbiteReady = 3002,
                WanderersMinuet = 2216;
        }

        public static class Debuffs
        {
            public const ushort
                VenomousBite = 124,
                Windbite = 129,
                CausticBite = 1200,
                Stormbite = 1201;
        }

        public static class Levels
        {
            public const byte
                Windbite = 30,
                EmpyrealArrow = 54,
                IronJaws = 56,
                Sidewinder = 60,
                BiteUpgrade = 64,
                RefulgentArrow = 70,
                BurstShot = 76,
                WanderersMinuet = 52,
                MagesBallad = 30,
                ArmysPaeon = 40,
                RadiantFinale = 90;
        }
    }

    internal class BardWanderersPitchPerfectFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BardWanderersPitchPerfectFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.WanderersMinuet)
            {
                var gauge = GetJobGauge<BRDGauge>();
                if (gauge.Song == Song.WANDERER)
                    return BRD.PitchPerfect;
            }

            return actionID;
        }
    }

    internal class BardStraightShotUpgradeFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BardStraightShotUpgradeFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.HeavyShot || actionID == BRD.BurstShot)
            {
                var globalCD = GetCooldown(BRD.HeavyShot);
                var gauge = GetJobGauge<BRDGauge>();

                if (globalCD.CooldownRemaining > 0.7 && IsEnabled(CustomComboPreset.BardOGCDFeature))
                {
                    var pitchCD = GetCooldown(BRD.PitchPerfect);
                    var bloodCD = GetCooldown(BRD.Bloodletter);
                    var empCD = GetCooldown(BRD.EmpyrealArrow);
                    var swCD = GetCooldown(BRD.Sidewinder);

                    if (!pitchCD.IsCooldown && gauge.Repertoire == 3 && gauge.Song == Song.WANDERER)
                        return BRD.PitchPerfect;

                    if (!bloodCD.IsCooldown)
                        return BRD.Bloodletter;

                    if (!empCD.IsCooldown && level >= BRD.Levels.EmpyrealArrow)
                        return BRD.EmpyrealArrow;

                    if (!swCD.IsCooldown && level >= BRD.Levels.Sidewinder)
                        return BRD.Sidewinder;

                    return BRD.Bloodletter;
                }

                // if (IsEnabled(CustomComboPreset.BardApexFeature) && (gauge.SoulVoice == 100 || OriginalHook(BRD.ApexArrow) != BRD.ApexArrow))
                //    return OriginalHook(BRD.ApexArrow);

                if (HasEffect(BRD.Buffs.StraightShotReady))
                    return OriginalHook(BRD.StraightShot);
            }

            return actionID;
        }
    }

    internal class BardIronJawsFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BardIronJawsFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.IronJaws)
            {
                if (level < BRD.Levels.IronJaws)
                {
                    var venomous = FindTargetEffect(BRD.Debuffs.VenomousBite);
                    var windbite = FindTargetEffect(BRD.Debuffs.Windbite);
                    if (venomous is not null && windbite is not null)
                    {
                        if (venomous?.RemainingTime < windbite?.RemainingTime)
                            return BRD.VenomousBite;
                        return BRD.Windbite;
                    }
                    else if (windbite is not null || level < BRD.Levels.Windbite)
                    {
                        return BRD.VenomousBite;
                    }

                    return BRD.Windbite;
                }

                if (level < BRD.Levels.BiteUpgrade)
                {
                    var venomous = TargetHasEffect(BRD.Debuffs.VenomousBite);
                    var windbite = TargetHasEffect(BRD.Debuffs.Windbite);

                    if (venomous && windbite)
                        return BRD.IronJaws;

                    if (windbite)
                        return BRD.VenomousBite;

                    return BRD.Windbite;
                }

                var caustic = TargetHasEffect(BRD.Debuffs.CausticBite);
                var stormbite = TargetHasEffect(BRD.Debuffs.Stormbite);

                if (caustic && stormbite)
                    return BRD.IronJaws;

                if (stormbite)
                    return BRD.CausticBite;

                return BRD.Stormbite;
            }

            return actionID;
        }
    }

    /*
    internal class BardApexFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BardApexFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.QuickNock || actionID == BRD.Ladonsbite)
            {
                var gauge = GetJobGauge<BRDGauge>();
                if (gauge.SoulVoice >= 80 || OriginalHook(BRD.ApexArrow) != BRD.ApexArrow)
                    return OriginalHook(BRD.ApexArrow);
            }

            return actionID;
        }
    }
    */

    internal class BardShadowbiteFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BardShadowbiteFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.QuickNock || actionID == BRD.Ladonsbite)
            {
                var globalCD = GetCooldown(BRD.QuickNock);
                var gauge = GetJobGauge<BRDGauge>();

                if (globalCD.CooldownRemaining > 0.7 && IsEnabled(CustomComboPreset.BardOGCDFeature))
                {
                    var pitchCD = GetCooldown(BRD.PitchPerfect);
                    var rainCD = GetCooldown(BRD.RainOfDeath);
                    var empCD = GetCooldown(BRD.EmpyrealArrow);
                    var swCD = GetCooldown(BRD.Sidewinder);

                    if (!pitchCD.IsCooldown && gauge.Repertoire == 3 && gauge.Song == Song.WANDERER)
                        return BRD.PitchPerfect;

                    if (!rainCD.IsCooldown)
                        return BRD.RainOfDeath;

                    if (!empCD.IsCooldown && level >= BRD.Levels.EmpyrealArrow)
                        return BRD.EmpyrealArrow;

                    if (!swCD.IsCooldown && level >= BRD.Levels.Sidewinder)
                        return BRD.Sidewinder;

                    return BRD.RainOfDeath;
                }

                if ((gauge.SoulVoice == 100 && IsEnabled(CustomComboPreset.BardApexFeature)) || OriginalHook(BRD.ApexArrow) != BRD.ApexArrow)
                    return OriginalHook(BRD.ApexArrow);

                if (HasEffect(BRD.Buffs.ShadowbiteReady))
                    return OriginalHook(BRD.Shadowbite);
            }

            return actionID;
        }
    }

    internal class BardSongFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BardSongFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.MagesBallad)
            {
                var wmCD = GetCooldown(BRD.WanderersMinuet);
                var mbCD = GetCooldown(BRD.MagesBallad);
                var apCD = GetCooldown(BRD.ArmysPaeon);

                // return sprint if they don't have any songs unlocked.
                if (level < BRD.Levels.MagesBallad)
                    return BRD.Peloton;

                // return whichever is highest priority and off CD
                if (!wmCD.IsCooldown && level >= BRD.Levels.WanderersMinuet)
                    return BRD.WanderersMinuet;
                if (!mbCD.IsCooldown && level >= BRD.Levels.MagesBallad)
                    return BRD.MagesBallad;
                if (!apCD.IsCooldown && level >= BRD.Levels.ArmysPaeon)
                    return BRD.ArmysPaeon;

                // if all three are on CD, return whichever is shortest CD for visibility
                if (wmCD.CooldownRemaining <= mbCD.CooldownRemaining && wmCD.CooldownRemaining <= apCD.CooldownRemaining && level >= BRD.Levels.WanderersMinuet)
                    return BRD.WanderersMinuet;
                if (apCD.CooldownRemaining <= mbCD.CooldownRemaining && level >= BRD.Levels.ArmysPaeon)
                    return BRD.ArmysPaeon;
                return BRD.MagesBallad;
            }

            return actionID;
        }
    }
    
    internal class BardSidewinderFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BardSidewinderFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            return (IsActionOffCooldown(BRD.Sidewinder) && !IsActionOffCooldown(BRD.EmpyrealArrow) && level >= BRD.Levels.Sidewinder) ? BRD.Sidewinder : BRD.EmpyrealArrow;
        }
    }

    internal class BardRadiantFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BardRadiantFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var gauge = GetJobGauge<BRDGauge>();
            return (!IsActionOffCooldown(BRD.BattleVoice) && level >= BRD.Levels.RadiantFinale) ? BRD.RadiantFinale : BRD.BattleVoice;
        }
    }

    internal class BardBarrageFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BardBarrageFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            return HasEffect(BRD.Buffs.StraightShotReady) ? OriginalHook(BRD.StraightShot) : BRD.Barrage;
        }
    }
}
