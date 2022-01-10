using System;

using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Enums;

namespace XIVComboExpandedestPlugin.Combos
{
    internal static class NIN
    {
        public const byte ClassID = 29;
        public const byte JobID = 30;

        public const uint
            SpinningEdge = 2240,
            GustSlash = 2242,
            Hide = 2245,
            Assassinate = 8814,
            Mug = 2248,
            DeathBlossom = 2254,
            AeolianEdge = 2255,
            TrickAttack = 2258,
            Ninjutsu = 2260,
            Chi = 2261,
            JinNormal = 2263,
            Kassatsu = 2264,
            ArmorCrush = 3563,
            DreamWithinADream = 3566,
            TenChiJin = 7403,
            HakkeMujinsatsu = 16488,
            Meisui = 16489,
            Jin = 18807,
            Bunshin = 16493,
            Huraijin = 25876,
            PhantomKamaitachi = 25774,
            ForkedRaiju = 25777,
            FleetingRaiju = 25778,
            ThrowingDagger = 2247;

        public static class Buffs
        {
            public const ushort
                Mudra = 496,
                Kassatsu = 497,
                Suiton = 507,
                Hidden = 614,
                Bunshin = 1954,
                RaijuReady = 2690,
                FleetingRaijuReady = 2691;
        }

        public static class Debuffs
        {
            public const ushort Placeholder = 0;
        }

        public static class Levels
        {
            public const byte
                GustSlash = 4,
                AeolianEdge = 26,
                Assassinate = 40,
                Kassatsu = 50,
                HakkeMujinsatsu = 52,
                ArmorCrush = 54,
                DreamWithinADream = 56,
                Huraijin = 60,
                Meisui = 72,
                EnhancedKassatsu = 76,
                Bunshin = 80,
                PhantomKamaitachi = 82,
                Raiju = 90;
        }
    }

    internal class NinjaNinjutsuFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.NinjaNinjutsuFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == NIN.AeolianEdge)
            {
                if (HasEffect(NIN.Buffs.Mudra))
                    return OriginalHook(NIN.Ninjutsu);
            }

            return actionID;
        }
    }

    internal class NinjaGCDNinjutsuFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.NinjaGCDNinjutsuFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (HasEffect(NIN.Buffs.Mudra))
                return OriginalHook(NIN.Ninjutsu);

            return actionID;
        }
    }

    internal class NinjaArmorCrushCombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.NinjaArmorCrushCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == NIN.ArmorCrush)
            {
                if (IsEnabled(CustomComboPreset.NinjaArmorCrushRaijuFeature))
                {
                    if (level >= NIN.Levels.Raiju && HasEffect(NIN.Buffs.RaijuReady))
                    {
                        if (IsEnabled(CustomComboPreset.NinjaSmartRaijuFeature))
                            return InMeleeRange() ? NIN.FleetingRaiju : NIN.ForkedRaiju;
                        return NIN.ForkedRaiju;
                    }
                }

                if (comboTime > 0)
                {
                    if (lastComboMove == NIN.SpinningEdge && level >= NIN.Levels.GustSlash)
                        return NIN.GustSlash;

                    if (lastComboMove == NIN.GustSlash && level >= NIN.Levels.ArmorCrush)
                        return NIN.ArmorCrush;
                }

                return NIN.SpinningEdge;
            }

            return actionID;
        }
    }

    internal class NinjaAeolianEdgeCombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.NinjaAeolianEdgeCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == NIN.AeolianEdge)
            {
                if (IsEnabled(CustomComboPreset.NinjaAeolianEdgeRaijuFeature))
                {
                    if (level >= NIN.Levels.Raiju && HasEffect(NIN.Buffs.RaijuReady))
                    {
                        if (IsEnabled(CustomComboPreset.NinjaSmartRaijuFeature))
                            return InMeleeRange() ? NIN.FleetingRaiju : NIN.ForkedRaiju;
                        return NIN.FleetingRaiju;
                    }
                }

                if (comboTime > 0)
                {
                    if (lastComboMove == NIN.SpinningEdge && level >= NIN.Levels.GustSlash)
                        return NIN.GustSlash;

                    if (lastComboMove == NIN.GustSlash && level >= NIN.Levels.AeolianEdge)
                        return NIN.AeolianEdge;
                }

                return NIN.SpinningEdge;
            }

            return actionID;
        }
    }

    internal class NinjaHakkeMujinsatsuCombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.NinjaHakkeMujinsatsuCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == (IsEnabled(CustomComboPreset.NinjaEvilHakkeMujinsatsuCombo) ? NIN.DeathBlossom : NIN.HakkeMujinsatsu))
            {
                if (comboTime > 0 && lastComboMove == NIN.DeathBlossom && level >= NIN.Levels.HakkeMujinsatsu)
                    return NIN.HakkeMujinsatsu;

                return NIN.DeathBlossom;
            }

            return actionID;
        }
    }

    internal class NinjaKassatsuTrickFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.NinjaKassatsuTrickFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == NIN.Kassatsu)
            {
                if ((HasEffect(NIN.Buffs.Suiton) && !IsActionOffCooldown(NIN.Kassatsu)) || HasEffect(NIN.Buffs.Hidden))
                    return NIN.TrickAttack;
            }

            return actionID;
        }
    }

    internal class NinjaKassatsuDWaDFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.NinjaKassatsuDWaDFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            return (!IsActionOffCooldown(NIN.Kassatsu) && IsActionOffCooldown(NIN.DreamWithinADream)) || level < NIN.Levels.Kassatsu ? OriginalHook(NIN.DreamWithinADream) : actionID;
        }
    }

    internal class NinjaHideMugFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.NinjaHideMugFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == NIN.Hide)
            {
                if (HasCondition(ConditionFlag.InCombat))
                    return NIN.Mug;
            }

            return actionID;
        }
    }

    internal class NinjaKassatsuChiJinFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.NinjaKassatsuChiJinFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == NIN.Chi)
            {
                if (level >= NIN.Levels.EnhancedKassatsu && HasEffect(NIN.Buffs.Kassatsu))
                    return NIN.Jin;
            }

            return actionID;
        }
    }

    internal class NinjaTCJMeisuiFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.NinjaTCJMeisuiFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == NIN.TenChiJin)
            {
                if (HasEffect(NIN.Buffs.Suiton))
                    return NIN.Meisui;

                return NIN.TenChiJin;
            }

            return actionID;
        }
    }

    internal class NinjaHuraijinRaijuFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.NinjaHuraijinRaijuFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == NIN.Huraijin)
            {
                if (level >= NIN.Levels.Raiju && HasEffect(NIN.Buffs.RaijuReady))
                {
                    if (IsEnabled(CustomComboPreset.NinjaSmartRaijuFeature))
                        return InMeleeRange() ? NIN.FleetingRaiju : NIN.ForkedRaiju;
                    return IsEnabled(CustomComboPreset.NinjaHuraijinFleetingRaijuFeature) ? NIN.FleetingRaiju : NIN.ForkedRaiju;
                }
            }

            return actionID;
        }
    }

    internal class NinjaSmartRaijuFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.NinjaSmartRaijuFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            return InMeleeRange() ? NIN.FleetingRaiju : NIN.ForkedRaiju;
        }
    }

    internal class NinjaHuraijinCrushFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.NinjaHuraijinCrushFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            return lastComboMove == NIN.GustSlash && comboTime > 0 ? NIN.ArmorCrush : actionID;
        }
    }
}
