using MikietaApi.Models;
using System.ComponentModel.DataAnnotations;

namespace MikietaApi.Data.Entities
{
    public class IngredientPriceEntity
    {
        [Key]
        public int Id { get; set; }
        public int IngredientId { get; set; }
        public PizzaSize PizzaSize { get; set; }
        public decimal Price { get; set; }
    }
}
