# AI Usage Disclosure

## Tools Used

- **Claude Sonnet 4.6** (Anthropic) — chat-based code suggestions and architecture planning

## How I Used Each Tool

### Claude Sonnet 4.6
- **Architecture planning**: Discussed overall file structure, which C# features to demonstrate where, and how to fit the game into the TheAdventure SDL2 skeleton.
- **Code suggestions**: Used for initial drafts of several files, which I then read, understood, and modified.
- **Rubber-ducking**: Asked questions about SDL2 rendering, C# records vs classes, and LINQ expressions.

## Fully AI-Generated Regions

| File | Region | Notes |
|------|--------|-------|
| `UI/GameRenderer.cs` | `BuildFont()` method (pixel bitmap table) | The 5×7 dot-matrix font bitmap dictionary. Marked with `// AI-generated` / `// end AI-generated` inline. |

All other code was written by me or substantially modified from AI suggestions until I understood and owned each line.

## Authorship Estimate

Approximately 30–35% of committed C# source lines are AI-generated verbatim (primarily the font table). All game logic, state machine, SDL2 integration, and persistence code was written by me with AI assistance for reference and review.
