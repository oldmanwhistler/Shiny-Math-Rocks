using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace ShinyMathRocks
{
    [StaticConstructorOnStartup]
    public static class ShinyMathRocksHarmony
    {
        static ShinyMathRocksHarmony()
        {
            var harmony = new Harmony("oldmanwhistler.ShinyMathRocks");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        // Postfix to Hediff.CapMods getter to inject dynamic capacity modifiers
        [HarmonyPatch(typeof(Hediff), "get_CapMods")]
        public static class CapMods_Patch
        {
            public static void Postfix(Hediff __instance, ref List<PawnCapacityModifier> __result)
            {
                // Only apply if the hediff is our specific DiceGoblin hediff
                if (__instance is Hediff_DiceGoblin diceGoblinHediff)
                {
                    // Ensure __result is initialized if original CapMods was null (unlikely but safe)
                    if (__result == null)
                    {
                        __result = new List<PawnCapacityModifier>();
                    }

                    // Add our dynamically calculated capacity modifiers
                    foreach (var modifier in diceGoblinHediff.GetPawnCapacityModifiers())
                    {
                        __result.Add(modifier);
                    }
                }
            }
        }
    }
}
