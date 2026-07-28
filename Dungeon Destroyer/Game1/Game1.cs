using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;

namespace Game1
{
    /// <summary>
    /// Main game class. Owns the player ("warrior": WASD movement, run/shoot
    /// animation, projectiles), a couple of decorative/pickup sprites, and a
    /// small HUD (hearts + score).
    /// </summary>
    public class Game1 : Core
    {
        // ------------------------------------------------------------------
        // Textures & fonts
        // ------------------------------------------------------------------
        private Texture2D warrior;
        private Texture2D pumpkinDude;
        private Texture2D dungeonTileset;
        private Texture2D pixel; // 1x1 white texture, tinted/scaled to draw bullets.
        private SpriteFont font;

        // ------------------------------------------------------------------
        // Warrior sprite sheet layout (players_blue_x1.png: 8 cols x 13 rows, 32x32 px/frame).
        // Only the rows/columns below actually contain art; the rest of the sheet is blank.
        // ------------------------------------------------------------------
        private const int WarriorFrameSize = 32;
        private const float WarriorScale = 3f;
        private const int WarriorIdleRow = 0;   // 1 frame
        private const int WarriorRunRow = 3;    // 4 frames
        private const int WarriorShootRow = 4;  // 4 frames
        private const int WarriorRunFrameCount = 4;
        private const int WarriorShootFrameCount = 4;

        // pumpkin_dude.png is a sprite sheet too; we only ever show its first frame.
        private static readonly Rectangle PumpkinDudeFrame = new Rectangle(0, 0, 16, 32);
        private const float PumpkinDudeScale = 3f;
        private const float SmallEnemyScale = 2f;
        private const int EnemyRunFrameCount = 8;

        // 0x72_DungeonTilesetII_v1.7.png UI icons (16x16 each), coordinates found by
        // inspecting the sheet on a 16px grid.
        private static readonly Rectangle HeartFullFrame = new Rectangle(288, 368, 16, 16);
        private static readonly Rectangle HeartEmptyFrame = new Rectangle(320, 368, 16, 16);
        private static readonly Rectangle HealthPotionFrame = new Rectangle(288, 336, 16, 16);
        private static readonly Rectangle YellowPotionFrame = new Rectangle(336, 336, 16, 16);
        private const float PotionScale = 3f;

        /// <summary>
        /// A world pickup. Position and source frame live here so drawing and collision
        /// always agree, instead of repeating the same coordinates in two places.
        /// </summary>
        private class Potion
        {
            public Vector2 Position;
            public Rectangle SourceFrame;
            public int HealAmount;
            public int ScoreValue;
            public bool Collected;

            /// <summary>Screen-space bounds of this potion, used for pickup collision.</summary>
            public Rectangle Bounds => new Rectangle(
                (int)Position.X,
                (int)Position.Y,
                (int)(SourceFrame.Width * PotionScale),
                (int)(SourceFrame.Height * PotionScale));
        }

        private readonly List<Potion> potions = new List<Potion>
        {
            new Potion { Position = new Vector2(500, 300), SourceFrame = HealthPotionFrame, HealAmount = 1 },
            new Potion { Position = new Vector2(560, 300), SourceFrame = YellowPotionFrame, ScoreValue = 100 },
        };

        // ------------------------------------------------------------------
        // Warrior movement & facing
        // ------------------------------------------------------------------
        private Vector2 warriorPosition = new Vector2(300, 100);
        private const float WarriorSpeed = 200f; // pixels per second
        private bool isMoving;
        private bool facingLeft;

        // ------------------------------------------------------------------
        // Warrior animation state machine (Idle / Run / Shoot)
        // ------------------------------------------------------------------
        private enum PlayerAnimState { Idle, Run, Shoot }
        private PlayerAnimState animState = PlayerAnimState.Idle;
        private int animFrame;
        private float animTimer;
        private const float AnimFrameDuration = 0.12f; // seconds each frame is shown

