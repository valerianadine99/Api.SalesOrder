using Api.SalesOrder.Application.DTOs;
using Api.SalesOrder.Application.Interfaces;
using Api.SalesOrder.SharedKernel.Models;
using AutoMapper;
using MediatR;

namespace Api.SalesOrder.Application.Features.SalesOrder.Querys.GetOrders
{
    public class GetOrdersQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<GetOrdersQuery, PagedResult<OrderDto>>
    {
        public async Task<PagedResult<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        {
            var (orders, totalCount) = await _unitOfWork.Orders.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                request.CustomerFilter,
                request.StartDate,
                request.EndDate,
                cancellationToken);

            return new PagedResult<OrderDto>
            {
                Items = _mapper.Map<IEnumerable<OrderDto>>(orders),
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
