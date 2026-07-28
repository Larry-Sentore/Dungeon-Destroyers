# Dungeon Destroyer — Arena Defender

A 2D top-down survival game built with C# and MonoGame. Survive endless waves of
enemies, collect power-ups, and rack up the highest score you can before your
lives run out — or survive three minutes to win outright.

This project was built for the MonoGame module capstone assignment and
demonstrates object-oriented design, SOLID principles, and the mathematics of
game development (distance, vectors, dot product, cross product and linear
interpolation) in a fully playable game.

## Description

You control a lone defender pinned in a fixed arena. Enemies spawn from
outside the screen edges and close in from every direction; three enemy
archetypes behave differently:

- **Standard** — balanced speed, health and damage.
- **Fast** — quick and dangerous up close, but dies in a couple of hits.
- **Tank** — slow, but soaks up damage and hits hard on contact.

Enemies only chase you once you're inside their detection radius *and* their
forward field of view (a dot-product check) — sneak past their flanks and
they'll keep patrolling. While hunting, they turn to face you using a
cross-product-driven rotation.

Difficulty ramps up continuously the longer you survive: enemies spawn more
often and tougher archetypes become more common. Four power-ups spawn
periodically around the arena: **Health**, **Speed**, **Shield** and **Rapid
Fire**. Walking into one collects it — and so does shooting one, if you'd
rather snipe it from a distance.

Score comes from three sources: killing enemies (tougher kills are worth
more), collecting power-ups, and simply surviving each second.

## How to Play

| Action | Input |
|---|---|
| Move | `W`/`A`/`S`/`D` or Arrow Keys |
| Shoot (aims at your cursor) | Left Mouse Button or `Space` |
| Pause / Quit | `Esc` |

Click **PLAY** on the title screen to begin. If you die and still have lives
left, you respawn at full health with a moment of invulnerability. Run out of
lives and it's **Game Over**; survive 3 minutes and you get **Victory**. Either
way, click **PLAY AGAIN** to start a fresh run.

## How to Run

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- MonoGame's content tooling is restored automatically via NuGet
  (`MonoGame.Content.Builder.Task`) — no separate MGCB install is required.

### Run the game

```bash
cd "Dungeon Destroyer/Dungeon Destroyer"
dotnet run
```

Or open `Dungeon Destroyer/Dungeon Destroyer.slnx` in Visual Studio 2022 (or
Rider / VS Code) and run the `Dungeon Destroyer` project.

### Run the unit tests

```bash
cd "Dungeon Destroyer/Dungeon Destroyer.Tests"
dotnet test
```

or `dotnet test` from the solution folder to run every test project.

## Project Structure

```
Dungeon Destroyer/
├── Dungeon Destroyer.slnx              Solution file (game + tests)
├── Dungeon Destroyer/                  Game project
│   ├── Game1.cs                        Entry point / state machine / draw loop
│   ├── Core/                           Constants and engine-agnostic math helpers
│   ├── Interfaces/                     IDamageable, IMovable, ICollidable
│   ├── Entities/                       Entity base, Player, Projectile
│   │   ├── Enemies/                    Enemy base + Standard/Fast/Tank
│   │   └── PowerUps/                   PowerUp base + Health/Speed/Shield/RapidFire
│   ├── Systems/                        Spawning, collision, combat, scoring, difficulty
│   ├── UI/                             Start screen, HUD, Game Over/Victory screen
│   └── Content/                        Content.mgcb + DefaultFont.spritefont
└── Dungeon Destroyer.Tests/            NUnit test project (40+ tests)
```

## Where the Required Mathematics Live

| Concept | Where |
|---|---|
| Distance | `MathUtils.Distance`, `CollisionSystem.CirclesIntersect` (circle-circle overlap), `Enemy` detection radius check |
| Direction & Vectors | `Player.HandleMovementInput`, `Enemy` chase direction, `Projectile` travel direction |
| Algebra | `DifficultyManager` (spawn interval / stat scaling formulas), `ScoreSystem`, damage/heal clamping in `Player`/`Enemy` |
| Dot Product | `MathUtils.IsWithinFieldOfView` — gates whether an enemy notices the player |
| Cross Product | `MathUtils.TurnDirection` / `RotateTowardsUsingCross` — decides which way an enemy turns to face the player |
| Lerp (4 uses) | Player velocity smoothing, HUD health-bar animation, screen fade-in transition, power-up glow colour pulse |

## Software Design Notes

- **Entity** is an abstract base (position, collision radius, active flag)
  shared by `Player`, `Enemy`, `Projectile` and `PowerUp` — new entity types
  extend it without touching existing code (Open/Closed Principle).
- **IDamageable / IMovable / ICollidable** decouple systems from concrete
  types: `CollisionSystem` only ever talks to `ICollidable`, never to `Player`
  or `Enemy` directly (Dependency Inversion).
- **Systems are single-purpose**: `CollisionSystem` only detects overlaps,
  `CombatSystem` only decides what happens when they occur, `ScoreSystem` only
  tracks points, `DifficultyManager` only tracks the ramp. Each can be tested
  and reasoned about independently (Single Responsibility).
- **Player has no dependency on Keyboard/Mouse.** `Game1` reads input and
  passes plain `Vector2`/`GameTime` values into `Player`, `Enemy`, etc. This
  is what makes the game logic unit-testable without a running window or
  GraphicsDevice.

## Testing Strategy

The `Dungeon Destroyer.Tests` project (NUnit) contains 40+ tests covering:

- Pure math (`MathUtils`): distance, dot product, cross product, lerp, field-of-view and turn-direction checks.
- Collision detection (`CollisionSystem`): overlapping, separated and exactly-touching circles.
- Scoring (`ScoreSystem`): kill/power-up/survival-time scoring and input validation.
- Difficulty scaling (`DifficultyManager`): the algebraic spawn-interval and stat-scaling formulas, including their floors/caps.
- Player logic (`Player`): damage, healing, shield blocking, life/respawn handling, movement, and each timed power-up effect.
- Enemy logic (`Enemy`/archetypes): damage/death, and the distinct speed/health/damage profiles of each archetype.
- Power-ups: each concrete power-up's effect on the player.
- Spawner logic (`EnemySpawner`): spawn timing and that spawn positions stay outside the arena.

Tests avoid anything requiring a `GraphicsDevice`, window, or live keyboard —
they exercise plain C# objects and `Vector2`/`GameTime` values so they run
the same in CI as on a developer machine.

Run them with `dotnet test` from the `Dungeon Destroyer.Tests` folder.
