using API.Application.DTO;
using API.Application.DTOs;
using API.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Application.AutoMapper
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<ItemDTO, Item>().ReverseMap();  
            CreateMap<UpdateItemDTO, Item>().ReverseMap();
            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<Item, ItemDTO>().ReverseMap();
            CreateMap<EmployeeDTO, Employee>().ReverseMap();

            CreateMap<EmpResponseDTO, Employee>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString())).ReverseMap();

            CreateMap<AuthDTO, Employee>().ReverseMap();

            CreateMap<CreateProductDTO, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())   
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedOn, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<CreateItemDTO, Item>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ProductId, opt => opt.Ignore())
                .ReverseMap(); 

            CreateMap<UpdateProductDTO, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedOn, opt => opt.Ignore())  
                .ForMember(dest => dest.Items, opt => opt.Ignore())
                .ReverseMap();
        }   
    }
}   
