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

    public LoginService(IJwtTokenFactory jwtFactory)
    {
        _jwtFactory = jwtFactory;
    }
    
    public string Login(LoginModel model)
    {
        if (model.Login == "admin" && model.Password == "admin")
        {
            return _jwtFactory.Create(Guid.NewGuid().ToString());
        }

        throw new UnauthorizedAccessException();
    }
}