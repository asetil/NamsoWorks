/*
 * Yeni nesne üretme maliyetinin yüksek olduğu durumlarda (Üretilip belleğe aktarılması zaman açısından
 * maliyetli olan bir nesne veya new operatörü ile oluşan maaliyetten kaçınmak istendiğinde)
 * bu patterni kullanarak var olan nesnelerin kopyasını almak daha mantıklı. Mesela Age of Empires :)
 * İki tip kopya yöntemi var:shallow ve deep
 * 
 * shallow yönteminde nesnenin üye elemanlarıbit bit kopyalanırken referrans tipteki öğelerin sadece referansı kopylanır.
 * Kopyalanan nesnedeki referans güncellenirse ana kopyadaki de değişir.
 * 
 * Deep copy yönteminde ise nesne herşeyi ile kopyalanır. Tamamen yenibir nesne üretilir.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CleanCode.DesignPattern.Prototype
{
    // Prototype Class
    abstract class GameScenePrototype
    {
        //shallow copy
        public abstract GameScenePrototype Clone();

        //deep copy
        public GameScenePrototype DeepCopy()
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, this);
            stream.Seek(0, SeekOrigin.Begin);
            var copy = (GameScenePrototype)formatter.Deserialize(stream);
            stream.Close();
            return copy;
        }


        public abstract string Draw();
    }

    // Concrete Prototype Class A
    class Hero : GameScenePrototype
    {
        public int Width { get; set; }
        public int Heigth { get; set; }
        public string Name { get; set; }
        public HeroType Type { get; set; }
        public Hero(int width, int heigth, string name, HeroType heroType)
        {
            Width = width;
            Heigth = heigth;
            Name = name;
            Type = heroType;
        }

        public override GameScenePrototype Clone()
        {
            return this.MemberwiseClone() as GameScenePrototype;
        }

        public override string Draw()
        {
            return string.Format("I'm hero with name {0}", Name);
        }
    }

    // Concrete Prototype class B
    class Mine : GameScenePrototype
    {
        public double Gravity { get; set; }
        public MineType Type { get; set; }
        public Mine(double gravity, MineType mineType)
        {
            Gravity = gravity;
            Type = mineType;
        }
        public override GameScenePrototype Clone()
        {
            return this.MemberwiseClone() as GameScenePrototype;
        }

        public override string Draw()
        {
            return string.Format("This is a mine with {0} gravity of type {1}",Gravity,Type);
        }
    }

    // Prototype Manager class
    class GameSceneManager
    {
        public List<GameScenePrototype> GameObjects { get; set; }
        public GameSceneManager()
        {
            GameObjects = new List<GameScenePrototype>();
        }

        public void Test()
        {
            Hero hero1 = new Hero(10, 20, "Bıkanyus", HeroType.Archer);
            GameObjects.Add(hero1);
            Hero hero2 = new Hero(15, 35, "Wah!tupus", HeroType.Employee);
            GameObjects.Add(hero2);
            Mine mine1 = new Mine(3, MineType.Gold);
            GameObjects.Add(mine1);
            Mine mine2 = new Mine(5, MineType.Silver);
            GameObjects.Add(mine2);

            // Var olan Mine ve Hero nesne örneklerinden klonlama yapılır
            GameObjects.Add(mine2.Clone() as Mine);
            GameObjects.Add(hero1.Clone() as Hero);

            foreach (var prototype in GameObjects)
            {
                Console.WriteLine(prototype.Draw());
            }
        }
    }

    #region Yardımcılar
    enum HeroType
    {
        Warrior,
        Employee,
        Archer
    }
    enum MineType
    {
        Gold,
        Silver,
        Bronze
    }
    #endregion
}
