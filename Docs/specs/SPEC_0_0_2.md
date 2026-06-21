# Technical Specification: Dice Goblin Mechanics Mod Modification

## Document Overview
This specification defines the architectural and implementation requirements for expanding an existing RimWorld dice-rolling mod. The modifications add cross-map UI suppression, persistent statistics tracking via a custom health condition (`Hediff`), and a dynamic, player-configurable stat-buffing system based on roll performance.

* **Namespace:** `ShinyMathRocks`
* **Global Prefix:** `SMR_`

## 2. Persistent Hediff: Dice Goblin

### 2.1 Overview
Every valid dice-roll event must check for the presence of a permanent tracking health condition (`HediffDef`). If absent, the condition is initialized and applied to the pawn immediately. Uses the name SMR_DiceGoblinStatus for the `HediffDef`.

### 2.2 Functional and Data Requirements
The underlying `Hediff` class must reside within the `ShinyMathRocks` namespace, extend the standard RimWorld `Hediff` class, and completely manage its internal tracking state inside an overridden `ExposeData()` method. 

| Section | Spec Requirement   | Technical Property                    | Management Rule                                                                                                    |
| :--------| :-------------------| :--------------------------------------| :-------------------------------------------------------------------------------------------------------------------|
| **2.1** | Critical Trackers  | `int nat20Count`<br>`int nat1Count`   | Increment directly upon evaluating an un-modified roll of 20 or 1.                                                 |
| **2.2** | Volume Tracking    | `int totalRolls`                      | Increment by 1 for *every* resolved roll event.                                                                    |
| **2.3** | Favorite Die Track | `Dictionary<string, int> themeNat20s` | Key: `DiceThemeDef.defName` (string)<br>Value: Counter for critical successes (Nat 20s) under that specific theme. |

### 2.3 Favorite Dice Evaluation Logic
The favorite die is computed dynamically via a C# property lookup. It parses the tracking dictionary to determine which `DiceThemeDef` possesses the highest concentration of critical successes.

* **Algorithmic Rule:** Iterate through `themeNat20s`. Find the key with the maximum integer value.
* **Tie-Breaker:** Fallback to alphabetical sorting of the `defName` string or matching the first entry found.
* **Localization Output:** Returns the localized label string (`themeDef.label`) for rendering in inspect panes. If the collection is empty or values are zero, it defaults to `"None"`.

---

## 3. Dynamic Consciousness Modifier & Configuration

### 3.1 Gameplay Mechanics
A pawn's "luck delta" is calculated as:  

```
Delta = nat20Count - nat1Count
```

If `Delta > 0`, the pawn receives a programmatic increase to their **Consciousness** capacity based on parameters defined in the Mod Settings UI. If `Delta < 0`, then give negative penality. If `delta == 0` then modifier clamps cleanly at `0.0f`.

### 3.2 Mod Settings Configuration UI
Expose the following adjustable fields using the global `SMR_` prefix in the mod's `ModSettings` sub-class:

* **`SMR_rollsRequired` (int):** The net delta required per optimization tier. *Default: 3*
* **`SMR_buffValue` (float):** The absolute consciousness offset added per step. *Default: 0.01 (1%)*

### 3.3 Unified Code Structure (`ShinyMathRocks` Namespace)
Because standard RimWorld XML capacity modifiers (`<capMods>`) are structural and static, the evaluation must occur procedurally by overriding the `CapMods` enumerable property within the custom `Hediff_DiceGoblin` instance:

---

## 4. XML Definition Framework

The tracking `Hediff` explicitly declares its `defName` using the global `SMR_` prefix and points directly to the class structure nested within the `ShinyMathRocks` namespace.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<Defs>
  <HediffDef>
    <defName>SMR_DiceGoblinStatus</defName>
    <hediffClass>ShinyMathRocks.Hediff_DiceGoblin</hediffClass>
    <label>dice goblin</label>
    <description>This pawn is fundamentally obsessed with rolling dice. Their psychological state scales elegantly with their streak of critical successes.</description>
    <defaultLabelColor>(0.7, 0.4, 0.9)</defaultLabelColor>
    <isBad>false</isBad>
    <everCurableByItem>false</everCurableByItem>
    <priceImpact>false</priceImpact>
  </HediffDef>
</Defs>
```