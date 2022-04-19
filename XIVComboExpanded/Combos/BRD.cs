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
            RagingStrikes = 101,
            QuickNock = 106,
            Barrage = 107,
            RainOfDeath = 117,
            Bloodletter = 110,
            Windbite = 113,
            Peloton = 7557,
            MagesBallad = 114,
            ArmysPaeon = 116,
            BattleVoice = 118,
            WanderersMinuet = 3559,
            Peloton = 7557,
            IronJaws = 3560,
            PitchPerfect = 7404,
            CausticBite = 7406,
            Stormbite = 7407,
            RefulgentArrow = 7409,
            Shadowbite = 16494,
            BurstShot = 16495,
            ApexArrow = 16496,
            Ladonsbite = 25783,
            EmpyrealArrow = 3558,
            Sidewinder = 3562,
            BlastArrow = 25784,
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
                RainOfDeath = 45,
                BattleVoice = 50,
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

                    if (gauge.Song == Song.WANDERER && !pitchCD.IsCooldown && (gauge.Repertoire == 3 || (gauge.Repertoire >= 1 && gauge.SongTimer <= 3000)))
                        return BRD.PitchPerfect;

                    if (!bloodCD.IsCooldown)
                        return BRD.Bloodletter;

                    if (!empCD.IsCooldown && CanUseAction(BRD.EmpyrealArrow))
                        return BRD.EmpyrealArrow;

                    if (!swCD.IsCooldown && CanUseAction(BRD.Sidewinder))
                        return BRD.Sidewinder;

                    if (bloodCD.CooldownRemaining <= 30) return BRD.Bloodletter;
                }

                // if (IsEnabled(CustomComboPreset.BardApexFeature) && (gauge.SoulVoice == 100 || OriginalHook(BRD.ApexArrow) != BRD.ApexArrow))
                //    return OriginalHook(BRD.ApexArrow);

                if (HasEffect(BRD.Buffs.StraightShotReady))
                    return OriginalHook(BRD.StraightShot);
            }

            return actionID;
        }
    }

    internal class BardIronJawsFeaturePlus : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BardIronJawsFeaturePlus;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.IronJaws)
            {
                if (!CanUseAction(BRD.IronJaws))
                {
                    var venomous = FindTargetEffect(BRD.Debuffs.VenomousBite);
                    var windbite = FindTargetEffect(BRD.Debuffs.Windbite);
                    if (venomous is not null && windbite is not null)
                    {
                        if (venomous?.RemainingTime < windbite?.RemainingTime)
                            return BRD.VenomousBite;
                        return BRD.Windbite;
                    }
                    else if (windbite is not null || !CanUseAction(BRD.Windbite))
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

    internal class BardWanderersPitchPerfectFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BardWanderersPitchPerfectFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.Peloton && CurrentTarget is not null)
            {
                var gauge = GetJobGauge<BRDGauge>();
                if (gauge.Song == Song.WANDERER)
                    return BRD.PitchPerfect;
                return BRD.WanderersMinuet;
            }

            return actionID;
        }
    }

    internal class BardIronJawsFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BardIronJawsFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            return !CanUseAction(BRD.IronJaws) || (!TargetHasEffect(BRD.Debuffs.Stormbite) && !TargetHasEffect(BRD.Debuffs.Windbite)) ? (!CanUseAction(OriginalHook(BRD.Windbite)) ? BRD.VenomousBite : OriginalHook(BRD.Stormbite)) : actionID;
        }
    }

    internal class BardApexFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BardApexFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            /*if (actionID == BRD.QuickNock || actionID == BRD.Ladonsbite)
            {
                var gauge = GetJobGauge<BRDGauge>();
                if (gauge.SoulVoice >= 80 || OriginalHook(BRD.ApexArrow) != BRD.ApexArrow)
                    return OriginalHook(BRD.ApexArrow);
            }*/
            if (actionID == BRD.ApexArrow)
            {
                var globalCD = GetCooldown(BRD.ApexArrow);
                var gauge = GetJobGauge<BRDGauge>();

                if (globalCD.CooldownRemaining > 0.7 && IsEnabled(CustomComboPreset.BardOGCDFeature))
                {
                    var pitchCD = GetCooldown(BRD.PitchPerfect);
                    var bloodCD = GetCooldown(BRD.Bloodletter);
                    var empCD = GetCooldown(BRD.EmpyrealArrow);
                    var swCD = GetCooldown(BRD.Sidewinder);

                    if (gauge.Song == Song.WANDERER && !pitchCD.IsCooldown && (gauge.Repertoire == 3 || (gauge.Repertoire >= 1 && gauge.SongTimer <= 3000)))
                        return BRD.PitchPerfect;

                    if (!bloodCD.IsCooldown)
                        return BRD.Bloodletter;

                    if (!empCD.IsCooldown && CanUseAction(BRD.EmpyrealArrow))
                        return BRD.EmpyrealArrow;

                    if (!swCD.IsCooldown && CanUseAction(BRD.Sidewinder))
                        return BRD.Sidewinder;

                    if (bloodCD.CooldownRemaining <= 30) return BRD.Bloodletter;
                }
            }

            return BRD.ApexArrow;
        }
    }

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

                    if (HasEffect(BRD.Buffs.ShadowbiteReady))
                        return BRD.Barrage;

                    if (!rainCD.IsCooldown)
                        return BRD.RainOfDeath;

                    if (gauge.Song == Song.WANDERER && !pitchCD.IsCooldown && (gauge.Repertoire == 3 || (gauge.Repertoire >= 1 && gauge.SongTimer <= 3000)))
                        return BRD.PitchPerfect;

                    if (!empCD.IsCooldown && CanUseAction(BRD.EmpyrealArrow))
                        return BRD.EmpyrealArrow;

                    if (!swCD.IsCooldown && CanUseAction(BRD.Sidewinder))
                        return BRD.Sidewinder;

                    if (rainCD.CooldownRemaining <= 30) return BRD.RainOfDeath;
                }

                if (IsEnabled(CustomComboPreset.BardApexFeature) && (gauge.SoulVoice == 80 || OriginalHook(BRD.ApexArrow) != BRD.ApexArrow))
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
                var gauge = GetJobGauge<BRDGauge>();

                // return sprint if they don't have any songs unlocked.
                if (!CanUseAction(BRD.MagesBallad))
                    return BRD.Peloton;

                // return whichever is highest priority and off CD for lvl 90
                if (level == 90)
                {
                    if (!wmCD.IsCooldown && gauge.Coda[2] == Song.NONE)
                        return BRD.WanderersMinuet;
                    if (!mbCD.IsCooldown && gauge.Coda[0] == Song.NONE)
                        return BRD.MagesBallad;
                    if (!apCD.IsCooldown && gauge.Coda[1] == Song.NONE)
                        return BRD.ArmysPaeon;
                }

                // return whichever is highest priority and off CD
                if (!wmCD.IsCooldown && CanUseAction(BRD.WanderersMinuet))
                    return BRD.WanderersMinuet;
                if (!mbCD.IsCooldown)
                    return BRD.MagesBallad;
                if (!apCD.IsCooldown && CanUseAction(BRD.ArmysPaeon))
                    return BRD.ArmysPaeon;

                // if all three are on CD, return whichever is shortest CD for visibility
                if (wmCD.CooldownRemaining <= mbCD.CooldownRemaining && wmCD.CooldownRemaining <= apCD.CooldownRemaining && CanUseAction(BRD.WanderersMinuet))
                    return BRD.WanderersMinuet;
                if (apCD.CooldownRemaining <= mbCD.CooldownRemaining && CanUseAction(BRD.ArmysPaeon))
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
            return (IsActionOffCooldown(BRD.Sidewinder) && !IsActionOffCooldown(BRD.EmpyrealArrow) && CanUseAction(BRD.Sidewinder)) ? BRD.Sidewinder : BRD.EmpyrealArrow;
        }
    }

    internal class BardRadiantStrikesFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BardRadiantStrikesFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            return IsActionOffCooldown(BRD.RagingStrikes) || !CanUseAction(BRD.BattleVoice) || (IsEnabled(CustomComboPreset.BardRadiantFeature) && !IsActionOffCooldown(BRD.BattleVoice) && level < BRD.Levels.RadiantFinale) ? BRD.RagingStrikes : actionID;
        }
    }

    internal class BardRadiantFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BardRadiantFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            return (IsActionOffCooldown(BRD.BattleVoice) || level < BRD.Levels.RadiantFinale) ? BRD.BattleVoice : actionID;
        }
    }

    internal class BardBarrageFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BardBarrageFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            return HasEffect(BRD.Buffs.StraightShotReady) && !HasEffect(BRD.Buffs.ShadowbiteReady) ? OriginalHook(BRD.StraightShot) : BRD.Barrage;
        }
    }

    internal class BardRainFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BardRainFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            // return !TargetHasEffect(BRD.Debuffs.CausticBite) && !TargetHasEffect(BRD.Debuffs.Stormbite) && !TargetHasEffect(BRD.Debuffs.Windbite) && !TargetHasEffect(BRD.Debuffs.VenomousBite) && CanUseAction(BRD.RainOfDeath) ? BRD.RainOfDeath : BRD.Bloodletter;
            return (this.FilteredLastComboMove == OriginalHook(BRD.QuickNock) || this.FilteredLastComboMove == OriginalHook(BRD.Shadowbite)) && CanUseAction(BRD.RainOfDeath) ? BRD.RainOfDeath : BRD.Bloodletter;
        }
    }
}
