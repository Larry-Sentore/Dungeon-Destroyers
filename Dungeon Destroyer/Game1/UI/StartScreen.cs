using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game1.UI
{
    /// <summary>The title screen shown before the game starts.</summary>
    public static class StartScreen
    {
        /// <summary>Rendering the title, the controls, and the start prompt.</summary>
        public static void Draw(SpriteBatch spriteBatch, SpriteFont font, Viewport viewport)
        {
            float centreY = viewport.Height / 2f;

            ScreenText.DrawCentered(spriteBatch, font, "DUNGEON DESTROYERS",
                viewport, centreY - 180f, GameConstants.TitleScale, Color.Orange);

            ScreenText.DrawCentered(spriteBatch, font, "Survive the pumpkin horde",
                viewport, centreY - 90f, 1f, Color.White);

            ScreenText.DrawCentered(spriteBatch, font, "Press ENTER to start",
                viewport, centreY - 10f, 1f, Color.White);

            ScreenText.DrawCentered(spriteBatch, font, "WASD  -  move",
                viewport, centreY + 80f, 0.8f, Color.LightGray);

            ScreenText.DrawCentered(spriteBatch, font, "ARROW KEYS  -  shoot",
                viewport, centreY + 120f, 0.8f, Color.LightGray);

            ScreenText.DrawCentered(spriteBatch, font, "ESC  -  quit",
                viewport, centreY + 160f, 0.8f, Color.LightGray);
        }
    }
}
