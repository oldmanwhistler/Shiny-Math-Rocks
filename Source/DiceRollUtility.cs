using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;
using System.Collections.Generic;

namespace ShinyMathRocks
{
    public static class DiceRollUtility
    {
        public static DiceThemeDef RandomDiceTheme()
        {
            List<DiceThemeDef> defs = DefDatabase<DiceThemeDef>.AllDefsListForReading;
            if (defs != null && defs.Count > 0)
            {
                defs.Shuffle(); // Randomize selection
                foreach (var def in defs)
                {
                    if (def != null && !def.texPath.NullOrEmpty())
                    {
                        Texture2D tex = ContentFinder<Texture2D>.Get(def.texPath, reportFailure: false);
                        if (tex != null)
                        {                            
                            return def;
                        }
                    }
                }
            }

            DiceThemeDef defaultTheme = DefDatabase<DiceThemeDef>.GetNamedSilentFail("SMR_TranslucentSapphireD20");
            return defaultTheme;
        }

        public static int RollForPawn(Pawn pawn)
        {
            int roll = Rand.RangeInclusive(1, 20);
            ApplyRoll(pawn, roll); // Pass null for diceThemeDefName
            return roll;
        }

        public static void ApplyRoll(Pawn pawn, int roll)
        {
            if (pawn == null)
            {
                return;
            }

            // Get or add Dice Goblin hediff
            Hediff_DiceGoblin diceGoblinHediff = pawn.health.hediffSet.GetFirstHediffOfDef(ShinyMathRocksDefOf.SMR_DiceGoblinStatus) as Hediff_DiceGoblin;
            if (diceGoblinHediff == null)
            {
                diceGoblinHediff = (Hediff_DiceGoblin)HediffMaker.MakeHediff(ShinyMathRocksDefOf.SMR_DiceGoblinStatus, pawn);
                pawn.health.AddHediff(diceGoblinHediff);
            }

            DiceThemeDef diceThemeDef = RandomDiceTheme();
            string diceThemeDefName = diceThemeDef?.defName;

            // Update stats
            diceGoblinHediff.totalRolls++;
            if (roll == 20)
            {
                diceGoblinHediff.nat20Count++;
                if (!diceThemeDefName.NullOrEmpty())
                {
                    if (diceGoblinHediff.themeNat20s.ContainsKey(diceThemeDefName))
                    {
                        diceGoblinHediff.themeNat20s[diceThemeDefName]++;
                    }
                    else
                    {
                        diceGoblinHediff.themeNat20s.Add(diceThemeDefName, 1);
                    }
                }
                else
                {
                    Log.Error($"{pawn.LabelShort} rolled a nat 20 without the dice name. This should not happen. Please report to mod author.");
                    
                }
            }
            else if (roll == 1)
            {
                diceGoblinHediff.nat1Count++;
            }

            // Existing mood and UI logic
            int stage = StageForRoll(roll);

            if (pawn.needs?.mood?.thoughts?.memories != null)
            {
                Thought_Memory memory = ThoughtMaker.MakeThought(ShinyMathRocksDefOf.SMR_DiceRollMood, stage);
                memory.moodPowerFactor = ShinyMathRocksMod.Settings?.moodScale ?? 1f;
                pawn.needs.mood.thoughts.memories.TryGainMemory(memory);
            }

            if (pawn.Map == Find.CurrentMap)
            {
                PawnOnMap(pawn, roll, stage, diceThemeDef);
            }            
        }


        private static void PawnOnMap(Pawn pawn, int roll, int stage, DiceThemeDef diceThemeDef)
        {
            if (pawn.Spawned && ShinyMathRocksDefOf.SMR_DiceClatter != null)
            {
                ShinyMathRocksDefOf.SMR_DiceClatter.PlayOneShot(
                    SoundInfo.InMap(new TargetInfo(pawn.Position, pawn.Map)));
            }
            if (PawnUtility.ShouldSendNotificationAbout(pawn))
            {
                MessageTypeDef messageType = roll == 1
                    ? MessageTypeDefOf.NegativeEvent
                    : (roll == 20 || roll >= 16 ? MessageTypeDefOf.PositiveEvent : MessageTypeDefOf.NeutralEvent);
                Messages.Message(pawn.LabelShortCap + " rolled a " + roll + ": " + LabelForStage(stage), pawn,
                    messageType, historical: false);

                if (ShinyMathRocksMod.Settings == null || ShinyMathRocksMod.Settings.showRollWindow)
                {
                    GameComponent_ShinyMathRocks.Instance.EnqueueDiceRollWindow(new Window_DiceRoll(pawn.LabelShortCap, roll, stage, diceThemeDef));
                }
            }
        }

        public static int StageForRoll(int roll)
        {
            if (roll <= 1) return 0;
            if (roll <= 5) return 1;
            if (roll <= 10) return 2;
            if (roll <= 15) return 3;
            if (roll <= 19) return 4;
            return 5;
        }

        public static string LabelForStage(int stage)
        {
            switch (stage)
            {
                case 0: return "nat 1";
                case 1: return "terrible roll";
                case 2: return "mediocre roll";
                case 3: return "decent roll";
                case 4: return "great roll";
                default: return "nat 20!";
            }
        }
    }
}
