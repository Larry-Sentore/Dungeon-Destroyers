# Dungeon Destroyers

A 2D top-down arcade survival shooter built with C# and MonoGame. You play a
warrior trapped in a dungeon while pumpkin enemies pour in from every edge of
the screen. Shoot them down, grab potions, and score as many points as you can
before your ten hearts run out.

Built for the MonoGame module capstone assignment. It demonstrates
object-oriented design, separation of game logic from rendering, and the
mathematics of game development — distance, vectors, dot product, cross product
and linear interpolation — in a fully playable game backed by unit tests.

## Description

Enemies spawn every two seconds from a random screen edge and hunt you down.
There are two types, and the difference between them is the core of the
difficulty:

| Enemy | Health | Contact damage | Speed | Score |
|---|---|---|---|---|
| **Small** | 2 | 1 heart | 90 px/s | 10 |
| **Big** | 5 | 3 hearts | 55 px/s | 25 |

Big enemies are tougher and hit far harder, but they are slow enough to run
away from. Small ones die quickly but close the distance fast.

Enemies do not home in on you blindly. Each one has a **facing direction** and
only notices you when you are both within its detection radius (400 pixels) and
inside the 90-degree cone in front of it — a dot product check. Once alerted, it
speeds up and charges. Because it turns using a cross product, it rotates
gradually rather than snapping around, so enemies visibly curve toward you and
can be flanked.

Potions drop every five seconds somewhere in the play area and drift toward you
once you are close enough:

- **Red potion** — restores one heart.
- **Yellow potion** — worth 100 points.

Score comes from destroying enemies and collecting yellow potions. When your
health reaches zero the run ends and your final score is shown.

## How to Play

| Action | Input |
|---|---|
| Move | `W` `A` `S` `D` |
| Shoot | Arrow keys — `↑` `↓` `←` `→` |
| Start / Restart | `Enter` |
| Quit | `Esc` |

Movement and shooting are on **separate keys**, so you can retreat in one
direction while firing in another. Holding an arrow key fires continuously at a
fixed rate; you do not need to press it repeatedly.

Press `Enter` on the title screen to begin. Each bullet deals 1 damage, so
small enemies take two shots and big ones take five. If an enemy touches you it
damages you, then goes on a one-second cooldown before it can hit you again.

When your hearts run out you get the **Game Over** screen with your final
score, and the game world stays visible behind it. Press `Enter` to start a
completely fresh run.

## How to Run

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)

### Run the game

From the solution folder:

```bash
cd "Dungeon Destroyer"
dotnet tool restore
dotnet run --project Game1
```

`dotnet tool restore` installs the MonoGame Content Builder (`dotnet-mgcb`),
which compiles the sprite sheets and font into the format the game loads at
runtime. It is defined in `.config/dotnet-tools.json` and only needs running
once per clone — but if you skip it, the build fails with `MSB3073` because
`mgcb` cannot be found.

Alternatively, open `Dungeon Destroyer/Game1.slnx` in Visual Studio 2022, Rider
or VS Code and run the `Game1` project.

### Run the unit tests

```bash
cd "Dungeon Destroyer"
dotnet test Game1.slnx
```

All 10 tests should pass in under a second.

## Project Structure

```
Dungeon Destroyer/
├── Game1.slnx                      Solution: game, library, tests
├── .config/dotnet-tools.json       MGCB content tool manifest
│
├── Game1/                          The game
│   ├── Program.cs                  Entry point
│   ├── Game1.cs                    Wiring: state machine, update and draw order
│   ├── GameConstants.cs            Every tuning value and sprite sheet coordinate
│   ├── Maths/
│   │   └── MathUtils.cs            Distance, direction, dot, cross, lerp
│   ├── Entities/
│   │   ├── Player.cs               Movement, facing, animation, health
│   │   ├── Enemy.cs                Steering, field of view, damage
│   │   ├── EnemyKind.cs            Small / Big
│   │   ├── Bullet.cs               Straight-line projectile
│   │   └── Potion.cs               Pickup effects and magnet behaviour
│   ├── Systems/
│   │   ├── WeaponSystem.cs         Aim input, fire rate, bullet lifetime
│   │   ├── CombatSystem.cs         All damage and scoring resolution
│   │   ├── EnemySpawner.cs         Timed enemy spawning
│   │   └── PotionSpawner.cs        Timed potion spawning
│   ├── UI/
│   │   ├── GameState.cs            Start / Playing / GameOver
│   │   ├── StartScreen.cs          Title screen and controls
│   │   ├── GameOverScreen.cs       Final score and restart prompt
│   │   ├── Hud.cs                  Hearts and score, smoothed
│   │   └── ScreenText.cs           Shared centred text drawing
│   └── Content/                    Content.mgcb, font, 3 sprite sheets
│
├── MonoGameLibrary/                Core base class wrapping MonoGame setup
│   └── core.cs
│
└── Game1.Tests/                    NUnit test project (10 tests)
```


## Testing

The `Game1.Tests` project (NUnit 4.2.2) contains 10 tests covering business
logic and mathematics rather than rendering:

| Test | Area |
|---|---|
| `Distance_ThreeFourFiveTriangle_ReturnsFive` | Distance calculation |
| `IsWithinFieldOfView_TargetBehind_ReturnsFalse` | Dot product |
| `Cross_TargetAnticlockwise_ReturnsNegative` | Cross product |
| `Lerp_AmountAboveOne_IsClampedToEndValue` | Utility class, edge case |
| `TakeDamage_MoreThanRemainingHealth_FloorsAtZero` | Health calculation, edge case |
| `UpdateMovement_DiagonalIsNotFasterThanStraight` | Vector normalisation |
| `Create_BigEnemy_IsTougherAndSlowerThanSmall` | Difficulty scaling |
| `ResolveBulletHits_KillingBlow_RemovesEnemyAndAwardsScore` | Score calculation, collision |
| `ResolveEnemyContact_EnemyOnCooldown_DealsNoDamage` | Collision, cooldown edge case |
| `ResolvePotionPickups_HealthPotion_HealsPlayerAndIsRemoved` | Power-up effect |

Tests never touch a `GraphicsDevice`, window or live keyboard state beyond
`KeyboardState`, so they run identically on any machine.

Writing them found a real bug: when the player stood at exactly 180 degrees
behind an enemy, the cross product returned zero, so the enemy never turned
around and stood facing away permanently. `MathUtils.TurnToward` now defaults
to one turn direction in that case.
