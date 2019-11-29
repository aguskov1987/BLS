// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace BLS.Tests
{
    internal class BasicPawn : BlsPawn
    {
        public virtual string Name { get; set; }
        public virtual DateTime Date { get; set; }
    }

    #region Valid model 1

    // Simple law firm business model:
    // There is a law firm which has a number of lawyers and assistants. Each lawyer has a number of assistants and each assistant can only have
    // one lawyer. Each lawyer can also have a number of clients. Clients can have one or more matters. Matters are basically files lawyers and
    // assistants work on for a particular client. Each matter can link to several other related matters
    internal class Matter : BlsPawn
    {
        public Matter()
        {
            RelatedMatters = new RelatesToMany<Matter>(this);
        }

        public virtual string Title { get; set; }
        
        // good example to present recursive relations
        public RelatesToMany<Matter> RelatedMatters { get; set; }
    }

    // BLS supports pawn inheritance so we can create a common Person pawn with common properties
    internal class PawnPerson : BlsPawn
    {
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
    }

    internal class Lawyer : PawnPerson
    {
        public Lawyer()
        {
            Clients = new RelatesToMany<Client>(this);
            Assistants = new RelatesToMany<Assistant>(this);
        }

        // Lawyers have access to their Assistants and Assistants can access their Lawyer
        public virtual RelatesToMany<Assistant> Assistants { get; set; }
        
        // Lawyer can access their clients. However, there is no requirement Clients need to access their Lawyer,
        // so the Client does not have a relation back to the Lawyer
        public virtual RelatesToMany<Client> Clients { get; set; }
    }

    internal class Assistant : PawnPerson
    {
        public Assistant()
        {
            Lawyer = new RelatesToOne<Lawyer>(this);
        }

        public virtual RelatesToOne<Lawyer> Lawyer { get; set; }
    }

    internal class Client : PawnPerson
    {
        public Client()
        {
            Matters = new RelatesToMany<Matter>(this);
        }

        public virtual RelatesToMany<Matter> Matters { get; set; }
    }

    internal class LawFirm : BlsPawn
    {
        public LawFirm()
        {
            Lawyers = new RelatesToMany<Lawyer>(this);
            Assistants = new RelatesToMany<Assistant>(this);
        }

        public virtual RelatesToMany<Lawyer> Lawyers { get; set; }
        public virtual RelatesToMany<Assistant> Assistants { get; set; }
        public virtual string Name { get; set; }
    }

    #endregion
    
    #region Valid model 2

    // In this model there is a car which has front wheels and back wheels
    internal class Car : BlsPawn
    {
        public Car()
        {
            FrontWheels = new RelatesToMany<Wheel>(this, "front");
            BackWheels = new RelatesToMany<Wheel>(this);
        }

        public RelatesToMany<Wheel> FrontWheels { get; set; }
        public RelatesToMany<Wheel> BackWheels { get; set; }
    }

    internal class Wheel : BlsPawn
    {
        public Wheel()
        {
            Car = new RelatesToOne<Car>(this, "front");
        }

        // In this case there is one relation back from the child to the parent and it is
        // specifier with the 'front' multiplexer. The graph resolver should match the multiplexed
        // relation and then resolve the BackWheels relation from the parent. The final picture should
        // look something like this:
        //    Car can retrieve their Front wheels and back wheels
        //    Wheels would only be able to retrieve their cars if the wheels is a front wheel
        public RelatesToOne<Car> Car { get; set; }
    }

    #endregion

    #region Invalid model 1

    internal class InvalidParent1 : BlsPawn
    {
        public InvalidParent1()
        {
            // two relations to the same pawn but no multiplexer
            Sons = new RelatesToMany<InvalidChild1>(this);
            Daughters = new RelatesToMany<InvalidChild1>(this);
        }
        
        public RelatesToMany<InvalidChild1> Sons { get; set; }
        public RelatesToMany<InvalidChild1> Daughters { get; set; }
    }

    internal class InvalidChild1 : BlsPawn
    {
        // Child does not have a link - impossible to resolve parent to child relation
    }

    #endregion
    
    #region Invalid model 2

    internal class InvalidParent2 : BlsPawn
    {
        public InvalidParent2()
        {
            Sons = new RelatesToMany<InvalidChild2>(this);
            Daughters = new RelatesToMany<InvalidChild2>(this);
        }

        public RelatesToMany<InvalidChild2> Sons { get; set; }
        public RelatesToMany<InvalidChild2> Daughters { get; set; }
    }

    internal class InvalidChild2 : BlsPawn
    {
        public InvalidChild2()
        {
            Parent = new RelatesToOne<InvalidParent2>(this);
        }

        // one relation back to the parent exists but there is an ambiguity of which relation (Sons or Daughters)
        // to attach to. need a multiplexer
        public RelatesToOne<InvalidParent2> Parent { get; set; }
    }

    #endregion
}