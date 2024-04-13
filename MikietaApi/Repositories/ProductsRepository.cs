using System.Data;
using Dapper;

namespace MikietaApi.Repositories;

public interface IProductsRepository : IRepository
{
    
}

public class ProductsRepository : IProductsRepository
{
    private const string TableName = "Products";
    private readonly IDbConnection _connection;

    public ProductsRepository(IDbConnection connection)
    {
        _connection = connection;
    }
    
    public void CreateTableIfNotExists()
    {
        var sql = $@"
                CREATE TABLE IF NOT EXISTS {TableName} (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Price REAL NOT NULL,
                    ProductType INTEGER NOT NULL
                )";

        _connection.Execute(sql);
    }
}