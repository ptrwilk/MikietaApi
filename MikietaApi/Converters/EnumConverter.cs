using MikietaApi.Data.Entities;
using MikietaApi.Models;

namespace MikietaApi.Converters;

public static class EnumConverter
{
    public static ProductType Convert(ProductTypeEntity type) =>
        (ProductType)Enum.Parse(typeof(ProductType), type.ToString());
    
    public static ProductTypeEntity Convert(ProductType type) =>
        (ProductTypeEntity)Enum.Parse(typeof(ProductTypeEntity), type.ToString());
}