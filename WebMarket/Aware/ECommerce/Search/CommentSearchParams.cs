using System.Linq;
using Aware.ECommerce.Model;
using Aware.Search;
using Aware.ECommerce.Enums;

namespace Aware.ECommerce.Search
{
    public class CommentSearchParams : SearchParams<Comment>
    {
        public int Rating { get; set; }
        public int RelationType { get; set; }
        public CommentStatus CommentStatus { get; set; }

        public override SearchHelper<Comment> PrepareFilters()
        {
            var searchHelper = SearchHelper;
            if (!string.IsNullOrEmpty(Keyword))
            {
                searchHelper.FilterBy(i => i.Title.ToLower().Contains(Keyword.ToLower()) || i.Value.ToLower().Contains(Keyword.ToLower()));
            }

            if (IDs != null && IDs.Any())
            {
                searchHelper.FilterBy(i => IDs.Contains(i.ID) || IDs.Contains(i.RelationID));
            }

            if (Rating > 0)
            {
                searchHelper.FilterBy(i => i.Rating == Rating);
            }

            if (CommentStatus != CommentStatus.None)
            {
                searchHelper.FilterBy(i => i.Status == CommentStatus);
            }
            return searchHelper;
        }
    }
}