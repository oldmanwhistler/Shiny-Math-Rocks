# Dice Description

Generate a dice description.

Need a `long_name` (max 5 words) `short_Name` (max two words, camel-case, no spaces) and `visual_desc` in one sentence.

## Output Format

Write this XML snippet as an output format.

```xml
  <ShinyMathRocks.DiceThemeDef>
    <defName>SMR_`short_name`D20</defName>
    <label>`long_name`</label>
    <description>`visual_desc`</description>
    <texPath>UI/Dice/`short_name`D20</texPath>
  </ShinyMathRocks.DiceThemeDef>    
```