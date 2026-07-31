namespace Game1.Entities
{
    /// <summary>The two enemy types. Stats are applied in Enemy.Create.</summary>
    public enum EnemyKind
    {
        /// <summary>Fast and weak: 2 health, deals 1 damage.</summary>
        Small,

        /// <summary>Slow and tough: 5 health, deals 3 damage.</summary>
        Big,
    }
}
