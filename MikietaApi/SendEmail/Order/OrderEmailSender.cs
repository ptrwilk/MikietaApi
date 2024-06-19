using System.Globalization;

namespace MikietaApi.SendEmail.Order;

public class OrderEmailSender : EmailSenderBase<OrderEmailSenderModel>
{
    private const string Path = "SendEmail/Order/Templates/send_order_email_template.html";
    private const string ProductFragmentPath = "SendEmail/Order/Templates/product_fragment.html";
    private const string IngredientsFragmentPath = "SendEmail/Order/Templates/ingredients_fragment.html";
    private const string DeliveryCostFragmentPath = "SendEmail/Order/Templates/delivery_cost_fragment.html";
    
    protected override string Subject => "Zamówienie w Pizzerii Mikieta";

    public OrderEmailSender(EmailSenderOption option) : base(option)
    {
    }

    protected override string ReadFromTemplate(OrderEmailSenderModel model)
    {
        var content = File.ReadAllText(Path);
        
        content = content.Replace("[LINK]", model.Link);
        content = content.Replace("[DATE]", model.OrderDate.ToString("yyyy-MM-dd"));
        content = content.Replace("[TIME]", model.OrderDate.ToString("HH:mm"));
        content = content.Replace("[DELIVERY_METHOD]", model.Delivery ? "Dostawa" : "Odbiór własny");
        content = content.Replace("[PAYMENT_METHOD]", model.TransferPaid ? "Przelew (zapłacono)" : "Gotówka");
        content = content.Replace("[TOTAL_PRICE]", ToString(model.Products.Select(x => x.Price).Sum() + model.DeliveryCost));
        content = content.Replace("[PRODUCT_FRAGMENT]", string.Join("\n", model.Products.Select(ReadFromProductFragment).ToArray()));
        content = content.Replace("[DELIVERY_COST_FRAGMENT]", ReadFromDeliveryCostFragment(model));
        content = content.Replace("[RECIPIENT_FRAGMENT]", ReadFromRecipientFragment(model));

        return content;
    }

    private string ReadFromProductFragment(OrderProductFragmentModel model)
    {
        var content = File.ReadAllText(ProductFragmentPath);
        
        content = content.Replace("[NAME]", model.Name);
        content = content.Replace("[PRICE]", ToString(model.Price));
        content = content.Replace("[INGREDIENT_FRAGMENT]", ReadFromIngredientsFragment(model.Ingredients));

        return content;
    }

    private string ReadFromIngredientsFragment(string[]? ingredients)
    {
        if (ingredients is null || ingredients.Length == 0)
        {
            return "";
        }
        
        var content = File.ReadAllText(IngredientsFragmentPath);
        
        content = content.Replace("[INGREDIENTS]", string.Join(", ", ingredients));

        return content;
    }

    private string ReadFromDeliveryCostFragment(OrderEmailSenderModel model)
    {
        if (!model.Delivery)
        {
            return string.Empty;
        }
        
        var content = File.ReadAllText(DeliveryCostFragmentPath);
        
        content = content.Replace("[COST]", ToString(model.DeliveryCost));
        
        return content;
    }

    private string ToString(double value)
    {
        return Math.Round(value, 2, MidpointRounding.AwayFromZero).ToString(CultureInfo.InvariantCulture);
    }
}