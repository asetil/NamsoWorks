using System;

namespace CleanCode.DesignPattern
{
    // Component
    interface IArm
    {
        string Name{get;set;}
        void Fire();
    }

    // ConcreteComponent
    class Artillery : IArm
    {
        protected double _barrel;
        protected double _range;
        public string Name { get; set;}

        public Artillery(double barrel, double range, string name)
        {
            _barrel = barrel;
            _range = range;
            Name = name;
        }

        public void Fire()
        {
            Console.WriteLine("{0} sınıfından olan topçu, {1} mm namlusundan {2} mesafeye ateşleme yaptı", Name, _barrel.ToString(), _range.ToString());
        }
    }

    // Decorator
    abstract class ArmsDecorator : IArm
    {
        protected IArm _arms;

        public string Name { get; set; }

        public ArmsDecorator(IArm arms)
        {
            _arms = arms;
        }
        public void Fire()
        {
            if (_arms != null)
                _arms.Fire();
        }
    }

    // ConcreteDecorator
    class ArtilleryDecorator : ArmsDecorator
    {
        public ArtilleryDecorator(IArm arms)
            : base(arms)
        {
        }

        public void Defense()
        {
            Console.WriteLine("\t{0} Savunma Modu!", _arms.Name);
        }
        public void Easy()
        {
            Console.WriteLine("\t{0} Atış serbest modu!", _arms.Name);
        }

        public void Fire()
        {
            base.Fire();
        }
    }
}