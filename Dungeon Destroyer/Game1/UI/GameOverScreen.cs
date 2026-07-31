using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game1.UI
{
    /// <summary>The screen shown once the player runs out of health.</summary>
    public static class GameOverScreen
    {
        /// <summary>Rendering the result, the final score, and the restart prompt.</summary>
        public static void Draw(SpriteBatch spriteBatch, SpriteFont font, Viewport viewport, int score)
        {
            float centreY = viewport.Height / 2f;

            ScreenText.DrawCentered(spriteBatch, font, "GAME OVER",
                viewport, centreY - 160f, GameConstants.TitleScale, Color.OrangeRed);

            ScreenText.DrawCentered(spriteBatch, font, $"Final score: {score}",
                viewport, centreY - 60f, 1.2f, Color.White);

            ScreenText.DrawCentered(spriteBatch, font, "Press ENTER to play again",
                viewport, centreY + 30f, 1f, Color.White);

            ScreenText.DrawCentered(spriteBatch, font, "Press ESC to quit",
                viewport, centreY + 90f, 0.8f, Color.LightGray);
        }
    }
}
