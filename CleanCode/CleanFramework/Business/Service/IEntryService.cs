using CleanFramework.Business.Model;
using Aware.Util.Model;
using Aware.Search;

namespace CleanFramework.Business.Service
{
    public interface IEntryService
    {
        SearchResult<Entry> Search(EntrySearchParams searchParams, bool loadCategories, bool loadAuthor);
        EntryListModel GetEntryList(EntrySearchParams searchParams);
        EntryDisplayModel GetEntryDisplayModel(int id);
        EntryDetailModel GetEntryDetail(int id, int authorID);
        Result Save(int userID, Entry model);
        Result Delete(int userID, int entryID);
    }
}
