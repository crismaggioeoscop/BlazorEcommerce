using BlazorApp1.Shared.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApp1.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<Order>>>> GetOrders()
        {
            return Ok(await _orderService.GetOrders());
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<ServiceResponse<OrderDetailsProductResponse>>> GetOrderDetails(int orderId)
        {
            return Ok(await _orderService.GetOrderDetails(orderId));
        }
    }
}