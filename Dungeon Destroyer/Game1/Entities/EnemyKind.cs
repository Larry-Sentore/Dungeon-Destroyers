namespace Game1.Entities
{
    /// <summary>
    /// The enemy variants. Each kind's stats are applied in <see cref="Enemy.Create"/>.
    /// </summary>
    public enum EnemyKind
    {
        /// <summary>Fast and fragile: 2 health, deals 1 damage on contact.</summary>
        Small,

        /// <summary>Slow and tanky: 5 health, deals 3 damage on contact.</summary>
        Big,
    }
}
