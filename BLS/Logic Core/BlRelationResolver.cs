using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BLS
{
    public class BlRelationResolver : IBlRelationResolver
    {
        class Connection
        {
            public string TargetName { get; set; }
            public string Mx { get; set; }
        }
        class ConnectedEntity
        {
            public ConnectedEntity()
            {
                Targets = new List<Connection>();
            }

            public string SourceName { get; set; }
            public List<Connection> Targets { get; set; }
        }

        private List<ConnectedEntity> _entities;
        private List<Tuple<string, string>> _fullTextSearchProps;
        private List<Tuple<string, string>> _softDeletionProps;

        public BlRelationResolver()
        {
            _entities = new List<ConnectedEntity>();
            _fullTextSearchProps = new List<Tuple<string, string>>();
            _softDeletionProps = new List<Tuple<string, string>>();
        }

        public void AddEntityWithRelation(BlEntity entityType)
        {
            Type tp = entityType.GetType();
            ConnectedEntity entity = new ConnectedEntity {SourceName = tp.Name};

            List<PropertyInfo> properties = tp.GetProperties().ToList();
            foreach (PropertyInfo property in properties)
            {
                var propType = property.PropertyType;
                if (propType.BaseType != null
                    && propType.Name.Contains("RelatesTo")
                    && propType.BaseType.Name == typeof(BlConnected<BlEntity>).Name
                    && propType.IsGenericType
                    && propType.GenericTypeArguments.Length == 1)
                {
                    Type relatedType = propType.GenericTypeArguments[0];

                    var obj = entityType.GetType().GetProperty(property.Name)?.GetValue(entityType, null);
                    if (obj != null)
                    {
                        Type objectType = obj.GetType().BaseType;
                        if (objectType != null)
                        {
                            var mx = objectType.GetProperty("Multiplexer");
                            string mxName = mx.GetValue(obj).ToString();
                            var connection = new Connection
                            {
                                TargetName = relatedType.Name,
                                Mx = mxName
                            };
                            entity.Targets.Add(connection);
                        }
                    }
                }
            }
            _entities.Add(entity);
        }

        public void VerifyRelationalIntegrity()
        {
            throw new NotImplementedException();
        }

        public List<string> GetResolvedContainers()
        {
            return _entities.Select(e => e.SourceName).ToList();
        }

        public List<Tuple<string, string, string>> GetResolvedRelations()
        {
            throw new NotImplementedException();
        }
    }
}