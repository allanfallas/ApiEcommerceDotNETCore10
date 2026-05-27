using System;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;

namespace ApiEcommerce.Repository.IRepository;

public interface IUserRepository
{
    ICollection<ApplicationUser> GetUsers();

    ApplicationUser? GetUser(string id);

    bool IsUniqueUser(string username);

    Task<UserLoginResponseDto> Login(UserLoginDto loginDto);

    Task<UserDataDto> Register(CreateUserDto createUserDto);

}
