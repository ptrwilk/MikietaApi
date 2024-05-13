using MikietaApi.Data.Entities;
using MikietaApi.Models;

namespace MikietaApi.Converters;

public static class EnumConverter
{
    public static ProductType Convert(ProductTypeEntity type) =>
        (ProductType)Enum.Parse(typeof(ProductType), type.ToString());
}