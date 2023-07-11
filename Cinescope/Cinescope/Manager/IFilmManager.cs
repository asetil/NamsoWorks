using Aware.Manager;
using Aware.Search;
using Cinescope.Web.Models;

namespace Cinescope.Manager
{
    public interface IFilmManager : IBaseManager<Film>
    {
        SearchResult<Film> GetFilms();
    }
}
