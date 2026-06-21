using RimWorld;
using UnityEngine;
using Verse;

namespace ShinyMathRocks
{
    public static class DicePreviewUtility
    {
        public static void DrawDiePreview(Rect rect, DiceThemeDef themeDef, string numberText, float alpha)
        {
            if (themeDef == null) return;

            Texture2D diceTexture = ContentFinder<Texture2D>.Get(themeDef.texPath, reportFailure: false);
            Color numberColor = themeDef.fontColor ?? Color.white; // Default to white if not specified

            Color oldColor = GUI.color;
            TextAnchor oldAnchor = Text.Anchor;
            GameFont oldFont = Text.Font;

            // Draw dice texture
            GUI.color = new Color(oldColor.r, oldColor.g, oldColor.b, alpha);
            if (diceTexture != null)
            {
                GUI.DrawTexture(rect, diceTexture, ScaleMode.ScaleToFit, true);
            }
            else
            {
                Widgets.DrawBoxSolid(rect.ContractedBy(rect.width * 0.1f), new Color(0.15f, 0.28f, 0.75f, alpha));
            }

            // Draw number
            Text.Anchor = TextAnchor.MiddleCenter;
            Text.Font = GameFont.Medium; // Use Medium for settings preview
            GUI.color = new Color(0f, 0f, 0f, 0.70f * alpha); // Shadow
            Widgets.Label(new Rect(rect.x + 2f, rect.y + rect.height * 0.35f + 3f, rect.width, rect.height * 0.3f), numberText);
            GUI.color = new Color(numberColor.r, numberColor.g, numberColor.b, alpha); // Main number color
            Widgets.Label(new Rect(rect.x, rect.y + rect.height * 0.35f, rect.width, rect.height * 0.3f), numberText);

            GUI.color = oldColor;
            Text.Anchor = oldAnchor;
            Text.Font = oldFont;
        }
    }
}
