# AI Usage Disclosure

## Tools Used

- **Claude Sonnet 4.6** (Anthropic)

## How It Was Used

- *Chat-based code suggestions:* Used to structure the GamePhase state machine for separating the Betting Phase, Player Turn, Dealer Turn, and Round Over states.
- *Logic debugging & Rubber-ducking:* Used to fix specific game logic bugs, such as invalid bet exceptions (InvalidBetException) and implementing correct Ace scoring logic (reducing Ace from 11 to 1 when hand would bust).
- *Mathematical formulas:* Used to generate the ellipse drawing formulas (FillEllipse, DrawEllipse) for rendering the oval felt table using trigonometry.
- *Architecture planning:* Used to discuss overall file structure and which C# features to demonstrate in which files.

## Fully AI-Generated Regions

| File | Region | Notes |
|------|--------|-------|
| `UI/GameRenderer.cs` | `_font` dictionary (pixel bitmap table) | 5x7 dot-matrix font bitmaps for all characters. Marked inline with `// AI-generated`. |
| `UI/GameRenderer.cs` | `FillEllipse()` and `DrawEllipse()` methods | Trigonometric formulas for oval felt table rendering. Marked inline with `// AI-generated`. |
| `Game/BlackjackGame.cs` | `ResolveDealerTurn()` and `DetermineResult()` methods | Dealer draw logic and win/push/bust condition resolution. Marked inline with `// AI-generated`. |
| `Game/HighScoreService.cs` | Entire file | Async/await JSON persistence with error handling. Marked inline with `// AI-generated`. |
| `Game/Deck.cs` | `ShuffleExtensions.Shuffle<T>()` method | Generic Fisher-Yates shuffle algorithm. Marked inline with `// AI-generated`. |

*(Note: Restul logicii de joc, input handling-ul, integrarea SDL2, și asamblarea componentelor au fost scrise și integrate manual, fiind doar asistate punctual de AI.)*

## Authorship Estimate

Approximately 35-40% of committed C# source lines are AI-generated verbatim (primarily the font table, ellipse math, and the two methods listed above). All game state management, SDL2 rendering structure, UI layout, and player interaction logic was written by the student with AI used only as a reference and debugging aid.
