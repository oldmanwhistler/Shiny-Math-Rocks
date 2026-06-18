using RimWorld;
using Verse;
using Verse.AI;

namespace ShinyMathRocks
{
    public class FloatMenuOptionProvider_RollDice : FloatMenuOptionProvider
    {
        protected override bool Drafted => true;
        protected override bool Undrafted => true;
        protected override bool Multiselect => false;
        protected override bool RequiresManipulation => true;

        protected override FloatMenuOption GetSingleOptionFor(Thing clickedThing, FloatMenuContext context)
        {
            if (clickedThing.def != ShinyMathRocksDefOf.SMR_ShinyMathRock)
            {
                return null;
            }

            Pawn pawn = context.FirstSelectedPawn;
            string label = "Roll " + clickedThing.LabelShort;

            if (!clickedThing.IngestibleNow || !pawn.RaceProps.CanEverEat(clickedThing.def))
            {
                return null;
            }

            if (!clickedThing.IsSociallyProper(pawn))
            {
                return new FloatMenuOption(label + ": " + "ReservedForPrisoners".Translate().CapitalizeFirst(), null);
            }

            if (clickedThing.def.IsDrug && !pawn.DrugIsSuitable(clickedThing.def))
            {
                return new FloatMenuOption(label + ": " + "DrugNotSuitable".Translate().CapitalizeFirst(), null);
            }

            if (clickedThing.def.IsNonMedicalDrug && !pawn.CanTakeDrug(clickedThing.def))
            {
                return new FloatMenuOption(label + ": " + TraitDefOf.DrugDesire.DataAtDegree(-1).GetLabelCapFor(pawn), null);
            }

            if (!pawn.CanReach(clickedThing, PathEndMode.OnCell, Danger.Deadly))
            {
                return new FloatMenuOption(label + ": " + "NoPath".Translate().CapitalizeFirst(), null);
            }

            FloatMenuOption option = new FloatMenuOption(label, delegate
            {
                int count = FoodUtility.GetMaxAmountToPickup(clickedThing, pawn, 1);
                if (count <= 0)
                {
                    return;
                }

                clickedThing.SetForbidden(value: false);
                Job job = JobMaker.MakeJob(ShinyMathRocksDefOf.SMR_RollDice, clickedThing);
                job.count = 1;
                pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            }, MenuOptionPriority.Default);

            return FloatMenuUtility.DecoratePrioritizedTask(option, pawn, clickedThing);
        }
    }
}
