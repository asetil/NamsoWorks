using Aware.BL.Model;
using Aware.Data;
using Aware.Manager;
using Aware.Search;
using Aware.Util.Enum;
using Aware.Util.Log;
using Cinescope.Web.Models;
using System.Linq;

namespace Cinescope.Manager
{
    public class FilmManager : BaseManager<Film>, IFilmManager
    {
        private readonly IRepository<PlayerFilm> _playerFilmRepository;
        public FilmManager(IRepository<Film> repository, IRepository<PlayerFilm> playerFilmRepository, IAwareLogger logger) : base(repository, logger)
        {
            _playerFilmRepository = playerFilmRepository;
        }

        public SearchResult<Film> GetFilms()
        {
            var filmSearchResult = SearchBy(p => p.Status == StatusType.Active);
            if (filmSearchResult.HasResult)
            {
                var searchParams = new SearchParams<PlayerFilm>();
                var filmIds = filmSearchResult.Results.Select(p => p.ID).ToList();
                searchParams.FilterBy(p => filmIds.Contains(p.FilmId));
                searchParams.NavigationFields = "Player";

                var playerInfoList = _playerFilmRepository.Find(searchParams).ToList();
                filmSearchResult.Results = filmSearchResult.Results.Select(item =>
                {
                    item.Players = playerInfoList.Where(p => p.FilmId == item.ID).Select(p => p.Player).ToList();
                    return item;
                }).ToList();
            }

            return filmSearchResult;
        }

        protected override OperationResult<Film> OnBeforeUpdate(ref Film existing, Film model)
        {
            throw new System.NotImplementedException();
        }
    }
}
