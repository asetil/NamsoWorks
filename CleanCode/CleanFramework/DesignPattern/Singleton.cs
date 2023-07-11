/*
 * Design Patterns in object oriented world is reusable solution to common software design problems which occur 
 * again and again in real world application development.
 * 
 * Creational Design Pattern : Singleton, Prototype,Abstract Factory
 * Structural Design Patterns : Facade, Decorator, Adapter
 * Behavioral Design Patterns :Observer, Memento,Chain of Responsibility
 *
 * Singleton deseni bir programın yaşam süresince belirli bir nesneden sadece bir örneğinin(instance) olmasını
 * garantiler.
 * 
 */

namespace CleanCode.DesignPattern
{
    public class Singleton : IDesignPattern
    {
        private static Singleton _single;
        private Singleton() { } //private constructor new diyerek yeni instance oluşturmayı engeller
        private Singleton(string name) { _name=name; } //private constructor new diyerek yeni instance oluşturmayı engeller
       
        private static object _lock = new object();
        private string _name;
        public static Singleton GetInstance(string name)
        {
            if (_single == null) { _single = new Singleton(name); }
            return _single;
        }

        public static Singleton GetThreadSafeInstance() //Multi thread için synclock kullanımı
        {
            if (_single == null)
            {
                lock (_lock)
                {
                    if (_single == null)
                        _single = new Singleton();
                }
            }
            return _single;
        }

        public override string ToString()
        {
            return _name;
        }
    }
}
