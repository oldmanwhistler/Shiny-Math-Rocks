using UnityEngine;
using Verse;

namespace ShinyMathRocks
{
    public class ShinyMathRocksSettings : ModSettings
    {
        public bool showRollWindow = true;
        public float moodScale = 1f;
        public int rollsRequired = 3;
        public float buffValue = 0.01f;

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
            listing.Label("Optimization delta per tier: " + ShinyMathRocksMod.Settings.rollsRequired);
            ShinyMathRocksMod.Settings.rollsRequired = (int)listing.Slider(ShinyMathRocksMod.Settings.rollsRequired, 1, 20);
            listing.Gap(4f);
            listing.Label("Consciousness buff/debuff per tier: " + ShinyMathRocksMod.Settings.buffValue.ToStringPercent());
            ShinyMathRocksMod.Settings.buffValue = listing.Slider(ShinyMathRocksMod.Settings.buffValue, 0.001f, 0.05f);
            listing.Gap(8f);
            if (listing.ButtonText("Reset settings"))
            {
                ShinyMathRocksMod.Settings.showRollWindow = true;
                ShinyMathRocksMod.Settings.moodScale = 1f;
                ShinyMathRocksMod.Settings.rollsRequired = 3;
                ShinyMathRocksMod.Settings.buffValue = 0.01f;
            }
            listing.End();
        }
    }
}
