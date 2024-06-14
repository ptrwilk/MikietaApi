using MikietaApi.SendEmail.Order;

namespace MikietaApi.Tests.SendEmail;

public class OrderEmailSenderTest
{
    private class OrderEmailSenderMock : OrderEmailSender
    {
        public OrderEmailSenderMock() : base(null)
        {
        }

        public new string ReadFromTemplate(OrderEmailSenderModel model)
        {
            return base.ReadFromTemplate(model);
        }
    }

    [Explicit]
    [Test]
    public void OrderEmailSender()
    {
        var sender = new OrderEmailSenderMock();
        
        var content = sender.ReadFromTemplate(new OrderEmailSenderModel
        {
            Delivery = true,
            DeliveryCost = 4,
            TransferPaid = false,
            Link = "https://google.pl",
            OrderDate = DateTime.Now,
            Products = new OrderProductFragmentModel[]
            {
                new()
                {
                    Name = "Some Name",
                    Price = 12.341,
                    Ingredients = new[] { "Ser", "Kurczak" }
                },
                new()
                {
                    Name = "Some Name 2",
                    Price = 2.331,
                },
                new()
                {
                    Name = "Some Name 3",
                    Price = 4.331,
                    Ingredients = Array.Empty<string>()
                }
            }
        });

        File.WriteAllText("OrderEmailSender.html", content);
    }
}