using System;
using CleanCode.DesignPattern.Observer.Base;
using CleanCode.DesignPattern.Observer.Publishers;
using CleanCode.DesignPattern.Observer.Subscribers;
using CleanCode.DesignPattern.Prototype;

namespace CleanCode.DesignPattern
{
    public static class DbTester
    {
        public static void TestAbstractClass()
        {
            Abstract1 b = new Abstract1();
            Console.WriteLine(b.X());
            Abstract3 d = new Abstract3();
            Console.WriteLine(d.Y());
        }

        public static void TestSingleton()
        {
            //Bu şekilde hata verecek
            //Singleton x = new Singleton();
            //Console.WriteLine(x.ToString());

            Console.WriteLine("Singleton Test : 3 instance alıyorum üçü de aynı :)");

            Singleton s1 = Singleton.GetInstance("Singleton Design Pattern1");
            Console.WriteLine(s1.ToString());

            Singleton s2 = Singleton.GetInstance("Singleton Design Pattern2");
            Console.WriteLine(s2.ToString());

            Singleton s3 = Singleton.GetInstance("Singleton Design Pattern3");
            Console.WriteLine(s3.ToString());
        }

        public static void TestFactoryPattern()
        {
           var factoryClient=new FactoryClient();
            factoryClient.Test();
        }

        public static void TestAbstractFactoryPattern()
        {
            AbstractFactoryApplicationClass msSql = new AbstractFactoryApplicationClass((new MSSQLFactory()));
            msSql.Connect();
            msSql.Execute();
            msSql.Disconnect();
            Console.WriteLine();

            AbstractFactoryApplicationClass oracle = new AbstractFactoryApplicationClass((new OracleFactory()));
            oracle.Connect();
            oracle.Execute();
            oracle.Disconnect();
            Console.ReadLine();
        }

        public static void TestMemento()
        {
            var noteTaker = new NoteTaker();
            noteTaker.CreateNote();
            noteTaker.TypeText("saddasdasdsad");
            noteTaker.Show();
            noteTaker.Paste("yener");
            noteTaker.Show();
            noteTaker.PreviousStep();
            noteTaker.Show();


            GameWorld zulu = new GameWorld { Name = "Zulu", Popluation = 10000 };
            Console.WriteLine(zulu.ToString());
            GameWorldCareTaker taker = new GameWorldCareTaker();
            taker.World = zulu.Save();

            zulu.Popluation += 10;
            Console.WriteLine(zulu.ToString());

            zulu.Load(taker.World);
            Console.WriteLine(zulu.ToString());
        }

        public static void TestObserver()
        {
            Kanal19 k19 = new Kanal19();
            Kanal20 k20 = new Kanal20();

            CorumHaberAjansi CHA = new CorumHaberAjansi();
            SungurluHaberAjansi SHA = new SungurluHaberAjansi();

            CHA.AddSubscriber(k19);
            CHA.Subscribers.Add(k20);
            SHA.AddSubscriber(k20);

            CHA.SendNews(NewsCategory.Sport, "Burak Yılmaz Galatasarayda", "Galatsary 5 Milyon Euro bedeli ile Trabzonspordan Burak Yılmaz'ı kadrosuna dahil etti!");
            CHA.SendNews(NewsCategory.Sport, "Observer Tasarım Deseni", "Yenilikçi bir desen");
            SHA.SendNews(NewsCategory.Magazine, "Kate Winslet itiraf etti:", "Tittanik en iyi filmimdi!");
            SHA.SendNews(NewsCategory.Magazine, "Ayda su bulunmuş!", "Ama kuyu suyuymuş!");

            k19.Publish();
            k20.Publish();
        }

        public static void TestPrototype()
        {
            GameSceneManager manager = new GameSceneManager();
            manager.Test();
        }

        public static void TestDecorator()
        {
            // Bileşen örneklenir
            Artillery azman = new Artillery(125, 40, "Fırtına A1");
            azman.Fire();

            // Decorator nesnesi örneklenir
            ArtilleryDecorator azmanDekorator = new ArtilleryDecorator(azman);
            // Decorator nesnesi üzerinden o anki asıl Component için(Artillery sınıfı) ek fonksiyonellikler çağırılır.
            azmanDekorator.Defense();
            azmanDekorator.Fire();
            azmanDekorator.Easy();
            azmanDekorator.Fire();
        }
    }
}
