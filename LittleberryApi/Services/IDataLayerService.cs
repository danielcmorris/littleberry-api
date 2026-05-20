using System.Data;

namespace LittleberryApi.Services;

public interface IDataLayerService
{
    Task<DataSet> GetDataAsync(string sql);
    Task<int> RunSqlAsync(string sql);
    Task<string> RunSqlStringAsync(string sql);
}
