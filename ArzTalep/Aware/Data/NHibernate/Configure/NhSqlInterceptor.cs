//using System.Collections.Generic;
//using NHibernate;
//using NHibernate.SqlCommand;
//using Worchart.BL.Log;

//namespace Worchart.Data.NHibernate.Configure
//{
//    public class NhSqlInterceptor : EmptyInterceptor, IInterceptor
//    {
//        public static List<string> NHibernateSql { get; set; }
//        private readonly ILogger _logger;

//        public NhSqlInterceptor(ILogger logger)
//        {
//            _logger = logger;
//        }

//        SqlString IInterceptor.OnPrepareStatement(SqlString sql)
//        {
//            var formattedSql = FormatSql(sql.ToString());
//            //NHibernateSql = NHibernateSql ?? new List<string>();
//            //NHibernateSql.Add(formattedSql);

//            _logger.Warn("NhSqlInterceptor|SqlRun", formattedSql, false);
//            return sql;
//        }

//        private static string FormatSql(string unformattedSql)
//        {
//            string newSql = unformattedSql.Replace("SELECT", "SELECT\n\t");
//            newSql = newSql.Replace("FROM", "\nFROM\n\t");
//            newSql = newSql.Replace("WHERE", "\nWHERE\n\t");
//            newSql = newSql.Replace("=", " = ");
//            newSql = newSql.Replace(",", ",\n\t");
//            newSql = newSql.Replace(" AND", " AND\n\t");
//            newSql = newSql.Replace(" ON", "\n\t\tON");
//            newSql = newSql.Replace("INNER JOIN", "\n\tINNER JOIN");
//            newSql = newSql.Replace("ORDER BY", "\nORDER\t BY");
//            newSql = newSql.Replace("GROUP BY", "\nGROUP\t BY");
//            newSql = newSql.Replace(" AS", " \tAS");
//            return newSql;
//        }
//    }
//}