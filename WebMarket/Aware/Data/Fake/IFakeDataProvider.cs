using System.Collections.Generic;

namespace Aware.Data.Fake
{
    public interface IFakeDataProvider
    {
        IEnumerable<T> GetFakeData<T>();
    }
}