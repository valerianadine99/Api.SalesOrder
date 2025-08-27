using Api.SalesOrder.Application.DTOs;
using Api.SalesOrder.Application.Features.SalesOrder.Commands.CreateOrder;
using Api.SalesOrder.Application.Features.SalesOrder.Commands.DeleteOrder;
using Api.SalesOrder.Application.Features.SalesOrder.Commands.UpdateOrder;
using Api.SalesOrder.Application.Features.SalesOrder.Querys.GetOrderById;
using Api.SalesOrder.Application.Features.SalesOrder.Querys.GetOrders;
using Api.SalesOrder.SharedKernel.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.SalesOrder.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all orders with pagination and filters
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "Vendedor")]
        public async Task<ActionResult<PagedResult<OrderDto>>> GetOrders(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? customerFilter = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {            
            var query = new GetOrdersQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                CustomerFilter = customerFilter,
                StartDate = startDate,
                EndDate = endDate
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get order by ID with details
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Policy = "Vendedor")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            var query = new GetOrderByIdQuery { Id = id };
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Create a new order
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "Vendedor")]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderCommand command)
        {
            command.CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetOrder), new { id = result.Id }, result);
        }

        /// <summary>
        /// Update an existing order
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult<OrderDto>> UpdateOrder(int id, [FromBody] UpdateOrderCommand command)
        {
            command.UpdatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

            if (id != command.Id)
                return BadRequest("ID mismatch");

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Delete an order
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

            var command = new DeleteOrderCommand { Id = id, DeletedBy = userId };
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
