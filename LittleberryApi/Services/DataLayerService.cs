using System.Data;
using Microsoft.Data.SqlClient;

namespace LittleberryApi.Services;

public class DataLayerService : IDataLayerService
{
    private readonly string _connectionString;
    private readonly ILogger<DataLayerService> _logger;

    public DataLayerService(IConfiguration configuration, ILogger<DataLayerService> logger)
    {
        _connectionString = configuration.GetConnectionString("server18") + ";Connection Timeout=10;";
        _logger = logger;
    }

    public async Task<DataSet> GetDataAsync(string sql)
    {
        var ds = new DataSet();

        try
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);
            command.CommandType = CommandType.Text;

            using var adapter = new SqlDataAdapter(command);
            adapter.Fill(ds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing SQL: {Sql}", sql);
        }

        return ds;
    }

    public async Task<int> RunSqlAsync(string sql)
    {
        try
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);
            command.CommandType = CommandType.Text;

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing SQL: {Sql}", sql);
            return -1;
        }
    }

    public async Task<string> RunSqlStringAsync(string sql)
    {
        try
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);
            command.CommandType = CommandType.Text;

            var result = await command.ExecuteScalarAsync();
            return result?.ToString() ?? "";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing SQL: {Sql}", sql);
            return "";
        }
    }
}
