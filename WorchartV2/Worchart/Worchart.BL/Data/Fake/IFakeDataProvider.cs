using System.Collections.Generic;

namespace Worchart.Data.Fake
{
    public interface IFakeDataProvider
    {
        IEnumerable<T> GetFakeData<T>();
    }
}