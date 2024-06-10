using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MikietaApi.Data;
using MikietaApi.Data.Entities;
using MikietaApi.Models;
using MikietaApi.Services;
using MikietaApi.Stripe;
using Shouldly;

namespace MikietaApi.Tests.Services;

public class OrderServiceTest
{
    private IOrderService _orderService = null!;
    private DataContext _dbContext = null!;

    [SetUp]
    public void SetUp()
    {
        var factory = WebAppFactory.CreateFactory(services => { });
        
        var provider = factory.Services.CreateScope().ServiceProvider;

        var context = provider.GetRequiredService<IHttpContextAccessor>();
        context.HttpContext = new DefaultHttpContext();
        
        _orderService = provider.GetRequiredService<IOrderService>();

        _dbContext = provider.GetRequiredService<DataContext>();

        _dbContext.Database.EnsureDeleted();
        
        _dbContext.Database.Migrate();

        var ingredients = new List<IngredientEntity>
        {
            new ()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                Name = "Ingredient1"
            },
            new ()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                Name = "Ingredient2"
            },
            new ()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                Name = "Ingredient3"
            },
            new ()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000004"),
                Name = "Ingredient4"
            }
        };
        _dbContext.Ingredients.AddRange(ingredients);

        _dbContext.Products.Add(new ProductEntity
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            Name = "Pizza1",
            Ingredients = new List<IngredientEntity>
            {
                ingredients.First(x => x.Id == Guid.Parse("10000000-0000-0000-0000-000000000001")),
                ingredients.First(x => x.Id == Guid.Parse("10000000-0000-0000-0000-000000000002"))
            }
        });
        
        _dbContext.Products.Add(new ProductEntity
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
            Name = "Pizza2",
            Ingredients = new List<IngredientEntity>
            {
                ingredients.First(x => x.Id == Guid.Parse("10000000-0000-0000-0000-000000000001")),
                ingredients.First(x => x.Id == Guid.Parse("10000000-0000-0000-0000-000000000003"))
            }
        });
        
        _dbContext.Products.Add(new ProductEntity
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
            Name = "Pizza3",
            Ingredients = new List<IngredientEntity>
            {
                ingredients.First(x => x.Id == Guid.Parse("10000000-0000-0000-0000-000000000001")),
                ingredients.First(x => x.Id == Guid.Parse("10000000-0000-0000-0000-000000000003"))
            }
        });

        _dbContext.SaveChanges();
    }

    private static IEnumerable<TestCaseData> CreateOrderedProducts_Cases()
    {
        yield return new TestCaseData(new OrderModel
        {
            ProductQuantities = new ProductQuantityModel[]
            {
                new()
                {
                    Quantity = 1,
                    ProductId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    AdditionalIngredients = new []
                    {
                        new AdditionalIngredientModel
                        {
                            Quantity = 2,
                            IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000001")
                        }
                    }
                }
            }
        }, new OrderedProductEntity[]
        {
            new ()
            {
                ProductId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Name = "Pizza1",
                OrderedProductOrderedIngredients = new OrderedProductOrderedIngredientEntity[]
                {
                    new ()
                    {
                        OrderedIngredient = new OrderedIngredientEntity()
                        {
                            IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                            Name = "Ingredient1"
                        },
                        Quantity = 1
                    },
                    new ()
                    {
                        OrderedIngredient = new OrderedIngredientEntity()
                        {
                            IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                            Name = "Ingredient2"
                        },
                        Quantity = 1
                    },
                    new ()
                    {
                        OrderedIngredient = new OrderedIngredientEntity()
                        {
                            IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                            Name = "Ingredient1"
                        },
                        IsAdditionalIngredient = true,
                        Quantity = 2
                    }
                }
            },
        }).SetName("CreateOrderedProducts 01");
        
        yield return new TestCaseData(new OrderModel
        {
            ProductQuantities = new ProductQuantityModel[]
            {
                new()
                {
                    Quantity = 1,
                    ProductId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    RemovedIngredients = new []
                    {
                        new RemovedIngredientModel
                        {
                            IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000001")
                        }
                    }
                }
            }
        }, new OrderedProductEntity[]
        {
            new ()
            {
                ProductId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Name = "Pizza1",
                OrderedProductOrderedIngredients = new OrderedProductOrderedIngredientEntity[]
                {
                    new ()
                    {
                        OrderedIngredient = new OrderedIngredientEntity()
                        {
                            IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                            Name = "Ingredient2"
                        },
                        Quantity = 1
                    },
                    new ()
                    {
                        OrderedIngredient = new OrderedIngredientEntity()
                        {
                            IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                            Name = "Ingredient1"
                        },
                        Quantity = 1,
                        IsIngredientRemoved = true
                    },
                }
            }
        }).SetName("CreateOrderedProducts 02");
        
        yield return new TestCaseData(new OrderModel
        {
            ProductQuantities = new ProductQuantityModel[]
            {
                new()
                {
                    Quantity = 1,
                    ProductId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    ReplacedIngredients = new []
                    {
                        new ReplacedIngredientModel()
                        {
                            FromIngredientId = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                            ToIngredientId = Guid.Parse("10000000-0000-0000-0000-000000000002")
                        }
                    }
                }
            }
        }, new OrderedProductEntity[]
        {
            new ()
            {
                ProductId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Name = "Pizza1",
                OrderedProductOrderedIngredients = new OrderedProductOrderedIngredientEntity[]
                {
                    new ()
                    {
                        OrderedIngredient = new OrderedIngredientEntity()
                        {
                            IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                            Name = "Ingredient2"
                        },
                        Quantity = 1
                    },
                    new ()
                    {
                        OrderedIngredient = new OrderedIngredientEntity()
                        {
                            IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                            Name = "Ingredient1"
                        },
                        Quantity = 1,
                        ReplacedIngredient = new OrderedIngredientEntity()
                        {
                            IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                            Name = "Ingredient2"
                        }
                    },
                }
            },
        }).SetName("CreateOrderedProducts 03");
        
        yield return new TestCaseData(new OrderModel
        {
            ProductQuantities = new ProductQuantityModel[]
            {
                new()
                {
                    Quantity = 1,
                    ProductId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    AdditionalIngredients = new AdditionalIngredientModel[]
                    {
                      new ()
                      {
                          Quantity = 1,
                          IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000004")
                      }  
                    },
                    RemovedIngredients = new RemovedIngredientModel[]
                    {
                        new ()
                        {
                            IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000001")
                        }
                    },
                    ReplacedIngredients = new []
                    {
                        new ReplacedIngredientModel()
                        {
                            FromIngredientId = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                            ToIngredientId = Guid.Parse("10000000-0000-0000-0000-000000000003")
                        }
                    }
                },
                new()
                {
                    Quantity = 1,
                    ProductId = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                    RemovedIngredients = new RemovedIngredientModel[]
                    {
                        new ()
                        {
                            IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000001")
                        },
                        new ()
                        {
                            IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000003")
                        }
                    },
                },
                new()
                {
                    Quantity = 2,
                    ProductId = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                    AdditionalIngredients = new AdditionalIngredientModel[]
                    {
                        new ()
                        {
                            Quantity = 2,
                            IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000004")
                        }  
                    },
                    ReplacedIngredients = new []
                    {
                        new ReplacedIngredientModel()
                        {
                            FromIngredientId = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                            ToIngredientId = Guid.Parse("10000000-0000-0000-0000-000000000002")
                        },
                        new ReplacedIngredientModel()
                        {
                            FromIngredientId = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                            ToIngredientId = Guid.Parse("10000000-0000-0000-0000-000000000001")
                        }
                    }
                },
            }
        }, new OrderedProductEntity[]
        {
            new ()
            {
                ProductId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Name = "Pizza1",
                OrderedProductOrderedIngredients = new OrderedProductOrderedIngredientEntity[]
                {
                    new ()
                    {
                        OrderedIngredient = new OrderedIngredientEntity()
                        {
                            IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000004"),
                            Name = "Ingredient4"
                        },
                        IsAdditionalIngredient = true,
                        Quantity = 1
                    },
                    new ()
                    {
                        OrderedIngredient = new OrderedIngredientEntity()
                        {
                            IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                            Name = "Ingredient1"
                        },
                        IsIngredientRemoved = true,
                        Quantity = 1
                    },
                    new ()
                    {
                        OrderedIngredient = new OrderedIngredientEntity()
                        {
                            IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                            Name = "Ingredient2"
                        },
                        Quantity = 1,
                        ReplacedIngredient = new OrderedIngredientEntity()
                        {
                            IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                            Name = "Ingredient3"
                        }
                    },
                }
            },
            new ()
            {
                ProductId = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Name = "Pizza2",
                OrderedProductOrderedIngredients = new OrderedProductOrderedIngredientEntity[]
                {
                    new ()
                    {
                        OrderedIngredient = new OrderedIngredientEntity()
                        {
                            IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                            Name = "Ingredient1"
                        },
                        IsIngredientRemoved = true,
                        Quantity = 1
                    },
                    new ()
                    {
                        OrderedIngredient = new OrderedIngredientEntity()
                        {
                            IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                            Name = "Ingredient3"
                        },
                        IsIngredientRemoved = true,
                        Quantity = 1
                    },
                }
            },
            new ()
            {
                ProductId = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                Name = "Pizza3",
                OrderedProductOrderedIngredients = new OrderedProductOrderedIngredientEntity[]
                {
                    new ()
                    {
                        OrderedIngredient = new OrderedIngredientEntity()
                        {
                            IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000004"),
                            Name = "Ingredient4"
                        },
                        IsAdditionalIngredient = true,
                        Quantity = 2
                    },
                    new ()
                    {
                        OrderedIngredient = new OrderedIngredientEntity()
                        {
                            IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                            Name = "Ingredient1"
                        },
                        Quantity = 1,
                        ReplacedIngredient = new OrderedIngredientEntity()
                        {
                            IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                            Name = "Ingredient2"
                        }
                    },
                    new ()
                    {
                        OrderedIngredient = new OrderedIngredientEntity()
                        {
                            IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                            Name = "Ingredient3"
                        },
                        Quantity = 1,
                        ReplacedIngredient = new OrderedIngredientEntity()
                        {
                            IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                            Name = "Ingredient1"
                        }
                    },
                }
            },
        }).SetName("CreateOrderedProducts 04");
    }

    [TestCaseSource(nameof(CreateOrderedProducts_Cases))]
    public void CreateOrderedProducts(OrderModel model, OrderedProductEntity[] expectedOrderedProductEntity)
    {
        var res = Helpers.InvokePrivateMethod<OrderedProductEntity[]>(_orderService, "CreateOrderedProducts", model);
        
        res.ShouldBeEquivalentTo(expectedOrderedProductEntity);
    }
}