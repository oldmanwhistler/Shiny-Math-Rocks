using UnityEngine;
using Verse;

namespace ShinyMathRocks
{
    public class ShinyMathRocksSettings : ModSettings
    {
        public bool showRollWindow = true;
        public float moodScale = 1f;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref showRollWindow, "showRollWindow", true);
            Scribe_Values.Look(ref moodScale, "moodScale", 1f);
        }
    }

    public class ShinyMathRocksMod : Mod
    {
        public static ShinyMathRocksSettings Settings;

        public ShinyMathRocksMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<ShinyMathRocksSettings>();
            Log.Message("[Shiny Math Rocks] loaded.");
        }

        public override string SettingsCategory()
        {
            return "Shiny Math Rocks";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);
            listing.CheckboxLabeled("Show D20 roll popup", ref Settings.showRollWindow, "When enabled, rolling a Shiny Math Rock opens the animated D20 result overlay.");
            listing.Gap(8f);
            listing.Label("Mood scale: " + Settings.moodScale.ToString("0.00") + "x");
            Settings.moodScale = listing.Slider(Settings.moodScale, 0f, 2f);
            listing.Gap(8f);
            if (listing.ButtonText("Reset mood scale"))
            {
                Settings.moodScale = 1f;
            }
            listing.End();
        }
    }
}
