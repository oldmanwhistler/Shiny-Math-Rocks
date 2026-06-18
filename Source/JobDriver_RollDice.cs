using RimWorld;

namespace ShinyMathRocks
{
    // Uses RimWorld's robust ingest flow (pickup, reserve, progress bar, finalize),
    // but is a distinct JobDef so orders/reports can say the pawn is rolling dice.
    public class JobDriver_RollDice : JobDriver_Ingest
    {
    }
}
