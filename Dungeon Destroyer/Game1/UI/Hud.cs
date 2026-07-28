using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game1.UI
{
    /// <summary>Heads-up display: health hearts top-left, score top-right.</summary>
    public static class Hud
    {
        public static void Draw(
            SpriteBatch spriteBatch,
            Texture2D tileset,
            SpriteFont font,
            int currentHealth,
            int score,
            Viewport viewport)
        {
            DrawHearts(spriteBatch, tileset, currentHealth);
            DrawScore(spriteBatch, font, score, viewport);
        }

        /// <summary>One heart per point of max health, filled or empty by current health.</summary>
        private static void DrawHearts(SpriteBatch spriteBatch, Texture2D tileset, int currentHealth)
        {
            Vector2 heartPosition = new Vector2(GameConstants.HudMargin, GameConstants.HudMargin);

            for (int i = 0; i < GameConstants.MaxHealth; i++)
            {
                Rectangle frame = i < currentHealth
                    ? GameConstants.HeartFullFrame
                    : GameConstants.HeartEmptyFrame;

                spriteBatch.Draw(
                    tileset,
                    heartPosition,
                    frame,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    GameConstants.HeartScale,
                    SpriteEffects.None,
                    0f);

                heartPosition.X += (GameConstants.HeartSize * GameConstants.HeartScale)
                    + GameConstants.HeartSpacing;
            }
        }

        /// <summary>Right-aligned score, measured so it stays flush to the edge as digits grow.</summary>
        private static void DrawScore(SpriteBatch spriteBatch, SpriteFont font, int score, Viewport viewport)
        {
            string scoreText = $"Score: {score}";
            Vector2 textSize = font.MeasureString(scoreText);
            Vector2 position = new Vector2(
                viewport.Width - textSize.X - GameConstants.HudMargin,
                GameConstants.HudMargin);

            spriteBatch.DrawString(font, scoreText, position, Color.White);
        }
    }
}
