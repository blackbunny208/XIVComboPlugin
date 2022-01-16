using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedestPlugin.Combos
{
    internal static class SGE
    {
        public const byte JobID = 40;

        public const uint
            Dosis = 24283,
            Diagnosis = 24284,
            Prognosis = 24286,
            Holos = 24310,
            Ixochole = 24299,
            Taurochole = 24303,
            Druochole = 24296,
            Egeiro = 24287,
            Kardia = 24285,
            Soteria = 24294,
            Toxikon = 24304,
            Phlegma = 24289,
            Phlegmara = 24307,
            Phlegmaga = 24313,
            Dyskrasia = 24297,
            Zoe = 24300,
            Pepsis = 24301,
            Physis = 24288,
            Physis2 = 24302,
            Rhizomata = 24309,
            Krasis = 24317,
            Kerachole = 24298,
            Haima = 24305,
            Panhaima = 24311,
            Pneuma = 24318,
            Eukrasia = 24290;

        public static class Buffs
        {
            public const ushort
                Kardia = 2604;
        }

        public static class Debuffs
        {
            public const ushort
                Placeholder = 0;
        }

        public static class Levels
        {
            public const ushort
                Dosis = 1,
                Prognosis = 10,
                Soteria = 35,
                Druochole = 45,
                Kerachole = 50,
                Ixochole = 52,
                Physis2 = 60,
                Taurochole = 62,
                Haima = 70,
                Dosis2 = 72,
                Rizomata = 74,
                Holos = 76,
                Panhaima = 80,
                Dosis3 = 82,
                Krasis = 86,
                Pneuma = 90;
        }
    }

    internal class SageKardiaFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.SageKardiaFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SGE.Soteria)
            {
                if (HasEffect(SGE.Buffs.Kardia) && IsActionOffCooldown(SGE.Soteria))
                    return SGE.Soteria;
                return SGE.Kardia;
            }

            return actionID;
        }
    }

    internal class SageExtremeButtonSaverFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.SageExtremeButtonSaverFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (CurrentTarget is null)
            {
                if (actionID == SGE.Haima && level >= SGE.Levels.Panhaima)
                    return SGE.Panhaima;
                if (actionID == SGE.Taurochole && level >= SGE.Levels.Kerachole)
                    return SGE.Kerachole;
                if (actionID == SGE.Krasis)
                    return OriginalHook(SGE.Physis);
                if (actionID == SGE.Druochole && level >= SGE.Levels.Ixochole)
                    return SGE.Ixochole;
                if (actionID == SGE.Diagnosis && level >= SGE.Levels.Prognosis)
                    return OriginalHook(SGE.Prognosis);
            }

            if (actionID == SGE.Krasis && level < SGE.Levels.Krasis)
                return OriginalHook(SGE.Physis);

            if (actionID == SGE.Pneuma && level < SGE.Levels.Pneuma)
                return SGE.Holos;

            return actionID;
        }
    }

    internal class SagePhlegmaToxicBalls : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.SagePhlegmaToxicBalls;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (CurrentTarget is null && IsEnabled(CustomComboPreset.SagePhlegmaBalls))
                return OriginalHook(SGE.Dyskrasia);

            var gauge = GetJobGauge<SGEGauge>();

            if (level >= SGE.Levels.Dosis3)
                if (GetCooldown(SGE.Phlegmaga).CooldownRemaining > 45 && gauge.Addersting > 0)
                    return OriginalHook(SGE.Toxikon);

            if (level >= SGE.Levels.Dosis2)
                if (GetCooldown(SGE.Phlegmara).CooldownRemaining > 45 && gauge.Addersting > 0)
                    return OriginalHook(SGE.Toxikon);

            if (GetCooldown(SGE.Phlegma).CooldownRemaining > 45 && gauge.Addersting > 0)
                return OriginalHook(SGE.Toxikon);

            return actionID;
        }
    }

    internal class SagePhlegmaBalls : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.SagePhlegmaBalls;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (CurrentTarget is null)
                return OriginalHook(SGE.Dyskrasia);

            if (level >= SGE.Levels.Dosis3)
                if (GetCooldown(SGE.Phlegmaga).CooldownRemaining > 45)
                    return OriginalHook(SGE.Dyskrasia);

            if (level >= SGE.Levels.Dosis2)
                if (GetCooldown(SGE.Phlegmara).CooldownRemaining > 45)
                    return OriginalHook(SGE.Dyskrasia);

            if (GetCooldown(SGE.Phlegma).CooldownRemaining > 45)
                return OriginalHook(SGE.Dyskrasia);

            return actionID;
        }
    }

    internal class SageTauroDruoFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.SageTauroDruoFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (!IsActionOffCooldown(SGE.Taurochole) || level < SGE.Levels.Taurochole)
                return SGE.Druochole;

            return actionID;
        }
    }

    internal class SageEukrasiaDosisFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.SageEukrasiaDosisFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            return GetJobGauge<SGEGauge>().Eukrasia ? OriginalHook(SGE.Dosis) : actionID;
        }
    }

    internal class SageLucidReminderFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.SageLucidReminderFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if ((actionID == SGE.Physis || actionID == SGE.Physis2) &&
                IsActionOffCooldown(All.LucidDreaming) && HasCondition(ConditionFlag.InCombat) && !IsActionOffCooldown(level >= SGE.Levels.Physis2 ? SGE.Physis2 : SGE.Physis) && LocalPlayer?.CurrentMp <= 9000)
                return All.LucidDreaming;

            return IsActionOffCooldown(All.LucidDreaming) && HasCondition(ConditionFlag.InCombat) && !IsActionOffCooldown(actionID) && LocalPlayer?.CurrentMp <= 9000 ? All.LucidDreaming : actionID;
        }
    }
}
