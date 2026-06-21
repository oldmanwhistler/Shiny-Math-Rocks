using RimWorld;
using Verse;

namespace ShinyMathRocks
{
    [DefOf]
    public static class ShinyMathRocksDefOf
    {
        public static ThingDef SMR_ShinyMathRock;
        public static JobDef SMR_RollDice;
        public static ThoughtDef SMR_DiceRollMood;
        public static SoundDef SMR_DiceClatter;
        public static HediffDef SMR_DiceGoblinStatus;

        static ShinyMathRocksDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ShinyMathRocksDefOf));
        }
    }
}
