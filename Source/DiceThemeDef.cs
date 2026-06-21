using System.Collections.Generic;
using Verse;
using UnityEngine; // Added for Color

namespace ShinyMathRocks
{
    public class DiceThemeDef : Def
    {
        public string texPath;
        public Color? fontColor; // New field for font color

        public override IEnumerable<string> ConfigErrors()
        {
            foreach (string error in base.ConfigErrors())
            {
                yield return error;
            }

            if (texPath.NullOrEmpty())
            {
                yield return "texPath is required.";
            }
        }
    }
}
