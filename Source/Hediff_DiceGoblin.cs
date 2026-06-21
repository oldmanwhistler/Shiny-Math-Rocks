using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine; // Added
using Verse;

namespace ShinyMathRocks
{
    public class Hediff_DiceGoblin : Hediff    {
        public int totalRolls;
        public int nat20Count;
        public int nat1Count;
        public float Lucky => nat20Count - 0.5f * nat1Count;
        public Dictionary<string, int> themeNat20s = new Dictionary<string, int>();

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref totalRolls, "totalRolls", 0);
            Scribe_Values.Look(ref nat20Count, "nat20Count", 0);
            Scribe_Values.Look(ref nat1Count, "nat1Count", 0);
            Scribe_Collections.Look(ref themeNat20s, "themeNat20s", LookMode.Value, LookMode.Value);
            if (Scribe.mode == LoadSaveMode.PostLoadInit && themeNat20s == null)
            {
                themeNat20s = new Dictionary<string, int>();
            }
        }

        public string FavoriteDieLabel
        {
            get
            {
                if (themeNat20s == null || themeNat20s.Count == 0)
                {
                    return "None".Translate();
                }

                string favoriteThemeDefName = null;
                int maxNat20s = -1;

                foreach (var entry in themeNat20s)
                {
                    if (entry.Value > maxNat20s)
                    {
                        maxNat20s = entry.Value;
                        favoriteThemeDefName = entry.Key;
                    }
                    else if (entry.Value == maxNat20s && favoriteThemeDefName != null)
                    {
                        // Tie-breaker: alphabetical order or first found. Let's stick with first found for now for simplicity.
                        // Can add alphabetical sort if needed later.
                    }
                }

                if (favoriteThemeDefName != null)
                {
                    DiceThemeDef themeDef = DefDatabase<DiceThemeDef>.GetNamedSilentFail(favoriteThemeDefName);
                    if (themeDef != null)
                    {
                        return $"{themeDef.LabelCap}\n\n  {themeDef.description}\n\n{maxNat20s} Nat 20s with fav die.";
                    }
                }

                return "None".Translate();
            }
        }

        public IEnumerable<PawnCapacityModifier> GetPawnCapacityModifiers() // This method will be called by a HediffComp
        {
            float buffValue = ShinyMathRocksMod.Settings?.buffValue ?? 0.01f;
            int rollsRequired = ShinyMathRocksMod.Settings?.rollsRequired ?? 3;

            if (rollsRequired <= 0) rollsRequired = 1; // Prevent division by zero

            float effectiveDelta = Lucky;
            float consciousnessOffset = 0f;

            if (effectiveDelta != 0)
            {
                int tier = Mathf.FloorToInt(Mathf.Abs(effectiveDelta) / rollsRequired);
                consciousnessOffset = buffValue * tier * Mathf.Sign(effectiveDelta);
            }
            
            if (consciousnessOffset != 0f)
            {
                yield return new PawnCapacityModifier // Using PawnCapacityModifier directly
                {
                    capacity = PawnCapacityDefOf.Consciousness,
                    offset = consciousnessOffset
                };
            }
        }

        public override string TipStringExtra
        {
            get
            {
                string baseTip = base.TipStringExtra;
                string stats = "SMR_DiceGoblinStats".Translate(
                    totalRolls,
                    nat20Count,
                    nat1Count,
                    Lucky,
                    FavoriteDieLabel
                    );
                return baseTip + "\n" + stats;
            }
        }
    }
}
