using System.Collections.Generic;
using System.Linq;

namespace BLS
{
    internal class BlGraph : IBlGraph
    {
        private bool _compiled;
        private IStorageNamingEncoder _storageNamingEncoder;
        private List<BlGraphContainer> _compiledCollections = new List<BlGraphContainer>();
        private List<BlGraphRelation> _compiledRelations = new List<BlGraphRelation>();
        private BlsPawn[] _pawns;

        public void RegisterPawns(BlsPawn[] pawns)
        {
            _pawns = pawns;
            
            if (_pawns == null || _pawns.Length == 0)
            {
                throw new EmptyPawnCollectionError();
            }
            
            VerifyUniqueNames(pawns);
            _storageNamingEncoder = new NaiveStorageNamingEncoder();
        }

        public void CompileGraph()
        {
        }

        public List<BlGraphContainer> CompiledCollections => _compiledCollections;

        public List<BlGraphRelation> CompiledRelations => _compiledRelations;

        public void VerifyUniqueNames(BlsPawn[] pawns)
        {
            var names = pawns.Select(p => p.GetType().Name).ToArray();
            var distinctNames = names.Distinct().ToArray();
            if (distinctNames.Length >= names.Length)
                return;
            
            var dupes = names.Intersect(distinctNames).ToArray();
            throw new DuplicateFoundInPawnCollectionError(string.Join(',', dupes));
        }
    }
}

//public void AddEntityWithRelation(ConnectedPawn entityType)
//{
//Type tp = entityType.GetType();
//ConnectedEntity entity = new ConnectedEntity {SourceName = tp.Name};
//
//List<PropertyInfo> properties = tp.GetProperties().ToList();
//    foreach (PropertyInfo property in properties)
//{
//    var propType = property.PropertyType;
//    if (propType.BaseType != null
//        && propType.Name.Contains("RelatesTo")
//        && propType.BaseType.Name == typeof(Relation<>).Name
//        && propType.IsGenericType
//        && propType.GenericTypeArguments.Length == 1)
//    {
//        Type relatedType = propType.GenericTypeArguments[0];
//
//        var obj = entityType.GetType().GetProperty(property.Name)?.GetValue(entityType, null);
//        if (obj != null)
//        {
//            Type objectType = obj.GetType().BaseType;
//            if (objectType != null)
//            {
//                var mx = objectType.GetProperty("Multiplexer");
//                string mxName = mx.GetValue(obj).ToString();
//                var connection = new Connection
//                {
//                    TargetName = relatedType.Name,
//                    Mx = mxName
//                };
//                entity.Targets.Add(connection);
//            }
//        }
//    }
//}
//_entities.Add(entity);
//}