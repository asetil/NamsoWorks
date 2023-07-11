using System.Collections.Generic;

namespace CleanCode.DesignPattern.Observer.Base
{
    //Haber Ajansı haberleri kanallara dağıtır
    public abstract class Publisher
    {
        List<ISubscriber> _subscribers;
        News _news;
        string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public News News
        {
            get { return _news; }
        }

        internal List<ISubscriber> Subscribers
        {
            get { return _subscribers; }
        }

        private void NotifySubscribers() {

            foreach (var subscriber in this.Subscribers)
            {
                subscriber.Update(this);
            }
        }

        public void SendNews(NewsCategory category, string title, string content) {
            this._news = new News(this, category, title, content);
            this.NotifySubscribers();
        }

        public Publisher(string name) {
            this._name = name;
            this._news = null;
            this._subscribers = new List<ISubscriber>();
        }

        internal void AddSubscriber(ISubscriber subscriber) {
            this.Subscribers.Add(subscriber);
        }

        internal void RemoveSubscriber(ISubscriber subscriber)
        {
            this.Subscribers.Remove(subscriber);
        }
    }
}
