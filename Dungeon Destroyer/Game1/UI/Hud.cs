using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Game1.Maths;

namespace Game1.UI
{
    /// <summary>
    /// The on-screen display: hearts top-left, score top-right. It keeps its own
    /// display values and lerps them toward the real ones, so the health and score
    /// slide smoothly instead of jumping.
    /// </summary>
    public class Hud
    {
        // The values actually drawn. They chase the real values every frame.
        private float displayedHealth = GameConstants.MaxHealth;
        private float displayedScore;

        /// <summary>Lerp: eases the drawn health and score toward their real values.</summary>
        public void Update(int currentHealth, int score, float deltaTime)
        {
            float t = GameConstants.HudLerpSpeed * deltaTime;

            displayedHealth = MathUtils.Lerp(displayedHealth, currentHealth, t);
            displayedScore = MathUtils.Lerp(displayedScore, score, t);
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D tileset, SpriteFont font, Viewport viewport)
        {
            DrawHearts(spriteBatch, tileset);
            DrawScore(spriteBatch, font, viewport);
        }

        /// <summary>
        /// Rendering one heart per health point. The heart at the edge of the current
        /// health fades part-way, which is what makes the bar look smooth.
        /// </summary>
        private void DrawHearts(SpriteBatch spriteBatch, Texture2D tileset)
        {
            Vector2 heartPosition = new Vector2(GameConstants.HudMargin, GameConstants.HudMargin);

            for (int i = 0; i < GameConstants.MaxHealth; i++)
            {
                // Algebra: how full this particular heart should be, from 0 to 1.
                float fill = MathHelper.Clamp(displayedHealth - i, 0f, 1f);

                // Empty heart underneath, so a part-faded full heart has a backing.
                spriteBatch.Draw(
                    tileset,
                    heartPosition,
                    GameConstants.HeartEmptyFrame,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    GameConstants.HeartScale,
                    SpriteEffects.None,
                    0f);

                // Lerp: fades the full heart in over the empty one.
                if (fill > 0f)
                {
                    spriteBatch.Draw(
                        tileset,
                        heartPosition,
                        GameConstants.HeartFullFrame,
                        Color.White * fill,
                        0f,
                        Vector2.Zero,
                        GameConstants.HeartScale,
                        SpriteEffects.None,
                        0f);
                }

                // Algebra: step across by one heart width plus the gap.
                heartPosition.X += (GameConstants.HeartSize * GameConstants.HeartScale)
                    + GameConstants.HeartSpacing;
            }
        }

        /// <summary>Rendering the score, right-aligned so it stays flush as digits grow.</summary>
        private void DrawScore(SpriteBatch spriteBatch, SpriteFont font, Viewport viewport)
        {
            // Rounds the lerped value, so the score counts up instead of jumping.
            string scoreText = $"Score: {(int)MathF.Round(displayedScore)}";

            // Algebra: x position = screen width - text width - margin.
            Vector2 textSize = font.MeasureString(scoreText);
            Vector2 position = new Vector2(
                viewport.Width - textSize.X - GameConstants.HudMargin,
                GameConstants.HudMargin);

            spriteBatch.DrawString(font, scoreText, position, Color.White);
        }
    }
}
