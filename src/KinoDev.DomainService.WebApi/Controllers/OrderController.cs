using KinoDev.DomainService.Infrastructure.Models;
using KinoDev.DomainService.Infrastructure.Services.Abstractions;
using KinoDev.DomainService.WebApi.Models;
using KinoDev.Shared.DtoModels.Orders;
using KinoDev.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KinoDev.DomainService.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderServcie;

        private readonly IOrderProcessorService _orderProcessorService;

        public OrdersController(
            IOrderService orderServcie,
            IOrderProcessorService orderProcessorService
            )
        {
            _orderServcie = orderServcie;
            _orderProcessorService = orderProcessorService;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetOrderAsync([FromRoute] Guid id)
        {
            var order = await _orderServcie.GetOrderAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        [HttpGet("summary/{id:guid}")]
        public async Task<IActionResult> GetOrderSummaryAsync([FromRoute] Guid id)
        {
            var order = await _orderServcie.GetOrderAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        [HttpPost("completed")]
        public async Task<IActionResult> GetCompletedOrdersAsync([FromBody] GetCompletedOrdersModel model)
        {
            var orders = await _orderServcie.GetCompletedOrdersAsync(model.OrderIds);
            if (orders.IsNullOrEmptyCollection())
            {
                return NotFound();
            }

            return Ok(orders);
        }

        [HttpPost("completed/email")]
        public async Task<IActionResult> GetCompletedOrdersByEmailAsync([FromBody] GetCompletedOrdersByEmailModel model)
        {
            var orders = await _orderServcie.GetCompletedOrdersByEmailAsync(model.Email);
            if (orders.IsNullOrEmptyCollection())
            {
                return NotFound();
            }

            return Ok(orders);
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

        [HttpPatch("{id:guid}/email")]
        public async Task<IActionResult> UpdateOrderEmailAsync([FromRoute] Guid id, [FromBody] string email)
        {
            // TODO: Use Operation result to determine what excatly failed
            var result = await _orderServcie.UpdateOrderEmailAsync(id, email);
            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpPost("{id:guid}/complete")]
        public async Task<IActionResult> CompleteOrderAsync([FromRoute] Guid id)
        {
            var result = await _orderServcie.CompleteOrderAsync(id);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteOrderAsync([FromRoute] Guid id)
        {
            var result = await _orderServcie.DeleteOrderAsync(id);
            if (!result)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        //TODO: Add authentication and authorization to these endpoints
        [AllowAnonymous]
        [HttpPost("process-order-file-created")]
        public async Task<IActionResult> ProcessOrderFileCreatedAsync([FromBody] OrderSummary orderSummary)
        {
            await _orderProcessorService.ProcessOrderFileUrl(orderSummary);

            return Ok();
        }

        
        [AllowAnonymous]
        [HttpPost("process-email-sent")]
        public async Task<IActionResult> ProcessEmailSentAsync([FromBody] OrderSummary orderSummary)
        {
            await _orderProcessorService.ProcessOrderEmail(orderSummary);

            return Ok();
        }
    }
}