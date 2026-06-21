using RimWorld;
using System.Collections.Generic;
using System.Linq; // Added for Any() extension method
using Verse;

namespace ShinyMathRocks
{
    public class GameComponent_ShinyMathRocks : GameComponent
    {
        private Queue<Window_DiceRoll> diceRollQueue = new Queue<Window_DiceRoll>();

        public static GameComponent_ShinyMathRocks Instance => Current.Game?.GetComponent<GameComponent_ShinyMathRocks>();

        public GameComponent_ShinyMathRocks(Game game)
        {
        }

        public void EnqueueDiceRollWindow(Window_DiceRoll window)
        {
            if (window == null) return;
            diceRollQueue.Enqueue(window);
        }

        public override void GameComponentTick()
        {
            base.GameComponentTick();

            if (diceRollQueue.Any() && !IsDiceRollWindowOpen())
            {
                Window_DiceRoll nextWindow = diceRollQueue.Dequeue();
                Find.WindowStack.Add(nextWindow);
            }
        }

        private bool IsDiceRollWindowOpen()
        {
            foreach (Window window in Find.WindowStack.Windows)
            {
                if (window is Window_DiceRoll)
                {
                    return true;
                }
            }
            return false;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            // We do not need to save the queue itself, as rolls are transient.
            // If the game is saved and loaded, the queue should be empty or reset.
            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                diceRollQueue.Clear();
            }
        }
    }
}
