using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game1.UI
{
    /// <summary>
    /// Shared text drawing for the menu screens. Kept in one place so the start and
    /// game over screens line their text up the same way.
    /// </summary>
    internal static class ScreenText
    {
        /// <summary>
        /// Rendering a line of text centred horizontally on the screen.
        /// Algebra: x = (screen width - text width) / 2.
        /// </summary>
        public static void DrawCentered(
            SpriteBatch spriteBatch,
            SpriteFont font,
            string text,
            Viewport viewport,
            float y,
            float scale,
            Color color)
        {
            Vector2 size = font.MeasureString(text) * scale;
            Vector2 position = new Vector2((viewport.Width - size.X) / 2f, y);

            spriteBatch.DrawString(
                font,
                text,
                position,
                color,
                0f,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0f);
        }
    }
}
