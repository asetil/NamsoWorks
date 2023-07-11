using System;
using System.Data;
using System.Data.SqlClient;
using Aware.Util;

namespace Aware.Data.SQL
{
    public class SqlHelper : ISqlHelper
    {
        public int Execute(string command)
        {
            try
            {
                var sqlConnection = Connect();
                var query = new SqlCommand(command, sqlConnection);
                var result = query.ExecuteNonQuery();

                query.Dispose();
                Disconnect(sqlConnection);
                return result;
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message + " (" + command + ")");
            }
        }

        public DataTable GetDataTable(string sql)
        {
            var dt = new DataTable();

            try
            {
                var sqlConnection = Connect();
                var adapter = new SqlDataAdapter(sql, sqlConnection);
                adapter.Fill(dt);
                adapter.Dispose();

                Disconnect(sqlConnection);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message + " (" + sql + ")");
            }
            return dt;
        }

        public DataSet GetDataset(string sql)
        {
            var ds = new DataSet();
            try
            {
                var sqlConnection = Connect();
                var adapter = new SqlDataAdapter(sql, sqlConnection);
                adapter.Fill(ds);
                adapter.Dispose();
                Disconnect(sqlConnection);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message + " (" + sql + ")");
            }
            return ds;
        }

        public DataRow GetDataRow(string sql)
        {
            var table = GetDataTable(sql);
            return table.Rows.Count == 0 ? null : table.Rows[0];
        }

        public string GetDataCell(string sql)
        {
            var table = GetDataTable(sql);
            return table.Rows.Count == 0 ? null : table.Rows[0][0].ToString();
        }

        public string RemoveInjection(string value)
        {
            value = value.ToLowerInvariant();
            value = value.Replace("'", "");
            value = value.Replace("--", "");
            value = value.Replace(";", "");
            value = value.Replace("(", "");
            value = value.Replace(")", "");
            value = value.Replace("waitfor", "");
            value = value.Replace("delay", "");
            value = value.Replace("=", "");
            value = value.Replace("&gt;", "");
            value = value.Replace("&lt;", "");
            value = value.Replace("char ", "");
            value = value.Replace("delete ", "");
            value = value.Replace("insert ", "");
            value = value.Replace("update ", "");
            value = value.Replace("select ", "");
            value = value.Replace("truncate ", "");
            value = value.Replace("union ", "");
            value = value.Replace("script ", "");
            value = value.Replace("*", "");
            return value;
        }

        public void SetDateFormat(string format = "DMY")
        {
            Execute("Set DateFormat " + format);
        }

        public QueryHelper Query { get { return new QueryHelper(); } }

        private SqlConnection Connect()
        {
            var sqlConnection = new SqlConnection(Config.ConnectionString);
            sqlConnection.Open();
            return sqlConnection;
        }

        private void Disconnect(SqlConnection sqlConnection)
        {
            if (sqlConnection == null) throw new ArgumentNullException("sqlConnection");

            sqlConnection.Close();
            sqlConnection.Dispose();
        }
    }

    public class QueryHelper
    {
        private string _table;
        private string _where;
        public QueryHelper Table<T>()
        {
            _table = typeof (T).Name;
            return this;
        }
        public QueryHelper Where(string where)
        {
            _where = where;
            return this;
        }

        public override string ToString()
        {
            var sql = string.Format("SELECT * FROM {0}", _table);
            if (!string.IsNullOrEmpty(_where))
            {
                sql += string.Format(" WHERE {0}", _where);
            }
            return sql;
        }
        
    }
}
