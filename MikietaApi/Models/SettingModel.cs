namespace MikietaApi.Models;

public class SettingModel
{
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? ZipCode { get; set; }
    public string? Phone { get; set; }
    public string? Facebook { get; set; }
    public double? DeliveryRange { get; set; }
    public double? DeliveryPrice { get; set; }

    public SettingHoursModel[] OpeningHours { get; set; } = null!;
    public SettingHoursModel[] DeliveryHours { get; set; } = null!;
}

public class SettingHoursModel
{
    public TimeSpan From { get; set; }
    public TimeSpan To { get; set; }
}