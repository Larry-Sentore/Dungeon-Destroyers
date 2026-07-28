using Microsoft.Xna.Framework;

namespace Game1
{
    /// <summary>
    /// Every tuning value and sprite-sheet coordinate in one place, so gameplay can be
    /// rebalanced without hunting through the entity and system classes.
    /// </summary>
    internal static class GameConstants
    {
        // ------------------------------------------------------------------
        // Warrior sprite sheet (players_blue_x1.png: 8 cols x 13 rows, 32x32 px/frame).
        // Only the rows below contain art; the rest of the sheet is blank.
        // ------------------------------------------------------------------
        public const int WarriorFrameSize = 32;
        public const float WarriorScale = 5f;
        public const int WarriorIdleRow = 0;   // 1 frame
        public const int WarriorRunRow = 3;    // 4 frames
        public const int WarriorShootRow = 4;  // 4 frames
        public const int WarriorRunFrameCount = 4;
        public const int WarriorShootFrameCount = 4;

        public const float WarriorSpeed = 200f;         // pixels per second
        public const float AnimFrameDuration = 0.12f;   // seconds each frame is shown
        public const float ShootAnimDuration = WarriorShootFrameCount * AnimFrameDuration;

        // The character art only fills the middle of its 32x32 cell, so the hitbox is
        // inset to stop collisions triggering from a visible distance away.
        public const int WarriorArtOffsetX = 10;
        public const int WarriorArtOffsetY = 8;
        public const int WarriorArtWidth = 12;
        public const int WarriorArtHeight = 20;

        // ------------------------------------------------------------------
        // Enemy sprite sheet (pumpkin_dude.png: a single row of 8 frames, 16x32 px each).
        // ------------------------------------------------------------------
        public const int PumpkinFrameWidth = 16;
        public const int PumpkinFrameHeight = 32;
        public const int PumpkinFrameCount = 8;
        public const float EnemyAnimFrameDuration = 0.15f;

        // ------------------------------------------------------------------
        // Enemy tuning. Small enemies are fast and fragile; big ones are slow,
        // tanky, and hit much harder.
        // ------------------------------------------------------------------
        public const int SmallEnemyHealth = 2;
        public const int BigEnemyHealth = 5;
        public const int SmallEnemyDamage = 1;
        public const int BigEnemyDamage = 3;
        public const float SmallEnemySpeed = 90f;   // pixels per second
        public const float BigEnemySpeed = 55f;
        public const float SmallEnemyScale = 2.5f;
        public const float BigEnemyScale = 4.5f;
        public const int SmallEnemyScore = 10;
        public const int BigEnemyScore = 25;

        public const float EnemySpawnInterval = 2f;  // seconds between spawns

        // ------------------------------------------------------------------
        // Shooting
        // ------------------------------------------------------------------
        public const float BulletSpeed = 600f;   // pixels per second
        public const float BulletSize = 8f;      // rendered width/height in pixels
        public const float FireCooldown = 0.2f;  // minimum seconds between shots
        public const int BulletDamage = 1;       // damage one bullet deals

        // ------------------------------------------------------------------
        // Player health
        // ------------------------------------------------------------------
        public const int MaxHealth = 10;

        // ------------------------------------------------------------------
        // Dungeon tileset UI icons (0x72_DungeonTilesetII_v1.7.png, 16x16 each),
        // coordinates found by inspecting the sheet on a 16px grid.
        // ------------------------------------------------------------------
        public static readonly Rectangle HeartFullFrame = new Rectangle(288, 368, 16, 16);
        public static readonly Rectangle HeartEmptyFrame = new Rectangle(320, 368, 16, 16);
        public static readonly Rectangle HealthPotionFrame = new Rectangle(288, 336, 16, 16);
        public static readonly Rectangle YellowPotionFrame = new Rectangle(336, 336, 16, 16);

        public const float PotionScale = 3f;

        // ------------------------------------------------------------------
        // HUD layout
        // ------------------------------------------------------------------
        public const float HeartScale = 2.5f;
        public const int HeartSize = 16;
        public const int HeartSpacing = 4;
        public const int HudMargin = 20;
    }
}
