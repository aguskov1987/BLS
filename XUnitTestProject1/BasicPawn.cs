// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

using System;

namespace BLS.Tests
{
    public class BasicPawn : BlsPawn
    {
        public virtual string Name { get; set; }
        public virtual DateTime Date { get; set; }
    }
}