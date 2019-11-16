using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using BLS.Utilities;

namespace BLS
{
    public abstract class BlConnected<T> where T : BlEntity
    {
        private readonly string _resolvedFromContainer;

        private readonly string _resolvedRelationName;
        protected readonly string ResolvedToContainer;
        private readonly HashSet<T> _addBuffer;
        private readonly HashSet<Tuple<BlEntity, T>> _moveBuffer;
        private readonly HashSet<T> _removeBuffer;

        private readonly BlEntity _source;

        protected BlConnected(BlEntity source, BlConnectionType connectionType, string multiplexer = null)
        {
            _addBuffer = new HashSet<T>();
            _removeBuffer = new HashSet<T>();
            _moveBuffer = new HashSet<Tuple<BlEntity, T>>();

            _source = source;
            ConnectionType = connectionType;
            Multiplexer = multiplexer;

            _resolvedRelationName = BlUtils.ResolveRelationName(_source.GetType(), typeof(T), multiplexer);
            _resolvedFromContainer = BlUtils.ResolveContainerName(_source.GetType());
            ResolvedToContainer = BlUtils.ResolveContainerName(typeof(T));

            source.OnPersist += SourceOnPersist;
        }

        public BlConnectionType ConnectionType { get; }
        public string Multiplexer { get; }

        public void Connect(T entity)
        {
            if (_removeBuffer.Contains(entity))
            {
                _removeBuffer.Remove(entity);
            }

            _addBuffer.Add(entity);
        }

        public void Disconnect(T entity)
        {
            if (_addBuffer.Contains(entity))
            {
                _addBuffer.Remove(entity);
            }
            else
            {
                _removeBuffer.Add(entity);
            }
        }

        public void MoveTo(T entity, BlEntity newConnection)
        {
            var t1 = newConnection.GetType().FullName;
            var t2 = _source.GetType().FullName;
            if (t1 == t2)
            {
                _moveBuffer.Add(new Tuple<BlEntity, T>(newConnection, entity));
            }
        }

        private void SourceOnPersist(object sender, PersistEventArgs e)
        {
            PersistAdditions(e.TransactionId);
            PersistRemovals(e.TransactionId);
            PersistMoves(e.TransactionId);

            BlUtils.StorageRef.CommitTransaction(e.TransactionId);
        }

        private void PersistMoves(string transactionId)
        {
            foreach (var tuple in _moveBuffer)
            {
                BlEntity newParent = tuple.Item1;
                T entityToMove = tuple.Item2;

                // check if the new parent to move to is already in storage, if so,
                // move the relation; otherwise persist the parent first
                if (newParent.Id != null)
                {
                    bool removed = BlUtils.StorageRef.RemoveRelation(_resolvedFromContainer, _source.Id,
                        _resolvedRelationName, ResolvedToContainer, entityToMove.Id, transactionId);
                    if (removed)
                    {
                        BlUtils.StorageRef.InsertRelation(_resolvedFromContainer, newParent.Id,
                            _resolvedRelationName, ResolvedToContainer, entityToMove.Id, transactionId);
                    }
                }
                else
                {
                    newParent.PersistWithNoPropagation(transactionId);
                    bool removed = BlUtils.StorageRef.RemoveRelation(_resolvedFromContainer, _source.Id,
                        _resolvedRelationName, ResolvedToContainer, entityToMove.Id, transactionId);
                    if (removed)
                    {
                        BlUtils.StorageRef.InsertRelation(_resolvedFromContainer, newParent.Id,
                            _resolvedRelationName, ResolvedToContainer, entityToMove.Id, transactionId);
                    }
                }
            }
        }

        private void PersistRemovals(string transactionId)
        {
            foreach (var entity in _removeBuffer)
            {
                if (entity.Id != null)
                {
                    BlUtils.StorageRef.RemoveRelation(_resolvedFromContainer, _source.Id,
                        _resolvedRelationName, ResolvedToContainer, transactionId);
                }
            }
        }

        private void PersistAdditions(string transactionId)
        {
            foreach (var entity in _addBuffer)
            {
                if (entity.Id == null && entity.PersistWithNoPropagation(transactionId))
                {
                    BlUtils.StorageRef.InsertRelation(_resolvedFromContainer, _source.Id,
                        _resolvedRelationName, ResolvedToContainer, transactionId);
                }

                // todo: if not null
            }
        }
    }

    /// <summary>
    /// Relation to one entity
    /// </summary>
    /// <typeparam name="T">Type of the related entity</typeparam>
    public sealed class RelatesToOne<T> : BlConnected<T> where T : BlEntity
    {
        /// <summary>
        /// Initialize a new 'one to one' relation
        /// </summary>
        /// <param name="source">use 'this' when initializing from constructor of the source entity</param>
        /// <param name="multiplexer">Used if there is more than one relation to the same entity</param>
        public RelatesToOne(BlEntity source, string multiplexer = null)
            : base(source, BlConnectionType.OneToOne, multiplexer)
        {
        }

        /// <summary>
        /// Returns the related entity or null if there is none
        /// </summary>
        /// <returns>Related entity</returns>
        public T Get()
        {
            return null;
        }
    }

    /// <summary>
    /// Relation to a collection of entities
    /// </summary>
    /// <typeparam name="T">Type of the related entities</typeparam>
    public sealed class RelatesToMany<T> : BlConnected<T> where T : BlEntity
    {
        /// <summary>
        /// Initialize a new 'one to many' relation
        /// </summary>
        /// <param name="source">use 'this' when initializing from constructor of the source entity</param>
        /// <param name="multiplexer">Used if there is more than one relation to the same entity</param>
        public RelatesToMany(BlEntity source, string multiplexer = null)
            : base(source, BlConnectionType.OneToMany, multiplexer)
        {
        }

        /// <summary>
        /// Find all related entities or only those meeting the condition if the
        /// <paramref name="filter"/> is provided
        /// </summary>
        /// <param name="filter">Condition to filter the results</param>
        /// <returns>Cursor containing the result set</returns>
        public BlStorageCursor<T> Find(Expression<Func<T, bool>> filter = null)
        {
            return BlUtils.StorageRef.FindInContainer(ResolvedToContainer, filter);
        }

        /// <summary>
        /// Perform an aggregation calculation on the specified collection
        /// </summary>
        /// <typeparam name="TResult">Type of the property being computed</typeparam>
        /// <param name="type">Kind of aggregation to perform</param>
        /// <param name="computation">The expression to evaluate for each object before performing aggregation</param>
        /// <returns>Aggregated value</returns>
        public TResult ComputeFor<TResult>(SsComputationType type, Expression<Func<T, TResult>> computation)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the total number of entities in the relation
        /// </summary>
        /// <returns>Number of entities</returns>
        public int GetCount()
        {
            return BlUtils.StorageRef.GetContainerEntityCount(ResolvedToContainer);
        }
    }
}