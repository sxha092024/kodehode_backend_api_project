using Dapper;
using Microsoft.Data.Sqlite;
using System.Data;

public class DapperContext
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    private readonly ILogger _logger;

    public DapperContext(IConfiguration configuration, ILogger<DapperContext> logger)
    {
        _configuration = configuration;
        _logger = logger;
        if (_configuration.GetConnectionString("SqlConnection") is string connectionString)
        {
            _connectionString = connectionString;
        }
        else
        {
            throw new ArgumentException("SqlConnection must be a valid string");
        }
        Task.Run(this.Init);
    }

    public IDbConnection CreateConnection()
    {
        
        var connection = new SqliteConnection(_connectionString);
        // TODO: add UUID/GUID extension?
        return connection;
    }

    public async Task Init()
    {
        SqlMapper.AddTypeHandler(new SqliteGuidTypeHandler());
        using var connection = CreateConnection();
        await _initTables();

        async Task _initTables()
        {
            // TODO: use migrations instead? Either through migrations/*.sql or a C# ecosystem manager?
            var tableSql = """
                CREATE TABLE IF NOT EXISTS
                UploadTable(
                    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    Data BLOB NOT NULL,
                    ContentType TEXT NOT NULL,
                    Name TEXT,
                    Guid VARCHAR(36) NOT NULL
                );
            """;
            try {

            var res = await connection.ExecuteAsync(tableSql);
            }
            catch (Exception exc)
            {
                _logger.LogError("{@exc}", exc);
                throw;
            }
        }
    }
}