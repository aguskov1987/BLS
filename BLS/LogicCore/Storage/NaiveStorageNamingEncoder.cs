using System.Linq;

namespace BLS
{
    /// <summary>
    /// Naive naming encoder
    /// </summary>
    internal class NaiveStorageNamingEncoder : IStorageNamingEncoder
    {
        public string EncodePawnContainerName(string pawn)
        {
            return pawn;
        }

        public string EncodePawnRelationName(string source, string target, string multiplexer)
        {
            var containers = new[] {source, target}.OrderBy(c => c).ToArray();
            return $"{containers[0]}{multiplexer}{containers[1]}";
        }
    }
}