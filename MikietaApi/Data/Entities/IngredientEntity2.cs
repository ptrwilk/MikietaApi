using MikietaApi.Data.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace MikietaApi.Data.Entities
{
    public class IngredientEntity2
    {
        [Key]
        public int Id { get; set; }
        public IngredientTypeEntity IngredientType { get; set; }
    }
}
