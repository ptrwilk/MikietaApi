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
            //TODO: brac to potem najprawdopodbiniej z configu, nie z settingsów
            Link = "http://google.pl",
            LinkText = "www.pizzeriamikieta.pl"
        };

        action(res);
        
        return res;
    }
}