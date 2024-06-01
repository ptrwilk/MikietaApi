namespace MikietaApi.Models
{
    public class PizzaModel : ProductModel
    {
        public DoughType DoughType { get; set; }
        public PizzaSize PizzaSize { get; set; }
        public IList<ProductModel> Subproducts { get; set; } = new List<ProductModel>();
    }

    public enum PizzaSize
    {
        Small,
        Medium,
        Large
    }

    public enum DoughType
    {
        ThickCrust,
        ThinCrust
    }
}
