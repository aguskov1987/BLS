using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BLS.Functional;
using BLS.PropertyValidation;
using BLS.Utilities;

namespace BLS
{
    /// <summary>
    /// This is a very important class. It is the in-memory graph of the business logic.
    /// the class resolves the relations defined in the business classes (derived from <see cref="BlsPawn"/>)
    /// and collapses them into uniquely named relations between containers. The containers, meanwhile,
    /// are constructed from the business classes themselves and each container defines the set of properties
    /// pawn objects in that containers will have. See containers (<see cref="BlGraphContainer"/>) and their
    /// property definitions (<see cref="BlContainerProp"/>) for further details.
    /// </summary>
    internal class BlGraph : IBlGraph
    {
        private bool _compiled;
        private List<BlGraphContainer> _compiledCollections = new List<BlGraphContainer>();
        private List<BlGraphRelation> _compiledRelations = new List<BlGraphRelation>();
        private BlsPawn[] _pawns;
        private IStorageNamingEncoder _storageNamingEncoder;

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
            foreach (BlsPawn pawn in _pawns)
            {
                ResolveContainerMetadataFromPawnSubClass(pawn);
            }
            ResolveRelations();
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

        public void ResolveContainerMetadataFromPawnSubClass(BlsPawn pawn)
        {
            BlGraphContainer container = new BlGraphContainer
            {
                Properties = new List<BlContainerProp>(),
                BlContainerName = pawn.GetType().Name,
                StorageContainerName = _storageNamingEncoder.EncodePawnContainerName(pawn)
            };

            container = ResolveContainerProps(container, pawn);
            _compiledCollections.Add(container);
        }

        public void OverrideStorageNamingEncoder(IStorageNamingEncoder encoder)
        {
            _storageNamingEncoder = encoder;
        }

        public string GetStorageContainerNameForPawn(BlsPawn pawn)
        {
            return _storageNamingEncoder.EncodePawnContainerName(pawn);
        }

        public void ResolveRelations()
        {
            var nodes = new List<LoseNode>();
            foreach (BlsPawn pawn in _pawns)
            {
                LoseNode node = ConvertToLoseNode(pawn);
                nodes.Add(node);
            }
        }

        private BlGraphContainer ResolveContainerProps(BlGraphContainer container, BlsPawn pawn)
        {
            var softDeleteFlagUsed = false;
            
            List<PropertyInfo> properties = pawn.GetType().GetProperties().ToList();
            foreach (PropertyInfo property in properties)
            {
                Type propType = property.PropertyType;
                
                if (BlUtils.IsEnumerableType(propType) && propType != typeof(string))
                {
                    throw new DisallowedPawnProperty($"Collection properties are not allowed in pawns: {property.Name} of {pawn.GetType().Name}");
                }
                
                Attribute[] attributes = property.GetCustomAttributes().ToArray();
                
                var blProp = new BlContainerProp();
                if (property.CanRead && property.CanWrite)
                {
                    container.Properties.Add(blProp);
                    
                    blProp.Name = property.Name;
                    blProp.PropType = propType;

                    if (attributes.Any())
                    {
                        foreach (Attribute attribute in attributes)
                        {
                            if (attribute is UsedForSoftDeletes)
                            {
                                if (softDeleteFlagUsed)
                                {
                                    throw new DuplicateSoftDeletionFlagError($"Attempting to declare second soft deletion flag in pawn {blProp.Name}. Only one soft deletion property is allowed per pawn");
                                }
                                blProp.IsSoftDeleteProp = true;
                                softDeleteFlagUsed = true;
                            }

                            if (attribute is FullTextSearchable)
                            {
                                if (blProp.PropType != typeof(string))
                                {
                                    throw new InvalidFullTextSearchAttributeError($"Attempting to apply a full text search attribute to a non-string property {blProp.Name} of {container.BlContainerName}");
                                }

                                blProp.IsSearchable = true;
                            }

                            if (attribute is StringLengthRestriction sRes)
                            {
                                if (blProp.PropType != typeof(string))
                                {
                                    throw new InvalidRestrictiveAttributeError($"Attempting to apply a string attribute to a non-string property {blProp.Name} of {container.BlContainerName}");
                                }

                                blProp.MinChar = sRes.MinCharacters;
                                blProp.MaxChar = sRes.MaxCharacters == 0 ? int.MaxValue : sRes.MaxCharacters;
                            }

                            if (attribute is NumberRestriction nRes)
                            {
                                if (!BlUtils.IsNumericType(blProp.PropType))
                                {
                                    throw new InvalidRestrictiveAttributeError($"Attempting to apply a numeric attribute to a non-number property {blProp.Name} of {container.BlContainerName}");
                                }

                                blProp.MinValue = nRes.Minimum;
                                blProp.MaxValue = Math.Abs(nRes.Maximum) < 0.000001 ? float.MaxValue : nRes.Maximum;
                            }

                            if (attribute is DateRestriction dRes)
                            {
                                if (blProp.PropType != typeof(DateTime))
                                {
                                    throw new InvalidRestrictiveAttributeError($"Attempting to apply a date restriction attribute to a non-date property {blProp.Name} of {container.BlContainerName}");
                                }

                                DateTime earliestValue;
                                DateTime latestValue;

                                if (string.IsNullOrEmpty(dRes.Earliest))
                                {
                                    earliestValue = DateTime.MinValue;
                                }
                                else
                                {
                                    var parsed = DateTime.TryParse(dRes.Earliest, out earliestValue);
                                    if (!parsed)
                                    {
                                        throw new InvalidRestrictiveAttributeError($"Date restriction attribute is not in correct format: {blProp.Name} of {container.BlContainerName}");
                                    }
                                }

                                if (string.IsNullOrEmpty(dRes.Latest))
                                {
                                    latestValue = DateTime.MaxValue;
                                }
                                else
                                {
                                    var parsed = DateTime.TryParse(dRes.Latest, out latestValue);
                                    if (!parsed)
                                    {
                                        throw new InvalidRestrictiveAttributeError($"Date restriction attribute is not in correct format: {blProp.Name} of {container.BlContainerName}");
                                    }
                                }

                                blProp.EarliestDate = earliestValue;
                                blProp.LatestDate = latestValue;
                            }
                        }
                    }
                }
            }

            return container;
        }

        private LoseNode ConvertToLoseNode(BlsPawn pawn)
        {
            throw new System.NotImplementedException();
        }

        #region Helper structures

        struct LoseEnd
        {
            public BlsPawn ToPawnRef { get; set; }
            public string ToName { get; set; }
            public string Multiplexer { get; set; }
            public bool Connected { get; set; }
        }

        struct LoseNode
        {
            public BlsPawn PawnRef { get; set; }
            public string Name { get; set; }
            public LoseEnd[] ConnectionPoints { get; set; }
        }

        #endregion
    }

    internal class DisallowedPawnProperty : Exception
    {
        public DisallowedPawnProperty(string message) : base(message)
        {
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