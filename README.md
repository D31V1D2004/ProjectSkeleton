# Blackjack

A playable Blackjack game built on the TheAdventure skeleton using Silk.NET + SDL2.

## Build & Run

```bash
git clone <your-fork-url>
cd TheAdventure
dotnet run
```

Requires .NET 10 SDK. No other dependencies needed — SDL2 is bundled via NuGet.

## How to Play

| Action | Mouse | Keyboard |
|--------|-------|----------|
| Add $5 chip | Click $5 | Press `1` |
| Add $25 chip | Click $25 | Press `2` |
| Add $100 chip | Click $100 | Press `3` |
| Add $500 chip | Click $500 | Press `4` |
| Deal | Click DEAL | Press `Enter` |
| Hit | Click HIT | Press `H` |
| Stand | Click STAND | Press `S` |
| Double Down | Click DOUBLE | Press `D` |
| Clear bet | Click CLEAR | Press `Esc` |
| Next round | Click NEXT → | Press `Enter` |

## Rules

- Standard Blackjack: beat the dealer without going over 21.
- Blackjack (natural) pays 3:2.
- Dealer hits on soft 17.
- Double down allowed on first two cards only.
- 6-deck shoe, reshuffled when empty.
- You start with $500. High score is saved between sessions.

## Game Description

A single-player Blackjack game rendered in an SDL2 window with a pixel-font UI. 
The player places bets using chip buttons ($5/$25/$100/$500), then plays against a 
dealer following standard casino rules. Balance and high score persist to disk 
between sessions via async JSON I/O.

## Architecture

```
Game/
  Card.cs              — immutable record; pattern-matched values
  Deck.cs              — generic shuffle extension; 6-deck shoe
  Hand.cs              — LINQ scoring; soft/hard distinction
  Player.cs            — balance, betting, custom exception
  BlackjackGame.cs     — state machine (Betting/PlayerTurn/DealerTurn/RoundOver)
  HighScoreService.cs  — async/await JSON persistence; IDisposable
  InvalidBetException.cs — custom exception
UI/
  Button.cs            — hit-test helper
  GameRenderer.cs      — all SDL2 drawing; pixel-font renderer
Program.cs             — SDL event loop; keyboard shortcuts
```

## AI Usage Summary

This project was developed with assistance from Claude (Anthropic). See `AI_USAGE.md` for full details.
The pixel-font bitmap table in `GameRenderer.cs` is AI-generated (marked inline).
All game logic, state machine design, and SDL2 integration were written by the student.
