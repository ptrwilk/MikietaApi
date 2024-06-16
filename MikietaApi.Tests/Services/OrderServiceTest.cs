using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MikietaApi.Data;
using MikietaApi.Data.Entities;
using MikietaApi.Models;
using MikietaApi.SendEmail;
using MikietaApi.SendEmail.Order;
using MikietaApi.Services;
using MikietaApi.Stripe;
using NSubstitute;
using Shouldly;

namespace MikietaApi.Tests.Services;

public class OrderServiceTest
{
    public class StripeFacadeMock : StripeFacade
    {
        public StripeFacadeMock() : base("", "")
        {
        }

        public override StripeResponseModel CreateSession(StripeRequestModel[] models, double? deliveryPrice)
        {
            return new StripeResponseModel();
        }
    }

    private IOrderService _orderService = null!;
    private DataContext _dbContext = null!;
    private IDeliveryService _deliveryServiceMock = null!;
    private HttpClient _client = null!;

    [SetUp]
    public void SetUp()
    {
        _deliveryServiceMock = Substitute.For<IDeliveryService>();
        var factory = WebAppFactory.CreateFactory(services =>
        {
            services.Replace(ServiceDescriptor.Scoped<IDeliveryService>(_ => _deliveryServiceMock));
            services.Replace(ServiceDescriptor.Scoped<StripeFacade, StripeFacadeMock>());
            services.Replace(ServiceDescriptor.Scoped<IEmailSender<OrderEmailSenderModel>>(_ =>
                Substitute.For<IEmailSender<OrderEmailSenderModel>>()));
        });

        _client = factory.CreateClient();

        var provider = factory.Services.CreateScope().ServiceProvider;

        var context = provider.GetRequiredService<IHttpContextAccessor>();
        context.HttpContext = new DefaultHttpContext();

        _orderService = provider.GetRequiredService<IOrderService>();

        _dbContext = provider.GetRequiredService<DataContext>();

        _dbContext.Database.EnsureDeleted();

        _dbContext.Database.Migrate();

        var ingredients = new List<IngredientEntity>
        {
            new()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                Name = "Ingredient1",
            },
            new()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                Name = "Ingredient2",
            },
            new()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                Name = "Ingredient3"
            },
            new()
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
                    AdditionalIngredients = new[]
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
            new()
            {
                ProductId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Name = "Pizza1",
                OrderedProductOrderedIngredients = new OrderedProductOrderedIngredientEntity[]
                {
                    new()
                    {
                        OrderedIngredient = new OrderedIngredientEntity()
                        {
                            IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                            Name = "Ingredient1"
                        },
                        Quantity = 1
                    },
                    new()
                    {
                        OrderedIngredient = new OrderedIngredientEntity()
                        {
                            IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                            Name = "Ingredient2"
                        },
                        Quantity = 1
                    },
                    new()
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
                    RemovedIngredients = new[]
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
            new()
            {
                ProductId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Name = "Pizza1",
                OrderedProductOrderedIngredients = new OrderedProductOrderedIngredientEntity[]
                {
                    new()
                    {
                        OrderedIngredient = new OrderedIngredientEntity()
                        {
                            IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                            Name = "Ingredient2"
                        },
                        Quantity = 1
                    },
                    new()
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
                    ReplacedIngredients = new[]
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
            new()
            {
                ProductId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Name = "Pizza1",
                OrderedProductOrderedIngredients = new OrderedProductOrderedIngredientEntity[]
                {
                    new()
                    {
                        OrderedIngredient = new OrderedIngredientEntity()
                        {
                            IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                            Name = "Ingredient2"
                        },
                        Quantity = 1
                    },
                    new()
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
                        new()
                        {
                            Quantity = 1,
                            IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000004")
                        }
                    },
                    RemovedIngredients = new RemovedIngredientModel[]
                    {
                        new()
                        {
                            IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000001")
                        }
                    },
                    ReplacedIngredients = new[]
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
                        new()
                        {
                            IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000001")
                        },
                        new()
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
                        new()
                        {
                            Quantity = 2,
                            IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000004")
                        }
                    },
                    ReplacedIngredients = new[]
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
            new()
            {
                ProductId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Name = "Pizza1",
                OrderedProductOrderedIngredients = new OrderedProductOrderedIngredientEntity[]
                {
                    new()
                    {
                        OrderedIngredient = new OrderedIngredientEntity()
                        {
                            IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000004"),
                            Name = "Ingredient4"
                        },
                        IsAdditionalIngredient = true,
                        Quantity = 1
                    },
                    new()
                    {
                        OrderedIngredient = new OrderedIngredientEntity()
                        {
                            IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                            Name = "Ingredient1"
                        },
                        IsIngredientRemoved = true,
                        Quantity = 1
                    },
                    new()
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
            new()
            {
                ProductId = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Name = "Pizza2",
                OrderedProductOrderedIngredients = new OrderedProductOrderedIngredientEntity[]
                {
                    new()
                    {
                        OrderedIngredient = new OrderedIngredientEntity()
                        {
                            IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                            Name = "Ingredient1"
                        },
                        IsIngredientRemoved = true,
                        Quantity = 1
                    },
                    new()
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
            new()
            {
                ProductId = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                Name = "Pizza3",
                OrderedProductOrderedIngredients = new OrderedProductOrderedIngredientEntity[]
                {
                    new()
                    {
                        OrderedIngredient = new OrderedIngredientEntity()
                        {
                            IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000004"),
                            Name = "Ingredient4"
                        },
                        IsAdditionalIngredient = true,
                        Quantity = 2
                    },
                    new()
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
                    new()
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

    private static IEnumerable<TestCaseData> Order_TestCost_Cases()
    {
        yield return new TestCaseData(new OrderModel
        {
            Email = "test@test.test",
            Name = "name",
            Phone = "123",
            PaymentMethod = PaymentMethodType.Cash,
            ProductQuantities = new ProductQuantityModel[]
            {
                new()
                {
                    ProductId = Guid.Parse("00000000-0000-0000-0000-000000000004"),
                    Quantity = 2,
                }
            }
        }, PizzaType.Small, 1d, 28d).SetName("Order_TestCost 01");

        yield return new TestCaseData(new OrderModel
        {
            Email = "test@test.test",
            Name = "name",
            Phone = "123",
            PaymentMethod = PaymentMethodType.Cash,
            ProductQuantities = new ProductQuantityModel[]
            {
                new()
                {
                    ProductId = Guid.Parse("00000000-0000-0000-0000-000000000004"),
                    Quantity = 1,
                }
            }
        }, PizzaType.Medium, 1d, 16.25d).SetName("Order_TestCost 02");

        yield return new TestCaseData(new OrderModel
        {
            Email = "test@test.test",
            Name = "name",
            Phone = "123",
            PaymentMethod = PaymentMethodType.Cash,
            ProductQuantities = new ProductQuantityModel[]
            {
                new()
                {
                    ProductId = Guid.Parse("00000000-0000-0000-0000-000000000004"),
                    Quantity = 1,
                }
            }
        }, PizzaType.Large, 1d, 18.5d).SetName("Order_TestCost 03");

        yield return new TestCaseData(new OrderModel
        {
            Email = "test@test.test",
            Name = "name",
            Phone = "123",
            PaymentMethod = PaymentMethodType.Cash,
            ProductQuantities = new ProductQuantityModel[]
            {
                new()
                {
                    ProductId = Guid.Parse("00000000-0000-0000-0000-000000000004"),
                    Quantity = 1,
                    RemovedIngredients = new RemovedIngredientModel[]
                    {
                        new()
                        {
                            IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000005")
                        }
                    }
                }
            }
        }, PizzaType.Large, 1d, 15.5d).SetName("Order_TestCost 04");

        yield return new TestCaseData(new OrderModel
        {
            Email = "test@test.test",
            Name = "name",
            Phone = "123",
            PaymentMethod = PaymentMethodType.Cash,
            ProductQuantities = new ProductQuantityModel[]
            {
                new()
                {
                    ProductId = Guid.Parse("00000000-0000-0000-0000-000000000004"),
                    Quantity = 1,
                    AdditionalIngredients = new AdditionalIngredientModel[]
                    {
                        new()
                        {
                            Quantity = 2,
                            IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000006")
                        }
                    }
                }
            }
        }, PizzaType.Large, 1d, 23.5d).SetName("Order_TestCost 05");

        yield return new TestCaseData(new OrderModel
        {
            Email = "test@test.test",
            Name = "name",
            Phone = "123",
            PaymentMethod = PaymentMethodType.Cash,
            ProductQuantities = new ProductQuantityModel[]
            {
                new()
                {
                    ProductId = Guid.Parse("00000000-0000-0000-0000-000000000004"),
                    Quantity = 1,
                    ReplacedIngredients = new ReplacedIngredientModel[]
                    {
                        new()
                        {
                            FromIngredientId = Guid.Parse("10000000-0000-0000-0000-000000000006"),
                            ToIngredientId = Guid.Parse("10000000-0000-0000-0000-000000000005")
                        }
                    }
                }
            }
        }, PizzaType.Large, 1d, 19d).SetName("Order_TestCost 06");
    }

    [TestCaseSource(nameof(Order_TestCost_Cases))]
    public void Order_TestCost(OrderModel model, PizzaType pizzaType, double deliveryPrice, double expectedCost)
    {
        //Arrange
        _deliveryServiceMock.CheckDistance(Arg.Any<DeliveryModel>()).Returns(new DeliveryResponseModel
        {
            DeliveryPrice = deliveryPrice
        });

        var ingredients = new List<IngredientEntity>
        {
            new()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000005"),
                Name = "Ingredient5",
                PriceSmall = 1,
                PriceMedium = 2,
                PriceLarge = 3,
            },
            new()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000006"),
                Name = "Ingredient6",
                PriceSmall = 0.5,
                PriceMedium = 1.25,
                PriceLarge = 2.5,
            },
        };

        _dbContext.Ingredients.AddRange(ingredients);

        _dbContext.Products.Add(new ProductEntity
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000004"),
            Name = "Pizza1",
            Price = 12,
            Ingredients = ingredients,
            PizzaType = pizzaType
        });

        _dbContext.SaveChanges();

        //Act
        var orderId = _orderService.Order(model)?.OrderId!;
        var cost = _orderService.GetSingle(orderId.Value).Cost;

        //Assert
        cost.ShouldBe(expectedCost);
    }

    private static readonly string Message =
        "One or more provided Product Ids are not included in the expected set of IDs.";

    private static string RemovedMessage(string productId) =>
        $"One or more provided RemovedIngredient Ids are not included in the expected Product Id: {productId}.";

    private static readonly string AdditionalMessage =
        "One or more provided AdditionalIngredient Ids are not included in the expected set of Ingredient IDs.";

    private static string ReplacedFromMessage(string productId) =>
        $"One or more provided ReplacedFromIngredient Ids are not included in the expected Product Id: {productId}.";
    
    private static readonly string ReplacedToMessage =
        "One or more provided ReplacedToIngredient Ids are not included in the expected set of Ingredient IDs.";

    private static IEnumerable<TestCaseData> Order_ProductsIdsValidationTest_Cases()
    {
        yield return new TestCaseData(
            CreateModel(new ProductQuantityModel[]
            {
                new()
                {
                    ProductId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    Quantity = 1
                }
            }), true, "").SetName("Order_ProductsIdsValidationTest 01");

        yield return new TestCaseData(
            CreateModel(new ProductQuantityModel[]
            {
                new()
                {
                    ProductId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    Quantity = 1
                },
                new()
                {
                    ProductId = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                    Quantity = 1
                }
            }), true, "").SetName("Order_ProductsIdsValidationTest 02");

        yield return new TestCaseData(
            CreateModel(new ProductQuantityModel[]
            {
                new()
                {
                    ProductId = Guid.Parse("00000000-0000-0000-0000-000000000011"),
                    Quantity = 1
                }
            }), false, Message).SetName("Order_ProductsIdsValidationTest 03");

        yield return new TestCaseData(
            CreateModel(new ProductQuantityModel[]
            {
                new()
                {
                    ProductId = Guid.Parse("00000000-0000-0000-0000-000000000011"),
                    Quantity = 1
                },
                new()
                {
                    ProductId = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                    Quantity = 1
                },
                new()
                {
                    ProductId = Guid.Parse("00000000-0000-0000-0000-000000000012"),
                    Quantity = 1
                }
            }), false, Message).SetName("Order_ProductsIdsValidationTest 04");

        yield return new TestCaseData(
            CreateModel(new ProductQuantityModel[]
            {
                new()
                {
                    ProductId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    Quantity = 1,
                    RemovedIngredients = new RemovedIngredientModel[]
                    {
                        new()
                        {
                            IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000001")
                        }
                    }
                }
            }), true, "").SetName("Order_ProductsIdsValidationTest 05");

        yield return new TestCaseData(
                CreateModel(new ProductQuantityModel[]
                {
                    new()
                    {
                        ProductId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                        Quantity = 1,
                        RemovedIngredients = new RemovedIngredientModel[]
                        {
                            new()
                            {
                                IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000021")
                            }
                        }
                    }
                }), false, RemovedMessage("00000000-0000-0000-0000-000000000001"))
            .SetName("Order_ProductsIdsValidationTest 06");

        yield return new TestCaseData(
                CreateModel(new ProductQuantityModel[]
                {
                    new()
                    {
                        ProductId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                        Quantity = 1,
                        RemovedIngredients = new RemovedIngredientModel[]
                        {
                            new()
                            {
                                IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000003")
                            }
                        }
                    }
                }), false, RemovedMessage("00000000-0000-0000-0000-000000000001"))
            .SetName("Order_ProductsIdsValidationTest 07");

        yield return new TestCaseData(
            CreateModel(new ProductQuantityModel[]
            {
                new()
                {
                    ProductId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    Quantity = 1,
                    AdditionalIngredients = new AdditionalIngredientModel[]
                    {
                        new()
                        {
                            IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                            Quantity = 1
                        }
                    }
                }
            }), true, "").SetName("Order_ProductsIdsValidationTest 08");

        yield return new TestCaseData(
            CreateModel(new ProductQuantityModel[]
            {
                new()
                {
                    ProductId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    Quantity = 1,
                    AdditionalIngredients = new AdditionalIngredientModel[]
                    {
                        new()
                        {
                            IngredientId = Guid.Parse("10000000-0000-0000-0000-000000000023"),
                            Quantity = 1
                        }
                    }
                }
            }), false, AdditionalMessage).SetName("Order_ProductsIdsValidationTest 09");

        yield return new TestCaseData(
            CreateModel(new ProductQuantityModel[]
            {
                new()
                {
                    ProductId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    Quantity = 1,
                    ReplacedIngredients = new ReplacedIngredientModel[]
                    {
                        new()
                        {
                            FromIngredientId = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                            ToIngredientId = Guid.Parse("10000000-0000-0000-0000-000000000003")
                        }
                    }
                }
            }), true, "").SetName("Order_ProductsIdsValidationTest 10");
        
        yield return new TestCaseData(
            CreateModel(new ProductQuantityModel[]
            {
                new()
                {
                    ProductId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    Quantity = 1,
                    ReplacedIngredients = new ReplacedIngredientModel[]
                    {
                        new()
                        {
                            FromIngredientId = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                            ToIngredientId = Guid.Parse("10000000-0000-0000-0000-000000000004")
                        }
                    }
                }
            }), false, ReplacedFromMessage("00000000-0000-0000-0000-000000000001")).SetName("Order_ProductsIdsValidationTest 11");
        
        yield return new TestCaseData(
            CreateModel(new ProductQuantityModel[]
            {
                new()
                {
                    ProductId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    Quantity = 1,
                    ReplacedIngredients = new ReplacedIngredientModel[]
                    {
                        new()
                        {
                            FromIngredientId = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                            ToIngredientId = Guid.Parse("10000000-0000-0000-0000-000000000004")
                        }
                    }
                }
            }), true, "").SetName("Order_ProductsIdsValidationTest 12");
        
        yield return new TestCaseData(
            CreateModel(new ProductQuantityModel[]
            {
                new()
                {
                    ProductId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    Quantity = 1,
                    ReplacedIngredients = new ReplacedIngredientModel[]
                    {
                        new()
                        {
                            FromIngredientId = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                            ToIngredientId = Guid.Parse("10000000-0000-0000-0000-000000000011")
                        }
                    }
                }
            }), false, ReplacedToMessage).SetName("Order_ProductsIdsValidationTest 13");
        
        yield return new TestCaseData(
            CreateModel(new ProductQuantityModel[]
            {
                new()
                {
                    ProductId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    Quantity = 1,
                    ReplacedIngredients = new ReplacedIngredientModel[]
                    {
                        new()
                        {
                            FromIngredientId = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                            ToIngredientId = Guid.Parse("10000000-0000-0000-0000-000000000001")
                        }
                    }
                }
            }), true, "").SetName("Order_ProductsIdsValidationTest 14");
        
        yield return new TestCaseData(
            CreateModel(new ProductQuantityModel[]
            {
                new()
                {
                    ProductId = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                    Quantity = 1
                },
                new()
                {
                    ProductId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    Quantity = 1
                }
            }), true, "").SetName("Order_ProductsIdsValidationTest 15");
    }

    private static OrderModel CreateModel(ProductQuantityModel[] models) => new()
    {
        ProductQuantities = models,
        Name = "1",
        Phone = "1",
        Email = "w@w.w",
        PaymentMethod = PaymentMethodType.Cash,
        DeliveryRightAway = true,
        DeliveryMethod = DeliveryMethodType.DinningIn
    };

    [TestCaseSource(nameof(Order_ProductsIdsValidationTest_Cases))]
    public async Task Order_ProductsIdsValidationTest(OrderModel order, bool areIdsValid, string expectedMessage)
    {
        //Arrange
        _deliveryServiceMock.CheckDistance(Arg.Any<DeliveryModel>()).Returns(new DeliveryResponseModel
        {
            DeliveryPrice = 0
        });

        //Act
        var response = await _client.PostAsync("order", order.ToStringContent());

        //Assert
        if (areIdsValid)
        {
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }
        else
        {
            var content = await response.Content.ReadAsStringAsync();
            content.ShouldBe($"\"{expectedMessage}\"");
        }
    }
}