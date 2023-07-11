using System;

namespace CleanCode.DesignPattern
{
    public abstract class AbstractClass : IDesignPattern
    {
        public string X() { return "a"; }
        public abstract string Y();
        public abstract void Test();
    }

    public class Abstract1 : AbstractClass
    {

        public override string Y()
        {
            return "b";
        }

        public override void Test()
        {
            throw new NotImplementedException();
        }
    }

    public abstract class Abstract2 : AbstractClass
    {
        public override string Y()
        {
            return "cc";
        }
        public abstract void Z();
    }

    public class Abstract3 : Abstract2
    {
        public override string Y()
        {
            return "bb";
        }

        public override void Test()
        {
            throw new NotImplementedException();
        }

        public override void Z()
        {
            throw new NotImplementedException();
        }
    }
}
