using MikietaApi.Data.Entities.Enums;

namespace MikietaApi.Data.Entities
{
    public class PizzaEntity :ProductEntity
    {
        public required ICollection<IngredientEntity2> Ingredients2 { get; set; }
        public PizzaSizeEntity Size { get; set; }
        public DoughTypeEntity DoughType { get; set; }
    }
}
