using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BLS
{
    public abstract class BlConnected<T>
    {
        private BlEntity _source;

        public BlConnectionType ConnectionType { get; }
        public string Demultiplexer { get; }

        /// <summary>
        /// Initializes a new channel between the source object and its one or several related objects
        /// </summary>
        /// <param name="source">The entity object from which the connected objects should originate</param>
        /// <param name="connectionType">One to One, One to Many</param>
        /// <param name="demultiplexer"></param>
        /// should be the same for reciprocal relations
        public BlConnected(BlEntity source, BlConnectionType connectionType, string demultiplexer = null)
        {
            _source = source;
            ConnectionType = connectionType;
            Demultiplexer = demultiplexer;
        }

        public void ConnectOne(T entity)
        {
            throw new NotImplementedException();
        }

        public void ConnectMany(List<T> entity)
        {
            throw new NotImplementedException();
        }

        public void DisconnectOne(T entity)
        {
            throw new NotImplementedException();
        }

        public void DisconnectMany(List<T> entity)
        {
            throw new NotImplementedException();
        }

        public void Move(List<T> entities, BlEntity newParent)
        {
            throw new NotImplementedException();
        }

        public BlStorageCursor<T> Find(Expression<Func<T, bool>> check = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Perform an aggregation calculation on the specified collection
        /// </summary>
        /// <typeparam name="TResult">Type of the property being computed</typeparam>
        /// <param name="type">Type of aggregation to perform</param>
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
            throw new NotImplementedException();
        }
    }

    public sealed class RelatesToOne<T> : BlConnected<T>
    {
        public RelatesToOne(BlEntity source, string demultiplexer = null) : base(source, BlConnectionType.OneToOne, demultiplexer)
        {
        }
    }

    public sealed class RelatesToMany<T> : BlConnected<T>
    {
        public RelatesToMany(BlEntity source, string demultiplexer = null) : base(source, BlConnectionType.OneToMany, demultiplexer)
        {
        }
    }
}