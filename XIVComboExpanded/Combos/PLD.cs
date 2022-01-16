using Dalamud.Game.ClientState.Statuses;

namespace XIVComboExpandedestPlugin.Combos
{
    internal static class PLD
    {
        public const byte ClassID = 1;
        public const byte JobID = 19;

        public const uint
            FastBlade = 9,
            RiotBlade = 15,
            ShieldBash = 16,
            RageOfHalone = 21,
            GoringBlade = 3538,
            RoyalAuthority = 3539,
            LowBlow = 7540,
            TotalEclipse = 7381,
            Requiescat = 7383,
            HolySpirit = 7384,
            Prominence = 16457,
            HolyCircle = 16458,
            Confiteor = 16459,
            Atonement = 16460,
            BladeOfFaith = 25748,
            BladeOfTruth = 25749,
            BladeOfValor = 25750,
            SpiritsWithin = 29,
            Expiacion = 25747,
            CircleOfScorn = 23;

        public static class Buffs
        {
            public const ushort
                Requiescat = 1368,
                SwordOath = 1902;
        }

        public static class Debuffs
        {
            public const ushort Placeholder = 0;
        }

        public static class Levels
        {
            public const byte
                RiotBlade = 4,
                RageOfHalone = 26,
                Prominence = 40,
                CircleOfScorn = 50,
                GoringBlade = 54,
                RoyalAuthority = 60,
                HolyCircle = 72,
                Atonement = 76,
                Confiteor = 80,
                Expiacion = 86;
        }
    }

    internal class PaladinGoringBladeAtonementFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.PaladinGoringBladeAtonementFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            return HasEffect(PLD.Buffs.SwordOath) && lastComboMove != PLD.FastBlade && lastComboMove != PLD.RiotBlade ? PLD.Atonement : actionID;
        }
    }

    internal class PaladinGoringBladeCombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.PaladinGoringBladeCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == PLD.GoringBlade)
            {
                if (comboTime > 0)
                {
                    if (lastComboMove == PLD.FastBlade && level >= PLD.Levels.RiotBlade)
                        return PLD.RiotBlade;

                    if (lastComboMove == PLD.RiotBlade && level >= PLD.Levels.GoringBlade)
                        return PLD.GoringBlade;
                }

                return PLD.FastBlade;
            }

            return actionID;
        }
    }

    internal class PaladinRoyalAuthorityCombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.PaladinRoyalAuthorityCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == PLD.RoyalAuthority || actionID == PLD.RageOfHalone)
            {
                if (comboTime > 0)
                {
                    if (lastComboMove == PLD.FastBlade && level >= PLD.Levels.RiotBlade)
                        return PLD.RiotBlade;

                    if (lastComboMove == PLD.RiotBlade && level >= PLD.Levels.RageOfHalone)
                        return OriginalHook(PLD.RageOfHalone);
                }

                if (IsEnabled(CustomComboPreset.PaladinAtonementFeature))
                {
                    if (HasEffect(PLD.Buffs.SwordOath))
                        return PLD.Atonement;
                }

                return PLD.FastBlade;
            }

            return actionID;
        }
    }

    internal class PaladinProminenceCombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.PaladinProminenceCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == (IsEnabled(CustomComboPreset.PaladinEvilProminenceCombo) ? PLD.TotalEclipse : PLD.Prominence))
            {
                if (comboTime > 0)
                {
                    if (lastComboMove == PLD.TotalEclipse && level >= PLD.Levels.Prominence)
                        return PLD.Prominence;
                }

                return PLD.TotalEclipse;
            }

            return actionID;
        }
    }

    internal class PaladinConfiteorFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.PaladinConfiteorFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == PLD.HolySpirit || actionID == PLD.HolyCircle)
            {
                if (OriginalHook(PLD.Confiteor) != PLD.Confiteor)
                    return OriginalHook(PLD.Confiteor);

                Status? requiescat = FindEffect(PLD.Buffs.Requiescat);

                if (requiescat != null)
                {
                    if (requiescat.StackCount <= 1 && level >= PLD.Levels.Confiteor)
                    {
                        return OriginalHook(PLD.Confiteor);
                    }
                }

                return actionID;
            }

            return actionID;
        }
    }

    internal class PaladinRequiescatCombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.PaladinRequiescatCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == PLD.Requiescat)
            {
                if ((HasEffect(PLD.Buffs.Requiescat) && level >= PLD.Levels.Confiteor) || OriginalHook(PLD.Confiteor) != PLD.Confiteor)
                    return OriginalHook(PLD.Confiteor);

                return PLD.Requiescat;
            }

            return actionID;
        }
    }

    internal class PaladinScornfulSpiritsFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.PaladinScornfulSpiritsFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (level < PLD.Levels.CircleOfScorn)
                return PLD.SpiritsWithin;

            var oppositeActionID = actionID == PLD.SpiritsWithin || actionID == PLD.Expiacion ? PLD.CircleOfScorn : PLD.SpiritsWithin;

            if (!IsActionOffCooldown(actionID) && IsActionOffCooldown(oppositeActionID))
                return OriginalHook(oppositeActionID);

            return actionID;
        }
    }
}
