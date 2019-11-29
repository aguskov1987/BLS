namespace BLS
{
    /// <summary>
    /// This is an interface for storage naming encoder. Different storage solutions can have
    /// different constrains on how containers and their relations are named including max/min length of
    /// the container/relation name as well as allowed/disallowed characters.
    /// </summary>
    public interface IStorageNamingEncoder
    {
        string EncodePawnContainerName(string pawn);
        string EncodePawnRelationName(string source, string target, string multiplexer);
    }
}