using Jwt.Core;
using MikietaApi.Models;

namespace MikietaApi.Services;

interface ILoginService
{
    string Login(LoginModel model);
}

public class LoginService : ILoginService
{
    private readonly IJwtTokenFactory _jwtFactory;
    private readonly ConfigurationOptions _options;

    public LoginService(IJwtTokenFactory jwtFactory, ConfigurationOptions options)
    {
        _jwtFactory = jwtFactory;
        _options = options;
    }
    
    public string Login(LoginModel model)
    {
        if (model.Login == _options.AdminLogin && model.Password == _options.AdminPassword)
        {
            return _jwtFactory.Create(Guid.NewGuid().ToString());
        }

        throw new UnauthorizedAccessException();
    }
}