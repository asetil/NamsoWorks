using System;
using System.Collections.Generic;
using System.Linq;

namespace Aware.ECommerce.Model.Custom
{
    public class CommentViewModel
    {
        private double _ratingAverage =-1;
        public string Title { get; set; }
        public int RelationID { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
        public bool AllowNewComment { get; set; }
        public Dictionary<int, int> Stats { get; set; }
        public int CommentCount { get; set; }
        public bool IsPartial { get; set; }

        public double RatingAverage
        {
            get
            {
                if (_ratingAverage==-1 && Stats != null)
                {
                    var ratingTotal = Stats.Where(i => i.Key <= 5).Sum(i => i.Key * i.Value * 1.0);
                    var voteCount = Stats.Where(i => i.Key <= 5 && i.Key > 0).Sum(i => i.Value * 1.0);
                    var result = voteCount > 0 ? Math.Round(ratingTotal / voteCount, 1) : 0;
                    return result;
                }
                return _ratingAverage==-1 ? 0 : _ratingAverage;
            }
        }
    }
}
