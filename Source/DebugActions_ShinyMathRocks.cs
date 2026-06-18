using LudeonTK;
using RimWorld;
using Verse;

namespace ShinyMathRocks
{
    public static class DebugActions_ShinyMathRocks
    {
        [DebugAction("Shiny Math Rocks", "Spawn 10 shiny math rocks", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void SpawnShinyMathRocks()
        {
            Map map = Find.CurrentMap;
            if (map == null)
            {
                return;
            }

            IntVec3 cell = UI.MouseCell();
            if (!cell.InBounds(map))
            {
                return;
            }

            Thing dice = ThingMaker.MakeThing(ShinyMathRocksDefOf.SMR_ShinyMathRock);
            dice.stackCount = 10;
            GenPlace.TryPlaceThing(dice, cell, map, ThingPlaceMode.Near);
            Messages.Message("Spawned 10 shiny math rocks.", new TargetInfo(cell, map), MessageTypeDefOf.PositiveEvent, historical: false);
        }

        [DebugAction("Shiny Math Rocks", "Selected pawn rolls now", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void SelectedPawnRollsNow()
        {
            Pawn pawn = Find.Selector.SingleSelectedThing as Pawn;
            if (pawn == null)
            {
                Map map = Find.CurrentMap;
                if (map != null && map.mapPawns.FreeColonistsSpawned.Count > 0)
                {
                    pawn = map.mapPawns.FreeColonistsSpawned[0];
                }
            }

            if (pawn == null)
            {
                Messages.Message("Select a pawn first, or start a map with a colonist.", MessageTypeDefOf.RejectInput, historical: false);
                return;
            }

            DiceRollUtility.RollForPawn(pawn);
        }
    }
}
