using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ShinyMathRocks
{
    public class Window_DiceRoll : Window
    {
        private const float SpinDuration = 1.2f;
        private const float HoldDuration = 0.8f;
        private const float FadeDuration = 0.45f;

        private readonly string pawnLabel;
        private readonly int finalRoll;
        private readonly int stage;
        private readonly float startTime;
        private readonly int seed;
        private float lastTickSoundTime = -999f;
        private bool finalSoundPlayed;
        private Texture2D diceTexture;
        private string themeDefNameUsed; // Store the theme defName that was actually used

        public override Vector2 InitialSize => new Vector2(460f, 420f);
        protected override float Margin => 0f;

        public Window_DiceRoll(string pawnLabel, int finalRoll, int stage, DiceThemeDef diceThemeDef)
        {
            this.pawnLabel = pawnLabel;
            this.finalRoll = finalRoll;
            this.stage = stage;
            startTime = Time.realtimeSinceStartup;
            seed = Rand.Range(0, 9999);

            doWindowBackground = false;
            drawShadow = false;
            absorbInputAroundWindow = false;
            closeOnAccept = false;
            closeOnCancel = false;
            closeOnClickedOutside = false;
            preventCameraMotion = false;
            onlyOneOfTypeAllowed = false;
            focusWhenOpened = false;
            soundAppear = null;
            soundClose = null;
            draggable = true; // Make window draggable
            doCloseX = true; // Add close button

            // Attempt to load the texture for the specific theme passed to the constructor
            if (diceThemeDef != null && !diceThemeDef.texPath.NullOrEmpty())
            {
                Texture2D tex = ContentFinder<Texture2D>.Get(diceThemeDef.texPath, reportFailure: false);
                if (tex != null)
                {
                    diceTexture = tex;
                    themeDefNameUsed = diceThemeDef.defName;
                }
            }
            // Fallback to random if no specific theme was provided or failed to load
            if (diceTexture == null)
            {
                List<DiceThemeDef> defs = DefDatabase<DiceThemeDef>.AllDefsListForReading;
                if (defs != null && defs.Count > 0)
                {
                    defs.Shuffle(); // Randomize selection
                    foreach (var def in defs)
                    {
                        if (def != null && !def.texPath.NullOrEmpty())
                        {
                            Texture2D tex = ContentFinder<Texture2D>.Get(def.texPath, reportFailure: false);
                            if (tex != null)
                            {
                                diceTexture = tex;
                                themeDefNameUsed = def.defName;
                                break;
                            }
                        }
                    }
                }
                // Final fallback to default if no valid themes found
                if (diceTexture == null)
                {
Texture2D tex = ContentFinder<Texture2D>.Get("UI/Dice/ShinyD20", reportFailure: false);
                    if (tex != null)
                    {
                        diceTexture = tex;
                        themeDefNameUsed = "SMR_DefaultBlueD20";
                    }
                }
            }
        }

        public override void PostClose()
        {
            base.PostClose();
            // Save window position and size
            ShinyMathRocksMod.Settings.diceWindowPosition = windowRect.position;
            ShinyMathRocksMod.Settings.diceWindowSize = windowRect.size;
        }

        protected override void SetInitialSizeAndPosition()
        {
            if (ShinyMathRocksMod.Settings.diceWindowPosition.x >= 0)
            {
                // Use saved position and size
                windowRect = new Rect(ShinyMathRocksMod.Settings.diceWindowPosition, ShinyMathRocksMod.Settings.diceWindowSize);
            }
            else
            {
                // Default centering if not saved
                base.SetInitialSizeAndPosition();
            }
            // Ensure window is fully on screen
            windowRect = windowRect.Rounded();
        }

        public override void DoWindowContents(Rect inRect)
        {
            float elapsed = Time.realtimeSinceStartup - startTime;
            float total = SpinDuration + HoldDuration + FadeDuration;
            if (elapsed >= total)
            {
                Close(doCloseSound: false);
                return;
            }

            int shownRoll = CurrentShownRoll(elapsed);
            if (elapsed < SpinDuration)
            {
                TryPlayTick(elapsed);
            }
            else if (!finalSoundPlayed)
            {
                finalSoundPlayed = true;
                (finalRoll >= 16 ? SoundDefOf.Tick_High : finalRoll <= 5 ? SoundDefOf.Tick_Low : SoundDefOf.Tick_Tiny).PlayOneShotOnCamera();
            }

            float fade = elapsed > SpinDuration + HoldDuration ? 1f - ((elapsed - SpinDuration - HoldDuration) / FadeDuration) : 1f;
            fade = Mathf.Clamp01(fade);
            float flash = elapsed > SpinDuration && elapsed < SpinDuration + 0.35f ? 1f - ((elapsed - SpinDuration) / 0.35f) : 0f;

            Color oldColor = GUI.color;
            TextAnchor oldAnchor = Text.Anchor;
            GameFont oldFont = Text.Font;

            GUI.color = new Color(0f, 0f, 0f, 0.62f * fade);
            GUI.DrawTexture(inRect, BaseContent.WhiteTex);

            Rect diceRect = new Rect(inRect.center.x - 118f, 52f, 236f, 236f);
            Color resultColor = ResultColor();
            Color tint = Color.Lerp(Color.white, resultColor, flash * 0.45f);
            tint.a = fade;
            GUI.color = tint;
            if (diceTexture != null)
            {
                GUI.DrawTexture(diceRect, diceTexture, ScaleMode.ScaleToFit, true);
            }
            else
            {
                Widgets.DrawBoxSolid(diceRect.ContractedBy(18f), new Color(0.15f, 0.28f, 0.75f, 0.85f * fade));
            }

            Text.Anchor = TextAnchor.MiddleCenter;
            Text.Font = GameFont.Medium;
            GUI.color = new Color(0f, 0f, 0f, 0.70f * fade);
            Widgets.Label(new Rect(diceRect.x + 2f, diceRect.y + 60f + 3f, diceRect.width, 116f), shownRoll.ToString());
            GUI.color = new Color(1f, 0.96f, 0.78f, fade);
            Widgets.Label(new Rect(diceRect.x, diceRect.y + 60f, diceRect.width, 116f), shownRoll.ToString());

            Text.Font = GameFont.Small;
            GUI.color = new Color(1f, 1f, 1f, 0.92f * fade);
            Widgets.Label(new Rect(0f, 18f, inRect.width, 36f), pawnLabel + " rolls a D20...");

            if (elapsed >= SpinDuration)
            {
                GUI.color = new Color(resultColor.r, resultColor.g, resultColor.b, fade);
                Widgets.Label(new Rect(0f, 302f, inRect.width, 36f), ResultLabel());
            }

            GUI.color = oldColor;
            Text.Anchor = oldAnchor;
            Text.Font = oldFont;
        }

        private int CurrentShownRoll(float elapsed)
        {
            if (elapsed >= SpinDuration)
            {
                return finalRoll;
            }

            float t = Mathf.Clamp01(elapsed / SpinDuration);
            float speed = Mathf.Lerp(14f, 8f, t);
            int value = Mathf.FloorToInt((elapsed * speed) + seed) % 20;
            return value + 1;
        }

        private void TryPlayTick(float elapsed)
        {
            float t = Mathf.Clamp01(elapsed / SpinDuration);
            float interval = Mathf.Lerp(0.07f, 0.35f, t);
            if (elapsed - lastTickSoundTime >= interval)
            {
                lastTickSoundTime = elapsed;
                SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
            }
        }

        private Color ResultColor()
        {
            if (finalRoll == 1) return new Color(0.95f, 0.18f, 0.12f);
            if (finalRoll == 20) return new Color(1f, 0.82f, 0.22f);
            if (finalRoll >= 16) return new Color(0.28f, 1f, 0.38f);
            if (finalRoll <= 5) return new Color(1f, 0.43f, 0.20f);
            return new Color(0.75f, 0.87f, 1f);
        }

        private string ResultLabel()
        {
            switch (stage)
            {
                case 0: return "Critical miss — Rolled a Nat 1";
                case 1: return "Terrible roll";
                case 2: return "Mediocre roll";
                case 3: return "Decent roll";
                case 4: return "Great roll";
                default: return "Critical hit — Rolled a Nat 20!";
            }
        }
    }
}
