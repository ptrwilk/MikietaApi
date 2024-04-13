using System.Data;
using Dapper;

namespace MikietaApi.Repositories;

public interface IIngredientsRepository : IRepository
{
    
}

public class IngredientsRepository : IIngredientsRepository
{
    private const string TableName = "Ingredients";
    private readonly IDbConnection _connection;

    public IngredientsRepository(IDbConnection connection)
    {
        _connection = connection;
    }
    
    public void CreateTableIfNotExists()
    {
        var sql = $@"
                CREATE TABLE IF NOT EXISTS {TableName} (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL
                )";

        _connection.Execute(sql);
    }
}