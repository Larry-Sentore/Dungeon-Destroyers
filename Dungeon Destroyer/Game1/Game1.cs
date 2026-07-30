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
    /// Wires the game together: loads assets, owns the entity lists and systems, and
    /// drives the per-frame update and draw order. Gameplay logic itself lives in the
    /// Entities and Systems classes.
    /// </summary>
    public class Game1 : Core
    {
        // Textures & fonts
        private Texture2D warriorTexture;
        private Texture2D pumpkinTexture;
        private Texture2D dungeonTileset;
        private Texture2D pixel; // 1x1 white texture, bullets.
        private SpriteFont font;

        // Entities
        private Player player;
        private readonly List<Enemy> enemies = new List<Enemy>();
        private readonly List<Potion> potions = new List<Potion>();

        // Systems
        private readonly WeaponSystem weapons = new WeaponSystem();
        private readonly EnemySpawner spawner = new EnemySpawner();
        private readonly PotionSpawner potionSpawner = new PotionSpawner();

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

            player = new Player(warriorTexture, new Vector2(300, 100));

            // Two starter potions; PotionSpawner adds more as the game runs.
            potions.Add(Potion.Create(new Vector2(500, 300), isHealthPotion: true));
            potions.Add(Potion.Create(new Vector2(560, 300), isHealthPotion: false));

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Escape))
                Exit();

            // Movement phase: everything relocates for this frame
            player.UpdateMovement(keyboard, deltaTime);
            weapons.HandleInput(keyboard, player, deltaTime);
            weapons.UpdateBullets(deltaTime, GraphicsDevice.Viewport);
            spawner.Update(deltaTime, GraphicsDevice.Viewport, enemies);
            potionSpawner.Update(deltaTime, GraphicsDevice.Viewport, potions);

            foreach (Enemy enemy in enemies)
                enemy.Update(player.Bounds.Center.ToVector2(), deltaTime);

            // damage is applied to final positions
            score += CombatSystem.ResolvePotionPickups(player, potions);
            score += CombatSystem.ResolveBulletHits(weapons.Bullets, enemies);
            CombatSystem.ResolveEnemyContact(player, enemies);

            player.UpdateAnimation(deltaTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Purple);

            Core.SpriteBatch.Begin();

            foreach (Enemy enemy in enemies)
                enemy.Draw(Core.SpriteBatch, pumpkinTexture);

            player.Draw(Core.SpriteBatch);
            weapons.Draw(Core.SpriteBatch, pixel);

            foreach (Potion potion in potions)
                potion.Draw(Core.SpriteBatch, dungeonTileset);

            Hud.Draw(Core.SpriteBatch, dungeonTileset, font, player.Health, score, GraphicsDevice.Viewport);

            Core.SpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
