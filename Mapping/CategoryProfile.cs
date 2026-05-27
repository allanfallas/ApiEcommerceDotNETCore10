using System;
using ApiEcommerce.Models.Dtos;
using Mapster;

namespace ApiEcommerce.Mapping;

public class CategoryProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Category, CategoryDto>().IgnoreNullValues(true);
        config.NewConfig<CreateCategoryDto, Category>().IgnoreNullValues(true);
    }
}
