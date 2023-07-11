using NHibernate;
using Worchart.BL.Enum;

namespace Worchart.Search
{
    public interface ICriteriaHelper
    {
        bool LoadCount { get; }
        int Count { get; set; }

        ICriteriaHelper Init(int page = 0, int size = 0, int timeout = 0);
        ICriteriaHelper WithCount();
        ICriteriaHelper WithAlias(string table, string alias, JoinMode joinMode = JoinMode.InnerJoin);
        ICriteriaHelper OrderBy(string fieldName, bool ascending = true);
        ICriteriaHelper Eq(string fieldName, object value);
        ICriteriaHelper Greater(string fieldName, object value, bool checkEqual = false);
        ICriteriaHelper Lower(string fieldName, object value, bool checkEqual = false);
        ICriteriaHelper Like(string fieldName, object value);
        ICriteriaHelper In(string fieldName, object[] value);
        void Build(ref ICriteria criteria);
    }
}