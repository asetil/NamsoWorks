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


using System;

namespace CleanCode.DesignPattern
{
    //abstract tanımlamalar
    public interface IConnection
    {
        void Connect();
        void Disconnect();
    }

    public interface ICommand
    {
        void Execute();
    }

    public abstract class DatabaseFactory
    {
        public abstract IConnection CreateConnection();
        public abstract ICommand CreateCommand();
    }

    //MSSQL Tanımlamaları
    public class MSSQLConnection : IConnection
    {
        public void Connect()
        {
            Console.WriteLine("MSSQL baglantisi kuruldu.");
        }
        public void Disconnect()
        {
            Console.WriteLine("MSSQL baglantisi sonlandırıldı.");
        }
    }

    public class MSSQLCommand : ICommand
    {
        public void Execute()
        {
            Console.WriteLine("MSSQL sorgusu calistiriliyor.");
        }
    }

    public class MSSQLFactory : DatabaseFactory
    {
        public override IConnection CreateConnection()
        {
            return (new MSSQLConnection());
        }
        public override ICommand CreateCommand()
        {
            return (new MSSQLCommand());
        }
    }

    //Oracle Tanımlamaları
    public class OracleConnection : IConnection
    {
        public void Connect()
        {
            Console.WriteLine("Oracle baglantisi kuruldu.");
        }
        public void Disconnect()
        {
            Console.WriteLine("Oracle baglantisi sonlandırıldı.");
        }
    }

    public class OracleCommand : ICommand
    {
        public void Execute()
        {
            Console.WriteLine("Oracle sorgusu calistiriliyor.");
        }
    }

    public class OracleFactory : DatabaseFactory
    {
        public override IConnection CreateConnection()
        {
            return (new OracleConnection());
        }
        public override ICommand CreateCommand()
        {
            return (new OracleCommand());
        }
    }

    public class AbstractFactoryApplicationClass
    {
        private DatabaseFactory _databaseFactory;
        private IConnection _connection;
        private ICommand _command;
        public AbstractFactoryApplicationClass(DatabaseFactory databaseFactory)
        {
            _databaseFactory = databaseFactory;
            _connection = databaseFactory.CreateConnection();
            _command = databaseFactory.CreateCommand();
        }
        public void Connect()
        {
            _connection.Connect();
        }
        public void Disconnect()
        {
            _connection.Disconnect();
        }
        public void Execute()
        {
            _command.Execute();
        }
    }
}
