using Api.SalesOrder.Application.DTOs;
using Api.SalesOrder.Application.Features.SalesOrder.Commands.CreateOrder;
using Api.SalesOrder.Domain.Entities;
using AutoMapper;

namespace Api.SalesOrder.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
            CreateMap<OrderDetail, OrderDetailDto>();
            CreateMap<CreateOrderDto, CreateOrderCommand>();
            CreateMap<CreateOrderDetailDto, OrderDetail>();
        }
    }
}