        // ------------------------------------------------------------------
        // Shooting
        // ------------------------------------------------------------------
        private const float BulletSpeed = 600f;   // pixels per second
        private const float BulletSize = 8f;      // rendered width/height in pixels
        private const float FireCooldown = 0.2f;  // minimum seconds between shots
        private const float ShootAnimDuration = WarriorShootFrameCount * AnimFrameDuration;
        private readonly List<Bullet> bullets = new List<Bullet>();
        private KeyboardState previousKeyboard;
        private float fireCooldownTimer;
        private float shootTimer; // counts down while the shoot animation is playing

        // ------------------------------------------------------------------
        // Player health & score
        // ------------------------------------------------------------------
        private const int MaxHealth = 10;
        private int currentHealth = MaxHealth;
        private int score = 0;

        // ------------------------------------------------------------------
        // Enemy health and speed
        // ------------------------------------------------------------------
        private const float SmallEnemySpeed = 250f;
        private const int SmallEnemyHealth = 3;
        private const float BigEnemySpeed = 210f;
        private const int BigEnemyHealth = 5;


        /// <summary>A single fired projectile. Travels in a straight line until it leaves the screen.</summary>
        private class Bullet
        {
            public Vector2 Position;
            public readonly Vector2 Velocity;

            public Bullet(Vector2 position, Vector2 velocity)
            {
                Position = position;
                Velocity = velocity;
            }
        }

        public Game1() : base("Dungeon", 1280, 720, false)
        {
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            warrior = Core.Content.Load<Texture2D>("Sprites/players_blue_x1[1]");
            pumpkinDude = Core.Content.Load<Texture2D>("Sprites/pumpkin_dude");
            dungeonTileset = Core.Content.Load<Texture2D>("Sprites/0x72_DungeonTilesetII_v1.7[1]");
            font = Core.Content.Load<SpriteFont>("Fonts/DefaultFont");

            // A 1x1 white pixel, tinted and scaled at draw time, so bullets don't need a dedicated sprite asset.
            //
            // NOTE: Core hides Game.GraphicsDevice with a static property that it only
            // assigns *after* base.Initialize() - and base.Initialize() is what calls
            // LoadContent() - so the static is still null here. Go through Game's real
            // instance property instead.
            pixel = new Texture2D(((Game)this).GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Escape))
                Exit();

            UpdateMovement(keyboard, deltaTime);
            CheckPotionPickup();
            UpdateShooting(keyboard, deltaTime);
            UpdateBullets(deltaTime);
            UpdateAnimation(deltaTime);
            

            previousKeyboard = keyboard;
            base.Update(gameTime);
        }

        /// <summary>Reads WASD input, moves the warrior, and tracks left/right facing.</summary>
        private void UpdateMovement(KeyboardState keyboard, float deltaTime)
        {
            Vector2 direction = Vector2.Zero;

            if (keyboard.IsKeyDown(Keys.W)) direction.Y -= 1;
            if (keyboard.IsKeyDown(Keys.S)) direction.Y += 1;
            if (keyboard.IsKeyDown(Keys.A)) direction.X -= 1;
            if (keyboard.IsKeyDown(Keys.D)) direction.X += 1;

            isMoving = direction != Vector2.Zero;
            if (!isMoving)
                return;

            // Normalize so diagonal movement isn't faster than cardinal movement.
            direction.Normalize();
            warriorPosition += direction * WarriorSpeed * deltaTime;

            // Only flip facing on horizontal input; ignore vertical-only movement so
            // the sprite doesn't snap back to "facing right" while moving up/down.
            if (direction.X < 0) facingLeft = true;
            else if (direction.X > 0) facingLeft = false;
        }

        /// <summary>Handles the fire button (Space), its cooldown, and spawning bullets.</summary>
        private void UpdateShooting(KeyboardState keyboard, float deltaTime)
        {
            fireCooldownTimer -= deltaTime;
            if (shootTimer > 0f)
                shootTimer -= deltaTime;

            // Edge-triggered: only fires the instant Space is pressed, not every frame
            // it's held, so holding the key down doesn't machine-gun bullets.
            bool firePressed = keyboard.IsKeyDown(Keys.P) && !previousKeyboard.IsKeyDown(Keys.P);
            if (!firePressed || fireCooldownTimer > 0f)
                return;

            FireBullet();
            fireCooldownTimer = FireCooldown;
            shootTimer = ShootAnimDuration;
        }

