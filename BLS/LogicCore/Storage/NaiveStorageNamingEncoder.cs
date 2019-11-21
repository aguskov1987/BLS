namespace BLS
{
    /// <summary>
    /// Naive naming encoder
    /// </summary>
    internal class NaiveStorageNamingEncoder : IStorageNamingEncoder
    {
        public string EncodePawnContainerName(BlsPawn pawn)
        {
            return pawn.GetType().Name;
        }

        public string EncodePawnRelationName(BlsPawn source, BlsPawn target, string multiplexer)
        {
            return $"{source.GetType().Name}{multiplexer}{target.GetType().Name}";
        }
    }
}