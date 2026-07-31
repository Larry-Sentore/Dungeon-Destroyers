namespace Game1.UI
{
    /// <summary>The screen the game is currently on.</summary>
    public enum GameState
    {
        /// <summary>Title screen, waiting for the player to start.</summary>
        Start,

        /// <summary>The game is being played.</summary>
        Playing,

        /// <summary>The player has run out of health.</summary>
        GameOver,
    }
}
