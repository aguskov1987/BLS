using System;
using System.Linq.Expressions;

namespace BLS
{
    public abstract class Relation<T> where T : BlsPawn, new()
    {
        protected Relation(BlsPawn sourcePawn, string multiplexer, int minConnections, int maxConnections)
        {
            SourcePawn = sourcePawn;
            Multiplexer = multiplexer;
            MinConnections = minConnections;
            MaxConnections = maxConnections;
        }

        internal BlsPawn SourcePawn { get; }
        public string Multiplexer { get; }
        internal int MinConnections { get; }
        internal int MaxConnections { get; }
        
        public void Connect(T pawn)
        {
            
        }

        public void Disconnect(T pawn)
        {
            
        }
    }
    
    public sealed class RelatesToOne<T> : Relation<T> where T : BlsPawn, new()
    {
        public RelatesToOne(BlsPawn source, string multiplexer = "") : base(source, multiplexer, 0, 1)
        {
        }

        public T Get(bool includeSoftDeleted = false)
        {
            return null;
        }
    }

    public sealed class RelatesToMany<T> : Relation<T> where T : BlsPawn, new()
    {
        public RelatesToMany(BlsPawn source, string multiplexer = "", int min = 0, int max = int.MaxValue)
            : base(source, multiplexer, min, max)
        {
        }
        
        public StorageCursor<T> Find(Expression<Func<T, bool>> filter = null, bool includeSoftDeleted = false)
        {
            throw new NotImplementedException();
        }
        
        public TResult ComputeFor<TResult>(int type,
            Expression<Func<T, TResult>> computation, bool includeSoftDeleted = false)
        {
            throw new NotImplementedException();
        }
        
        public int GetCount(bool includeSotDeleted = false)
        {
            throw new NotImplementedException();
        }
    }
}