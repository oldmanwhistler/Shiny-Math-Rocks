using System.Collections.Generic;
using Verse;

namespace ShinyMathRocks
{
    public class DiceThemeDef : Def
    {
        public string texPath;

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
