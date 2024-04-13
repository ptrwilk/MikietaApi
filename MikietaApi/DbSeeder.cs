using System.Data;
using Dapper;
using MikietaApi.Repositories;

namespace MikietaApi;

public class DbSeeder
{
    private readonly IDbConnection _connection;
    private readonly IProductsRepository _productsRepository;
    private readonly IIngredientsRepository _ingredientsRepository;

    public DbSeeder(
        IDbConnection connection,
        IProductsRepository productsRepository,
        IIngredientsRepository ingredientsRepository)
    {
        _connection = connection;
        _productsRepository = productsRepository;
        _ingredientsRepository = ingredientsRepository;
    }
    
    public void Seed()
    {
        _productsRepository.CreateTableIfNotExists();
        _ingredientsRepository.CreateTableIfNotExists();
        CreateProductIngredientTable();
    }

    private void CreateProductIngredientTable()
    {
        var sql = $@"
                CREATE TABLE IF NOT EXISTS ProductIngredient(
                    ProductId INTEGER NOT NULL,
                    IngredientId INTEGER NOT NULL,
                    FOREIGN KEY (ProductId) REFERENCES Product (Id),
                    FOREIGN KEY (IngredientId) REFERENCES Ingredient (Id),
                    PRIMARY KEY (ProductId, IngredientId)
                )";

        _connection.Execute(sql);        
    }
}