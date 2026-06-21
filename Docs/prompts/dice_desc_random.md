# Dice Randomizer

Randomize a dice description based on the rules in this JSON.

Need a `long_name` (max 5 words) `short_Name` (max two words, camel-case, no spaces) and `visual_desc` in one sentence.

```json
{
  "$schema": "https://json-schema.org/draft/2020-12/schema",
  "title": "D20 Dice Generation Constraints",
  "description": "Defines the logical constraints, balance rules, and generation logic for randomizing a D20 design.",
  "generation_rules": {
    "allow_empty_inclusions": true,
    "max_inclusion_count": 2,
    "force_high_contrast_inking": true
  },
  "constraints": {
    "base_resin": {
      "opacity_types": ["transparent", "translucent", "opaque", "split_half_and_half"],
      "color_limits": {
        "min_colors": 1,
        "max_colors": 3,
        "note": "Exceeding 3 colors risks muddying the resin mix during curing."
      }
    },
    "inclusions": {
      "types": ["miniature_object", "dried_flora", "foil_flakes", "glitter_dust", "liquid_core"],
      "clash_rules": [
        {
          "condition": "if base_resin.opacity_type == 'opaque'",
          "restriction": "exclude all inclusions except foil_flakes on surface",
          "reason": "Internal objects are completely invisible inside opaque resin."
        },
        {
          "condition": "if inclusion.type == 'liquid_core'",
          "restriction": "max_inclusion_count = 1",
          "reason": "A liquid core center takes up the entire internal cavity of the D20."
        }
      ],
      "balance_weight_warnings": [
        "miniature_object"
      ]
    },
    "number_inking": {
        "ignore": true,
        "reason": "The prompt explicitly states to not show any numbers on the dice, so inking constraints are irrelevant."
    }
  }
}
```


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