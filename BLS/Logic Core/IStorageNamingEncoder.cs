namespace BLS
{
    public interface IStorageNamingEncoder
    {
        string EncodePawnContainerName(BlsPawn pawn);
        string EncodePawnRelationName(BlsPawn source, BlsPawn target, string multiplexer);
    }
}