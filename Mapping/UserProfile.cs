using System;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using Mapster;

namespace ApiEcommerce.Mapping;

public class UserProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        /*config.NewConfig<User, UserDto>().IgnoreNullValues(true);
        config.NewConfig<User, CreateUserDto>().IgnoreNullValues(true);
        config.NewConfig<User, UserLoginDto>().IgnoreNullValues(true);
        config.NewConfig<User, UserLoginResponseDto>().IgnoreNullValues(true);*/
        config.NewConfig<ApplicationUser, UserDataDto>().IgnoreNullValues(true);
        config.NewConfig<ApplicationUser, UserDto>().IgnoreNullValues(true);
    }
}
