namespace MikietaApi.Data.Entities;

public class SettingEntity
{
    public string Key { get; set; } = null!;
    public string? Value { get; set; }

    public const string Street = nameof(Street);
    public const string City = nameof(City);
    public const string ZipCode = nameof(ZipCode);
    public const string Phone = nameof(Phone);
    public const string Facebook = nameof(Facebook);
    public const string DeliveryRange = nameof(DeliveryRange);
    public const string DeliveryPrice = nameof(DeliveryPrice);
    
    public const string OpenMondayFrom = nameof(OpenMondayFrom);
    public const string OpenTuesdayFrom = nameof(OpenTuesdayFrom);
    public const string OpenWednesdayFrom = nameof(OpenWednesdayFrom);
    public const string OpenThursdayFrom = nameof(OpenThursdayFrom);
    public const string OpenFridayFrom = nameof(OpenFridayFrom);
    public const string OpenSaturdayFrom = nameof(OpenSaturdayFrom);
    public const string OpenSundayFrom = nameof(OpenSundayFrom);
    
    public const string OpenMondayTo = nameof(OpenMondayTo);
    public const string OpenTuesdayTo = nameof(OpenTuesdayTo);
    public const string OpenWednesdayTo = nameof(OpenWednesdayTo);
    public const string OpenThursdayTo = nameof(OpenThursdayTo);
    public const string OpenFridayTo = nameof(OpenFridayTo);
    public const string OpenSaturdayTo = nameof(OpenSaturdayTo);
    public const string OpenSundayTo = nameof(OpenSundayTo);
    
    public const string DeliveryMondayFrom = nameof(DeliveryMondayFrom);
    public const string DeliveryTuesdayFrom = nameof(DeliveryTuesdayFrom);
    public const string DeliveryWednesdayFrom = nameof(DeliveryWednesdayFrom);
    public const string DeliveryThursdayFrom = nameof(DeliveryThursdayFrom);
    public const string DeliveryFridayFrom = nameof(DeliveryFridayFrom);
    public const string DeliverySaturdayFrom = nameof(DeliverySaturdayFrom);
    public const string DeliverySundayFrom = nameof(DeliverySundayFrom);
    
    public const string DeliveryMondayTo = nameof(DeliveryMondayTo);
    public const string DeliveryTuesdayTo = nameof(DeliveryTuesdayTo);
    public const string DeliveryWednesdayTo = nameof(DeliveryWednesdayTo);
    public const string DeliveryThursdayTo = nameof(DeliveryThursdayTo);
    public const string DeliveryFridayTo = nameof(DeliveryFridayTo);
    public const string DeliverySaturdayTo = nameof(DeliverySaturdayTo);
    public const string DeliverySundayTo = nameof(DeliverySundayTo);

    public static readonly string[] OpensFrom =
        { OpenMondayFrom, OpenTuesdayFrom, OpenWednesdayFrom, OpenThursdayFrom, OpenFridayFrom, OpenSaturdayFrom, OpenSundayFrom };
    
    public static readonly string[] OpensTo =
        { OpenMondayTo, OpenTuesdayTo, OpenWednesdayTo, OpenThursdayTo, OpenFridayTo, OpenSaturdayTo, OpenSundayTo };
    
    public static readonly string[] DeliveriesFrom =
        { DeliveryMondayFrom, DeliveryTuesdayFrom, DeliveryWednesdayFrom, DeliveryThursdayFrom, DeliveryFridayFrom, DeliverySaturdayFrom, DeliverySundayFrom };
    
    public static readonly string[] DeliveriesTo =
        { DeliveryMondayTo, DeliveryTuesdayTo, DeliveryWednesdayTo, DeliveryThursdayTo, DeliveryFridayTo, DeliverySaturdayTo, DeliverySundayTo };
    
    public static readonly string[] Keys = new[] { Street, City, ZipCode, Phone, Facebook, DeliveryRange, DeliveryPrice }
        .Concat(OpensFrom).Concat(OpensTo).Concat(DeliveriesFrom).Concat(DeliveriesTo).ToArray();

}