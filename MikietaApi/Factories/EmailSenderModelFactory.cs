using MikietaApi.Data;
using MikietaApi.Data.Entities;
using MikietaApi.SendEmail;

namespace MikietaApi.Factories;

public interface IEmailSenderModelFactory
{
    T Create<T>(SettingEntity[] settings, Action<T> action)
        where T : EmailSenderModelBase, new();
}

public class EmailSenderModelFactory: IEmailSenderModelFactory
{
    private readonly ConfigurationOptions _options;

    public EmailSenderModelFactory(ConfigurationOptions options)
    {
        _options = options;
    }
    
    public T Create<T>(SettingEntity[] settings, Action<T> action)
        where T : EmailSenderModelBase, new()
    {
        var street = DataContext.GetValue<string>(settings, SettingEntity.Street);
        var city = DataContext.GetValue<string>(settings, SettingEntity.City);
        var zipCode = DataContext.GetValue<string>(settings, SettingEntity.ZipCode);
        var phone = DataContext.GetValue<string>(settings, SettingEntity.Phone);
        
        var res = new T
        {
            Address = $"{street}, {zipCode} {city}",
            Phone = phone,
            Link = _options.WebsiteUrl,
            LinkText = "www.pizzeriamiketa.pl"
        };

        action(res);
        
        return res;
    }
}