        /// <summary>Spawns a bullet at the warrior's center, travelling in the facing direction.</summary>
        private void FireBullet()
        {
            Vector2 center = warriorPosition + new Vector2(WarriorFrameSize, WarriorFrameSize) * WarriorScale / 2f;
            Vector2 velocity = new Vector2(facingLeft ? -BulletSpeed : BulletSpeed, 0f);
            bullets.Add(new Bullet(center, velocity));
        }

        /// <summary>Moves active bullets and discards any that have left the screen.</summary>
        private void UpdateBullets(float deltaTime)
        {
            Viewport viewport = GraphicsDevice.Viewport;

            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                bullets[i].Position += bullets[i].Velocity * deltaTime;

                if (bullets[i].Position.X < 0 || bullets[i].Position.X > viewport.Width)
                    bullets.RemoveAt(i);
            }
        }

        /// <summary>
        /// Picks the current animation state (Shoot takes priority over Run/Idle) and
        /// advances the frame timer.
        /// </summary>
        private void UpdateAnimation(float deltaTime)
        {
            PlayerAnimState newState =
                shootTimer > 0f ? PlayerAnimState.Shoot :
                isMoving ? PlayerAnimState.Run :
                PlayerAnimState.Idle;

            // Restart the frame counter whenever the state changes so, e.g., every shot
            // plays its animation from frame 0 instead of resuming mid-cycle.
            if (newState != animState)
            {
                animState = newState;
                animFrame = 0;
                animTimer = 0f;
            }

            int frameCount = animState switch
            {
                PlayerAnimState.Run => WarriorRunFrameCount,
                PlayerAnimState.Shoot => WarriorShootFrameCount,
                _ => 1, // Idle only has a single frame in the sheet.
            };

            if (frameCount <= 1)
                return;

            animTimer += deltaTime;
            if (animTimer < AnimFrameDuration)
                return;

            animTimer -= AnimFrameDuration;
            animFrame = (animFrame + 1) % frameCount;
        }

        /// <summary>Maps the current animation state/frame to a rectangle within the warrior sprite sheet.</summary>
        private Rectangle GetWarriorSourceRectangle()
        {
            int row = animState switch
            {
                PlayerAnimState.Run => WarriorRunRow,
                PlayerAnimState.Shoot => WarriorShootRow,
                _ => WarriorIdleRow,
            };

            return new Rectangle(animFrame * WarriorFrameSize, row * WarriorFrameSize, WarriorFrameSize, WarriorFrameSize);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Purple);

            Core.SpriteBatch.Begin();

            Core.SpriteBatch.Draw(
                pumpkinDude,
                new Vector2(100, 100),
                PumpkinDudeFrame,
                Color.White,
                0f,
                Vector2.Zero,
                PumpkinDudeScale,
                SpriteEffects.None,
                0f);

            Core.SpriteBatch.Draw(
                pumpkinDude,
                new Vector2(200, 100),
                PumpkinDudeFrame,
                Color.White,
                0f,
                Vector2.Zero,
                SmallEnemyScale,
                SpriteEffects.None,
                0f);

            DrawWarrior();
            DrawBullets();

            DrawPotions();

            DrawHud();

            Core.SpriteBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>
        /// Screen-space hitbox for the warrior. The character art only occupies roughly
        /// the middle third of its 32x32 cell, so the full scaled frame is inset to stop
        /// pickups triggering from a visible distance away.
        /// </summary>
        private Rectangle GetWarriorBounds()
        {
            // Art region within the 32x32 source frame (measured from the sprite sheet).
            const int artOffsetX = 10;
            const int artOffsetY = 8;
            const int artWidth = 12;
            const int artHeight = 20;

            return new Rectangle(
                (int)(warriorPosition.X + artOffsetX * WarriorScale),
                (int)(warriorPosition.Y + artOffsetY * WarriorScale),
                (int)(artWidth * WarriorScale),
                (int)(artHeight * WarriorScale));
        }

        /// <summary> getting bullet hitbox, used for collision detection with enemies or other objects. </summary>
        private Rectangle GetBulletBounds(Bullet bullet)
        {
            return new Rectangle(
                (int)(bullet.Position.X - BulletSize / 2f),
                (int)(bullet.Position.Y - BulletSize / 2f),
                (int)BulletSize,
                (int)BulletSize);
        }

        /// <summary> getting enemy hitbox, used for collision detection with bullets or player. </summary>
        private Rectangle GetEnemyBounds(Vector2 enemyPosition, int enemyWidth, int enemyHeight)
        {
            const int artOffsetX = 0;
            const int artOffsetY = 0;
            const int artWidth = 16;
            const int artHeight = 32;

            return new Rectangle(
                (int)enemyPosition.X,
                (int)enemyPosition.Y,
                enemyWidth,
                enemyHeight);
        }

        

        /// <summary>
        /// Collects any potion the warrior is overlapping: applies its effect, then marks
        /// it collected so it stops being drawn and can't be picked up twice.
        /// </summary>
        private void CheckPotionPickup()
        {
            Rectangle warriorBounds = GetWarriorBounds();

            foreach (Potion potion in potions)
            {
                if (potion.Collected || !warriorBounds.Intersects(potion.Bounds))
                    continue;

                potion.Collected = true;
                currentHealth = Math.Min(currentHealth + potion.HealAmount, MaxHealth);
                score += potion.ScoreValue;
            }
        }

        /// <summary>Draws the warrior using its current animation frame, flipped when facing left.</summary>
        private void DrawWarrior()
        {
            // Origin at the frame's center so flipping mirrors the sprite in place
            // instead of shifting it sideways by a frame width.
            Vector2 origin = new Vector2(WarriorFrameSize / 2f, WarriorFrameSize / 2f);
            Vector2 drawPosition = warriorPosition + origin * WarriorScale;
            SpriteEffects effects = facingLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Core.SpriteBatch.Draw(
                warrior,
                drawPosition,
                GetWarriorSourceRectangle(),
                Color.White,
                0f,
                origin,
                WarriorScale,
                effects,
                0f);
        }

        /// <summary>Draws each potion that hasn't been collected yet.</summary>
        private void DrawPotions()
        {
            foreach (Potion potion in potions)
            {
                if (potion.Collected)
                    continue;

                Core.SpriteBatch.Draw(
                    dungeonTileset,
                    potion.Position,
                    potion.SourceFrame,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    PotionScale,
                    SpriteEffects.None,
                    0f);
            }
        }

        /// <summary>Draws every active bullet as a small tinted square.</summary>
        private void DrawBullets()
        {
            foreach (Bullet bullet in bullets)
            {
                Core.SpriteBatch.Draw(
                    pixel,
                    bullet.Position,
                    null,
                    Color.Gold,
                    0f,
                    new Vector2(0.5f, 0.5f), // center of the 1x1 source texture
                    BulletSize,
                    SpriteEffects.None,
                    0f);
            }
        }


        /// <summary>Draws the health hearts (top-left) and score text (top-right).</summary>
        private void DrawHud()
        {
            const float heartScale = 2.5f;
            const int heartSize = 16;
            const int heartSpacing = 4;
            Vector2 heartPosition = new Vector2(20, 20);

            for (int i = 0; i < MaxHealth; i++)
            {
                Rectangle frame = i < currentHealth ? HeartFullFrame : HeartEmptyFrame;

                Core.SpriteBatch.Draw(
                    dungeonTileset,
                    heartPosition,
                    frame,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    heartScale,
                    SpriteEffects.None,
                    0f);

                heartPosition.X += (heartSize * heartScale) + heartSpacing;
            }

            string scoreText = $"Score: {score}";
            Vector2 textSize = font.MeasureString(scoreText);
            Vector2 scorePosition = new Vector2(GraphicsDevice.Viewport.Width - textSize.X - 20, 20);
            Core.SpriteBatch.DrawString(font, scoreText, scorePosition, Color.White);
        }
    }
}
