using RimWorld;
using Verse;
using Verse.Sound;

namespace ShinyMathRocks
{
    public static class DiceRollUtility
    {
        public static int RollForPawn(Pawn pawn)
        {
            int roll = Rand.RangeInclusive(1, 20);
            ApplyRoll(pawn, roll);
            return roll;
        }

        public static void ApplyRoll(Pawn pawn, int roll)
        {
            if (pawn == null)
            {
                return;
            }

            int stage = StageForRoll(roll);

            if (pawn.needs?.mood?.thoughts?.memories != null)
            {
                Thought_Memory memory = ThoughtMaker.MakeThought(ShinyMathRocksDefOf.SMR_DiceRollMood, stage);
                memory.moodPowerFactor = ShinyMathRocksMod.Settings?.moodScale ?? 1f;
                pawn.needs.mood.thoughts.memories.TryGainMemory(memory);
            }

            if (pawn.Map == Find.CurrentMap)
            {
                PawnOnMap(pawn, roll, stage);
            }            
        }

        private static void PawnOnMap(Pawn pawn, int roll, int stage)
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
                    Find.WindowStack.Add(new Window_DiceRoll(pawn.LabelShortCap, roll, stage));
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
