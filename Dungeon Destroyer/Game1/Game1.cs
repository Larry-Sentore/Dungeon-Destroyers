using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using Game1.Entities;
using Game1.Systems;
using Game1.UI;

namespace Game1
{
    /// <summary>
    /// Ties the game together: loads the sprites, holds the entity lists and systems,
    /// and switches between the start screen, the game, and the game over screen.
    /// </summary>
    public class Game1 : Core
    {
        // Textures & fonts
        private Texture2D warriorTexture;
        private Texture2D pumpkinTexture;
        private Texture2D dungeonTileset;
        private Texture2D pixel; // 1x1 white texture, scaled up to draw bullets.
        private SpriteFont font;

        // Entities
        private Player player;
        private readonly List<Enemy> enemies = new List<Enemy>();
        private readonly List<Potion> potions = new List<Potion>();

        // Systems. These are rebuilt on a new game so their timers start fresh.
        private WeaponSystem weapons = new WeaponSystem();
        private EnemySpawner spawner = new EnemySpawner();
        private PotionSpawner potionSpawner = new PotionSpawner();
        private Hud hud = new Hud();

        // Which screen we are on.
        private GameState gameState = GameState.Start;

        private KeyboardState previousKeyboard;
        private int score;

        public Game1() : base("Dungeon", 1280, 720, false)
        {
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            warriorTexture = Core.Content.Load<Texture2D>("Sprites/players_blue_x1[1]");
            pumpkinTexture = Core.Content.Load<Texture2D>("Sprites/pumpkin_dude");
            dungeonTileset = Core.Content.Load<Texture2D>("Sprites/0x72_DungeonTilesetII_v1.7[1]");
            font = Core.Content.Load<SpriteFont>("Fonts/DefaultFont");

            pixel = new Texture2D(((Game)this).GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            StartNewGame();

            base.LoadContent();
        }

        /// <summary>
        /// Resets everything back to a fresh run. Called before the first game and
        /// again whenever the player restarts from the game over screen.
        /// </summary>
        private void StartNewGame()
        {
            player = new Player(warriorTexture, new Vector2(300, 100));

            enemies.Clear();
            potions.Clear();
            score = 0;

            // New systems so the spawn timers, bullets and smoothed HUD values all
            // start from scratch rather than carrying over from the previous run.
            weapons = new WeaponSystem();
            spawner = new EnemySpawner();
            potionSpawner = new PotionSpawner();
            hud = new Hud();

            // Two starter potions; PotionSpawner adds more as the game runs.
            potions.Add(Potion.Create(new Vector2(500, 300), isHealthPotion: true));
            potions.Add(Potion.Create(new Vector2(560, 300), isHealthPotion: false));
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Escape))
                Exit();

            switch (gameState)
            {
                case GameState.Start:
                    UpdateMenu(keyboard, GameState.Playing);
                    break;

                case GameState.Playing:
                    UpdatePlaying(keyboard, deltaTime);
                    break;

                case GameState.GameOver:
                    UpdateMenu(keyboard, GameState.Playing);
                    break;
            }

            previousKeyboard = keyboard;
            base.Update(gameTime);
        }

        /// <summary>
        /// Waits for Enter on a menu screen, then starts a fresh game. Edge-triggered,
        /// so holding the key down cannot skip straight through the screen.
        /// </summary>
        private void UpdateMenu(KeyboardState keyboard, GameState nextState)
        {
            bool enterPressed = keyboard.IsKeyDown(Keys.Enter) && !previousKeyboard.IsKeyDown(Keys.Enter);
            if (!enterPressed)
                return;

            StartNewGame();
            gameState = nextState;
        }

        /// <summary>Runs one frame of actual gameplay.</summary>
        private void UpdatePlaying(KeyboardState keyboard, float deltaTime)
        {
            // Movement phase: everything moves to its new position for this frame.
            player.UpdateMovement(keyboard, deltaTime);
            weapons.HandleInput(keyboard, player, deltaTime);
            weapons.UpdateBullets(deltaTime, GraphicsDevice.Viewport);
            spawner.Update(deltaTime, GraphicsDevice.Viewport, enemies);
            potionSpawner.Update(deltaTime, GraphicsDevice.Viewport, potions);

            foreach (Enemy enemy in enemies)
                enemy.Update(player.Center, deltaTime);

            // Potions drift toward the player once they are close enough.
            foreach (Potion potion in potions)
                potion.Update(player.Center, deltaTime);

            // Combat phase: damage is worked out from the final positions.
            score += CombatSystem.ResolvePotionPickups(player, potions);
            score += CombatSystem.ResolveBulletHits(weapons.Bullets, enemies);
            CombatSystem.ResolveEnemyContact(player, enemies);

            player.UpdateAnimation(deltaTime);
            hud.Update(player.Health, score, deltaTime);

            // Out of health, so the run is over.
            if (player.Health <= 0)
                gameState = GameState.GameOver;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Purple);

            Core.SpriteBatch.Begin();

            switch (gameState)
            {
                case GameState.Start:
                    StartScreen.Draw(Core.SpriteBatch, font, GraphicsDevice.Viewport);
                    break;

                case GameState.Playing:
                    DrawWorld();
                    break;

                case GameState.GameOver:
                    // The world stays visible behind the game over text.
                    DrawWorld();
                    GameOverScreen.Draw(Core.SpriteBatch, font, GraphicsDevice.Viewport, score);
                    break;
            }

            Core.SpriteBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>Rendering the entities and the HUD.</summary>
        private void DrawWorld()
        {
            foreach (Enemy enemy in enemies)
                enemy.Draw(Core.SpriteBatch, pumpkinTexture);

            player.Draw(Core.SpriteBatch);
            weapons.Draw(Core.SpriteBatch, pixel);

            foreach (Potion potion in potions)
                potion.Draw(Core.SpriteBatch, dungeonTileset);

            hud.Draw(Core.SpriteBatch, dungeonTileset, font, GraphicsDevice.Viewport);
        }
    }
}
