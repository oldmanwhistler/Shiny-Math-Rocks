using RimWorld;
using Verse;

namespace ShinyMathRocks
{
    public class IngestionOutcomeDoer_RollDice : IngestionOutcomeDoer
    {
        protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested, int ingestedCount)
        {
            DiceRollUtility.RollForPawn(pawn); // No theme for ingestion outcome
        }
    }
}
