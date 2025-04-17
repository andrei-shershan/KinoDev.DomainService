using KinoDev.DomainService.Infrastructure.Models;
using KinoDev.DomainService.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KinoDev.DomainService.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderServcie;

        public OrdersController(IOrderService orderServcie)
        {
            _orderServcie = orderServcie;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetOrderAsync(Guid id)
        {
            var order = await _orderServcie.GetOrderAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        [HttpGet("summary/{id:guid}")]
        public async Task<IActionResult> GetOrderSummaryAsync(Guid id)
        {
            var order = await _orderServcie.GetOrderAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrderAsync([FromBody] CreateOrderModel orderModel)
        {
            var result = await _orderServcie.CreateOrderAsync(orderModel);
            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpPost("{id:guid}/complete")]
        public async Task<IActionResult> CompleteOrderAsync(Guid id)
        {
            var result = await _orderServcie.CompleteOrderAsync(id);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}