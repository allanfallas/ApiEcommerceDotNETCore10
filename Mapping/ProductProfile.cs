using System;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using Mapster;

namespace ApiEcommerce.Mapping;

public class ProductProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Product, ProductDto>()
            .Map(dest => dest.CategoryName, src => src.Category.Name)
            .IgnoreNullValues(true);
        config.NewConfig<CreateProductDto, Product>().IgnoreNullValues(true);
        config.NewConfig<UpdateProductDto, Product>().IgnoreNullValues(true);
    }
}
