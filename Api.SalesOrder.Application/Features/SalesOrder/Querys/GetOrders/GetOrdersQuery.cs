using Api.SalesOrder.Application.DTOs;
using Api.SalesOrder.SharedKernel.Models;
using MediatR;

namespace Api.SalesOrder.Application.Features.SalesOrder.Querys.GetOrders
{
    public class GetOrdersQuery : IRequest<PagedResult<OrderDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? CustomerFilter { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
