using System.Collections.Generic;
using System.Linq;
using Aware.ECommerce.Model;
using Aware.Search;
using Aware.Util.Lookup;
using Aware.Util.Model;

namespace WebMarket.Admin.Models
{
    public class CommentListModel
    {
        public SearchResult<Comment> SearchResult { get; set; }
        public IEnumerable<Item> ItemList { get; set; }
        public List<Lookup> RaitingStarList { get; set; }
        public List<Lookup> CommentStatusList { get; set; }
        public bool AllowEdit { get; set; }

        public Item GetItem(Comment comment)
        {
            Item result = null;
            if (comment != null)
            {
                result = ItemList.FirstOrDefault(i => i.ID == comment.RelationID);
            }
            return result ?? new Item(0, string.Empty);
        }
    }
}