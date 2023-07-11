using System;
using System.Collections.Generic;
using CleanCode.DesignPattern.Observer.Base;

namespace CleanCode.DesignPattern.Observer.Subscribers
{
    public class Kanal20 : ISubscriber
    {

        List<News> _news;
        public List<News> News
        {
            get
            {
                if (this._news == null) { this._news = new List<News>(); }
                return _news;
            }
        }

        public void Update(Publisher publisher)
        {
            this.News.Add(publisher.News);
        }

        public void Publish()
        {
            foreach (var item in News)
            {
                Console.WriteLine("{0} tatafından bildirildi\n{1}\n{2}\n", item.Publisher.Name, item.Title, item.Content);
            }
        }

    }
}
