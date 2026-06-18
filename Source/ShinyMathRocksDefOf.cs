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

        static ShinyMathRocksDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ShinyMathRocksDefOf));
        }
    }
}
