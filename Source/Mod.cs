using UnityEngine;
using Verse;
using System.Collections.Generic; // Added for FloatMenu

namespace ShinyMathRocks
{
    public class ShinyMathRocksSettings : ModSettings
    {
        public bool showRollWindow = true;
        public float moodScale = 1f;
        public int rollsRequired = 1;
        public float buffValue = 0.005f;
        public Vector2 diceWindowPosition = new Vector2(-1f, -1f);
        public Vector2 diceWindowSize = new Vector2(460f, 350f); // Default to initial size
        public string selectedDiceThemeDefName = "SMR_TranslucentSapphireD20"; // Default selected die for viewer
        public bool onlyRoll1sOr20s = false; // New setting for forced rolls

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref showRollWindow, "showRollWindow", true);
            Scribe_Values.Look(ref moodScale, "moodScale", 1f);
            Scribe_Values.Look(ref rollsRequired, "rollsRequired", 1);
            Scribe_Values.Look(ref buffValue, "buffValue", 0.05f);
            Scribe_Values.Look(ref diceWindowPosition, "diceWindowPosition", new Vector2(-1f, -1f));
            Scribe_Values.Look(ref diceWindowSize, "diceWindowSize", new Vector2(460f, 350f));
            Scribe_Values.Look(ref selectedDiceThemeDefName, "selectedDiceThemeDefName", "SMR_TranslucentSapphireD20");
            Scribe_Values.Look(ref onlyRoll1sOr20s, "onlyRoll1sOr20s", false); // Expose new setting
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
            Settings.rollsRequired = (int)listing.Slider(Settings.rollsRequired, 1, 20);
            listing.Gap(4f);
            listing.Label("Consciousness buff/debuff per tier: " + Settings.buffValue.ToStringPercent());
            Settings.buffValue = listing.Slider(Settings.buffValue, 0.001f, 0.05f);
            listing.Gap(16f); // More gap before the viewer

            // Dice Viewer Section
            Rect viewerRect = listing.GetRect(200f); // Reserve space for viewer
            Rect dropdownRect = new Rect(viewerRect.x, viewerRect.y, viewerRect.width * 0.5f, 30f);
            Rect previewRect = new Rect(viewerRect.x + viewerRect.width * 0.5f + 10f, viewerRect.y, 100f, 100f);

            // Dropdown to select DiceThemeDef
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(dropdownRect.LeftPart(0.4f), "SMR_DiceTheme".Translate());
            Text.Anchor = TextAnchor.UpperLeft;
            Rect dropdownButtonRect = dropdownRect.RightPart(0.6f);
            
            DiceThemeDef currentSelectedTheme = DefDatabase<DiceThemeDef>.GetNamedSilentFail(Settings.selectedDiceThemeDefName);
            string currentThemeLabel = currentSelectedTheme?.LabelCap ?? "None".Translate();

            if (Widgets.ButtonText(dropdownButtonRect, currentThemeLabel))
            {
                List<FloatMenuOption> options = new List<FloatMenuOption>();
                foreach (DiceThemeDef themeDef in DefDatabase<DiceThemeDef>.AllDefsListForReading)
                {
                    options.Add(new FloatMenuOption(themeDef.LabelCap, delegate
                    {
                        Settings.selectedDiceThemeDefName = themeDef.defName;
                    }));
                }
                Find.WindowStack.Add(new FloatMenu(options));
            }

            // Draw Dice Preview
            DiceThemeDef previewTheme = DefDatabase<DiceThemeDef>.GetNamedSilentFail(Settings.selectedDiceThemeDefName);
            if (previewTheme != null)
            {
                DicePreviewUtility.DrawDiePreview(previewRect, previewTheme, "20", 1f);

                Rect themeInfoRect = new Rect(viewerRect.x, viewerRect.y + 110f, viewerRect.width, viewerRect.height - 110f);
                Text.Anchor = TextAnchor.UpperLeft;
                GameFont oldFont = Text.Font;
                Text.Font = GameFont.Small;
                Widgets.Label(themeInfoRect, previewTheme.LabelCap + "\n\n" + previewTheme.description);
                Text.Font = oldFont;
            }
            else
            {
                // Fallback if selectedTheme is null or not found
                Widgets.DrawBoxSolidWithOutline(previewRect, Color.gray, Color.white);
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(previewRect, "SMR_NoDiceSelected".Translate());
            }
            Text.Anchor = TextAnchor.UpperLeft; // Reset anchor

            listing.GapLine();

            listing.Gap(8f);
            listing.CheckboxLabeled("Only roll 1s or 20s", ref Settings.onlyRoll1sOr20s,
                "If enabled, dice rolls will only result in 1s or 20s. For debug/fun.");
            
            listing.Gap(8f);
            if (listing.ButtonText("Reset settings"))
            {
                ShinyMathRocksMod.Settings.showRollWindow = true;
                ShinyMathRocksMod.Settings.moodScale = 1f;
                ShinyMathRocksMod.Settings.rollsRequired = 1;
                ShinyMathRocksMod.Settings.buffValue = 0.005f;
                ShinyMathRocksMod.Settings.diceWindowPosition = new Vector2(-1f, -1f);
                ShinyMathRocksMod.Settings.diceWindowSize = new Vector2(460f, 350f);
                ShinyMathRocksMod.Settings.selectedDiceThemeDefName = "SMR_TranslucentSapphireD20";
            }
            listing.End();
        }
    }
}
