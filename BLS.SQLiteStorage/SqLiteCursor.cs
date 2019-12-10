namespace BLS.SQLiteStorage
{
    internal class SqLiteCursor
    {
        /// <summary>
        /// the id of the cursor
        /// </summary>
        internal string CursorId;
        /// <summary>
        /// how many objects to retrieve in one call
        /// </summary>
        internal int BatchSize;
        /// <summary>
        /// what's the current object (on the fist call, it's always 0)
        /// </summary>
        internal int CurrentStartingPoint;
    }
}