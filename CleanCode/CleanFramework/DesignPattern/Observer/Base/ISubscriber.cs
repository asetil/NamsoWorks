using System.Collections.Generic;

namespace CleanCode.DesignPattern.Observer.Base
{
    //Kanal haber ajansından gelen haberi yayınlar
    public interface ISubscriber
    {
        List<News> News { get; }
        void Update(Publisher publisher);
        void Publish();
    }
}
