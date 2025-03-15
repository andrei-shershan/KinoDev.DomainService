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

        [HttpPost]
        public async Task<IActionResult> CreateOrderAsync([FromBody] CreateOrderModel orderModel)
        {
            System.Console.WriteLine("CreateOrderAsync");
            var result = await _orderServcie.CreateOrderAsync(orderModel);
            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }
    }
}