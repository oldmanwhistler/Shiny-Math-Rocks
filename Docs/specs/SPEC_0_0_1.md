# RimWorld Mod Specification: Shiny Math Rocks

## 1. Overview
**Mod Name:** Shiny Math Rocks
**Description:** Introduces collectible D20 dice to RimWorld that function mechanically as a safe, non-addictive "drug." Pawns consume (roll) the dice to receive a randomized mood modifier, accompanied by a Baldur's Gate 3-style UI overlay showing the dice roll. 

## 2. Item Definition (`ThingDef`)
The core item is the "Shiny Math Rock."
*   **Item Category:** Drug / Social
*   **Properties:** 
    *   100% safe.
    *   0% addiction chance.
    *   No overdose risk.
    *   No toxicity or health detriments.
*   **Market Value:** Low-to-moderate (to allow stockpiling).
*   **Stack Limit:** 75 or 400 (standard for small items).

## 3. Pawn Interaction & Animation
When a pawn is directed to "Ingest" (Roll) the Shiny Math Rock, the standard ingestion sequence is intercepted and modified.
*   **Job Def:** Custom job `JobDefOf.RollDice`.
*   **Animation:** The pawn performs an interaction animation (similar to tossing an item or playing horseshoes) rather than eating or drinking. 
*   **Sound Effect:** A physical dice-clattering sound effect plays in the world.

## 4. UI & Visual Effects (The BG3 Roll)
Upon completing the roll action, a UI overlay triggers on the player's screen, centered or slightly offset to not obscure the pawn.
*   **Visual Sequence:**
    1.  A D20 graphic appears on the screen.
    2.  Numbers rapidly cycle (1-20) for 1.5 - 2 seconds to build anticipation.
    3.  The cycling slows down and stops on the final rolled number.
    4.  The UI briefly flashes (color-coded: red for low rolls, gold/green for high rolls) and fades out.
*   **Audio:** Tick-tick-tick sound during the number cycling, followed by a conclusive "ding" or "thud" on the final result.

## 5. Extensibility & Texture Randomization
To make adding new dice skins easy for other modders (or the player), the mod will use a custom `Def` or a designated texture folder.

*   **Texture Randomizer:** When the UI overlay is called, the C# code selects a random loaded dice texture to display.
*   **How to add new textures:**
    *   **Def-based:** Create a custom XML Def called `DiceThemeDef`. Modders can simply write an XML patch pointing to their custom texture paths (`<UIBackground>Textures/UI/Dice/MyCoolDice</UIBackground>`).

## 6. Gameplay Effects (Mood Modifier)
The final number rolled determines the mood impact on the pawn. The mod uses a custom `ThoughtDef` with varying stages, applied immediately after the UI animation finishes.

| Dice Roll | Mood Result | Thought Def Label (Example) |
| :--- | :--- | :--- |
| **1 (Critical Miss)** | -5 | Rolled a Nat 1 |
| **2 - 5** | -2 | Terrible Roll |
| **6 - 10** | +2 | Mediocre Roll |
| **11 - 15** | +5 | Decent Roll |
| **16 - 19** | +10 | Great Roll |
| **20 (Critical Hit)** | +15 | Rolled a Nat 20! |

> **Note:** The mood buff/debuff should have a moderate duration (e.g., 0.5 to 1 in-game day). The thought should be stackable up to a limit (e.g., max 3 stacks) to prevent players from spamming dice to easily max out a pawn's mood.

## 7. Implementation Roadmap (Technical Requirements)
*   **XML:** Define the `ThingDef` (the item), `JobDef` (rolling), `ThoughtDef` (the mood stages), and `RecipeDef` (if craftable).
*   **C# Assembly (Harmony Patches & Custom Classes):**
    *   `IngestionOutcomeDoer_RollDice`: Custom class to handle the logic when the item is used.
    *   `Window_DiceRoll`: A custom GUI `Window` class to draw the Baldur's Gate 3-style animation on the screen.
    *   `MathRocksSettings`: Optional mod settings to toggle the UI popup (for players who just want the stats without the pause/animation) or adjust mood scaling.