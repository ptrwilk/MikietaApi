using System.Text.Json;
using System.Text.Json.Serialization;
using MikietaApi.Data.Entities;

namespace MikietaApi.Data;

public class SettingsPlainModel
{
    [JsonPropertyName("street")] public string Street { get; set; } = null!;
    [JsonPropertyName("city")] public string City { get; set; } = null!;
    [JsonPropertyName("zipCode")] public string ZipCode { get; set; } = null!;
    [JsonPropertyName("phone")] public string Phone { get; set; } = null!;
    [JsonPropertyName("email")] public string Email { get; set; } = null!;
    [JsonPropertyName("facebook")] public string Facebook { get; set; } = null!;
    [JsonPropertyName("pricePerKm")] public string PricePerKm { get; set; } = null!;
    [JsonPropertyName("distanceInKm")] public string DistanceInKm { get; set; } = null!;
    [JsonPropertyName("openingHours")] public SettingsHoursPlainModel OpeningHours { get; set; } = null!;
    [JsonPropertyName("deliveryHours")] public SettingsHoursPlainModel DeliveryHours { get; set; } = null!;
    [JsonPropertyName("closed")] public string[] Closed { get; set; } = null!;
}

public class SettingsHoursPlainModel
{
    [JsonPropertyName("monday")] public SettingsDayOfWeekPlainModel Monday { get; set; } = null!;
    [JsonPropertyName("tuesday")] public SettingsDayOfWeekPlainModel Tuesday { get; set; } = null!;
    [JsonPropertyName("wednesday")] public SettingsDayOfWeekPlainModel Wednesday { get; set; } = null!;
    [JsonPropertyName("thursday")] public SettingsDayOfWeekPlainModel Thursday { get; set; } = null!;
    [JsonPropertyName("friday")] public SettingsDayOfWeekPlainModel Friday { get; set; } = null!;
    [JsonPropertyName("saturday")] public SettingsDayOfWeekPlainModel Saturday { get; set; } = null!;
    [JsonPropertyName("sunday")] public SettingsDayOfWeekPlainModel Sunday { get; set; } = null!;
}

public class SettingsDayOfWeekPlainModel
{
    [JsonPropertyName("from")] public string From { get; set; } = null!;
    [JsonPropertyName("to")] public string To { get; set; } = null!;
}

public class SettingsParsedModel
{
    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;
}

public class SettingsParser
{
    public SettingsParsedModel[] Parse()
    {
        var text = File.ReadAllText(@"Data\Seeder\Settings\settings.json");

        var model = JsonSerializer.Deserialize<SettingsPlainModel>(text)!;

        var res = new List<SettingsParsedModel>
        {
            Create(SettingEntity.Street, model.Street),
            Create(SettingEntity.City, model.City),
            Create(SettingEntity.ZipCode, model.ZipCode),
            Create(SettingEntity.Phone, model.Phone),
            Create(SettingEntity.Facebook, model.Facebook),
            Create(SettingEntity.DeliveryRange, model.DistanceInKm),
            Create(SettingEntity.DeliveryPrice, model.PricePerKm),
            Create(SettingEntity.Email, model.Email),
            
            Create(SettingEntity.OpenMondayFrom, model.OpeningHours.Monday.From),
            Create(SettingEntity.OpenTuesdayFrom, model.OpeningHours.Tuesday.From),
            Create(SettingEntity.OpenWednesdayFrom, model.OpeningHours.Wednesday.From),
            Create(SettingEntity.OpenThursdayFrom, model.OpeningHours.Thursday.From),
            Create(SettingEntity.OpenFridayFrom, model.OpeningHours.Friday.From),
            Create(SettingEntity.OpenSaturdayFrom, model.OpeningHours.Saturday.From),
            Create(SettingEntity.OpenSundayFrom, model.OpeningHours.Sunday.From),
            
            Create(SettingEntity.OpenMondayTo, model.OpeningHours.Monday.To),
            Create(SettingEntity.OpenTuesdayTo, model.OpeningHours.Tuesday.To),
            Create(SettingEntity.OpenWednesdayTo, model.OpeningHours.Wednesday.To),
            Create(SettingEntity.OpenThursdayTo, model.OpeningHours.Thursday.To),
            Create(SettingEntity.OpenFridayTo, model.OpeningHours.Friday.To),
            Create(SettingEntity.OpenSaturdayTo, model.OpeningHours.Saturday.To),
            Create(SettingEntity.OpenSundayTo, model.OpeningHours.Sunday.To),
            
            Create(SettingEntity.DeliveryMondayFrom, model.DeliveryHours.Monday.From),
            Create(SettingEntity.DeliveryTuesdayFrom, model.DeliveryHours.Tuesday.From),
            Create(SettingEntity.DeliveryWednesdayFrom, model.DeliveryHours.Wednesday.From),
            Create(SettingEntity.DeliveryThursdayFrom, model.DeliveryHours.Thursday.From),
            Create(SettingEntity.DeliveryFridayFrom, model.DeliveryHours.Friday.From),
            Create(SettingEntity.DeliverySaturdayFrom, model.DeliveryHours.Saturday.From),
            Create(SettingEntity.DeliverySundayFrom, model.DeliveryHours.Sunday.From),
            
            Create(SettingEntity.DeliveryMondayTo, model.DeliveryHours.Monday.To),
            Create(SettingEntity.DeliveryTuesdayTo, model.DeliveryHours.Tuesday.To),
            Create(SettingEntity.DeliveryWednesdayTo, model.DeliveryHours.Wednesday.To),
            Create(SettingEntity.DeliveryThursdayTo, model.DeliveryHours.Thursday.To),
            Create(SettingEntity.DeliveryFridayTo, model.DeliveryHours.Friday.To),
            Create(SettingEntity.DeliverySaturdayTo, model.DeliveryHours.Saturday.To),
            Create(SettingEntity.DeliverySundayTo, model.DeliveryHours.Sunday.To),

            Create(SettingEntity.Closures, model.Closed)
        };

        return res.ToArray();
    }

    private SettingsParsedModel Create(string key, string value)
    {
        return new SettingsParsedModel
        {
            Key = key,
            Value = value
        };
    }
    
    private SettingsParsedModel Create(string key, string[] values)
    {
        return new SettingsParsedModel
        {
            Key = key,
            Value = JsonSerializer.Serialize(values)
        };
    }
}