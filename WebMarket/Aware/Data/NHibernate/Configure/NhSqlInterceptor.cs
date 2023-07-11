using System.Collections.Generic;
using Aware.Dependency;
using Aware.Util.Log;
using NHibernate;
using NHibernate.SqlCommand;

namespace Aware.Data.NHibernate.Configure
{
    public class NhSqlInterceptor : EmptyInterceptor, IInterceptor
    {
        public static List<string> NHibernateSql { get; set; }
        SqlString IInterceptor.OnPrepareStatement(SqlString sql)
        {
            var formattedSql = FormatSql(sql.ToString());
            //NHibernateSql = NHibernateSql ?? new List<string>();
            //NHibernateSql.Add(formattedSql);

            var logger = WindsorBootstrapper.Resolve<ILogger>();
            logger.Warn("SQL : " + formattedSql, string.Empty);
            return sql;
        }

        private static string FormatSql(string unformattedSql)
        {
            string newSql = unformattedSql.Replace("SELECT", "SELECT\n\t");
            newSql = newSql.Replace("FROM", "\nFROM\n\t");
            newSql = newSql.Replace("WHERE", "\nWHERE\n\t");
            newSql = newSql.Replace("=", " = ");
            newSql = newSql.Replace(",", ",\n\t");
            newSql = newSql.Replace(" AND", " AND\n\t");
            newSql = newSql.Replace(" ON", "\n\t\tON");
            newSql = newSql.Replace("INNER JOIN", "\n\tINNER JOIN");
            newSql = newSql.Replace("ORDER BY", "\nORDER\t BY");
            newSql = newSql.Replace("GROUP BY", "\nGROUP\t BY");
            newSql = newSql.Replace(" AS", " \tAS");
            return newSql;
        }
    }
}