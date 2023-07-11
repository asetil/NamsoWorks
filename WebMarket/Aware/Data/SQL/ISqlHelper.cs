using System.Data;

namespace Aware.Data.SQL
{
    public interface ISqlHelper
    {
        int Execute(string command);
        DataTable GetDataTable(string sql);
        DataSet GetDataset(string sql);
        DataRow GetDataRow(string sql);
        string GetDataCell(string sql);
        string RemoveInjection(string value);
        void SetDateFormat(string format = "DMY");
        QueryHelper Query { get; }
    }
}