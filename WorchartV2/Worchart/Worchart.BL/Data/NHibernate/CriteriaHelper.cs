using System.Collections.Generic;
using Worchart.Search;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using Worchart.BL.Enum;

namespace Worchart.Data.NHibernate
{
    public class CriteriaHelper : ICriteriaHelper
    {
        private readonly List<CriteriaAlias> _aliasList;
        private readonly List<SimpleExpression> _expressions;
        private readonly List<AbstractCriterion> _abstractCriterias;
        private readonly List<KeyValuePair<string, bool>> _orderByList;
        private int _page = 0;
        private int _size = 0;
        private int _timeout = 0; //second

        public CriteriaHelper()
        {
            _aliasList = new List<CriteriaAlias>();
            _expressions = new List<SimpleExpression>();
            _abstractCriterias = new List<AbstractCriterion>();
            _orderByList = new List<KeyValuePair<string, bool>>();
        }

        public bool LoadCount { get; private set; }
        public int Count { get; set; }

        public ICriteriaHelper Init(int page = 0, int size = 0, int timeout = 0)
        {
            _page = page;
            _size = size;
            _timeout = timeout;
            return this;
        }

        public ICriteriaHelper WithCount()
        {
            LoadCount = true;
            return this;
        }

        public ICriteriaHelper WithAlias(string table, string alias, JoinMode joinMode = JoinMode.InnerJoin)
        {
            var criteriaAlias = new CriteriaAlias(table, alias, joinMode);
            _aliasList.Add(criteriaAlias);
            return this;
        }

        public ICriteriaHelper OrderBy(string fieldName, bool ascending = true)
        {
            if (!string.IsNullOrEmpty(fieldName))
            {
                var orderBy = new KeyValuePair<string, bool>(fieldName, ascending);
                _orderByList.Add(orderBy);
            }
            return this;
        }

        public ICriteriaHelper Eq(string fieldName, object value)
        {
            if (!string.IsNullOrEmpty(fieldName))
            {
                var expression = Restrictions.Eq(fieldName, value);
                _expressions.Add(expression);
            }
            return this;
        }

        public ICriteriaHelper Greater(string fieldName, object value, bool checkEqual = false)
        {
            if (!string.IsNullOrEmpty(fieldName))
            {
                var expression = checkEqual ? Restrictions.Ge(fieldName, value) : Restrictions.Gt(fieldName, value);
                _expressions.Add(expression);
            }
            return this;
        }

        public ICriteriaHelper Lower(string fieldName, object value, bool checkEqual = false)
        {
            if (!string.IsNullOrEmpty(fieldName))
            {
                var expression = checkEqual ? Restrictions.Le(fieldName, value) : Restrictions.Lt(fieldName, value);
                _expressions.Add(expression);
            }
            return this;
        }

        public ICriteriaHelper Like(string fieldName, object value)
        {
            if (!string.IsNullOrEmpty(fieldName))
            {
                var expression = Restrictions.Like(fieldName, value);
                _expressions.Add(expression);
            }
            return this;
        }

        public ICriteriaHelper In(string fieldName, object[] value)
        {
            if (!string.IsNullOrEmpty(fieldName))
            {
                var abstractCriteria = Restrictions.In(fieldName, value);
                _abstractCriterias.Add(abstractCriteria);
            }
            return this;
        }

        public void Build(ref ICriteria criteria)
        {
            foreach (var criteriaAliase in _aliasList)
            {
                var joinCode = (int)criteriaAliase.JoinMode;
                criteria.CreateAlias(criteriaAliase.TableName, criteriaAliase.Alias, (JoinType)joinCode);
            }

            foreach (var expresion in _expressions)
            {
                criteria.Add(expresion);
            }

            foreach (var abstractCritera in _abstractCriterias)
            {
                criteria.Add(abstractCritera);
            }

            foreach (var orderBy in _orderByList)
            {
                criteria.AddOrder(new Order(orderBy.Key, orderBy.Value));
            }

            if (_page >= 0 && _size > 0)
            {
                criteria.SetFirstResult(_page * _size);
                criteria.SetMaxResults((_page + 1) * _size);
            }

            if (_timeout > 0)
            {
                criteria.SetTimeout(_timeout);
            }
        }
    }

    public class CriteriaAlias
    {
        public CriteriaAlias(string table, string alias, JoinMode joinMode = JoinMode.InnerJoin)
        {
            TableName = table;
            Alias = alias;
            JoinMode = joinMode;
        }

        public string TableName { get; set; }
        public string Alias { get; set; }
        public JoinMode JoinMode { get; set; }
    }
}
