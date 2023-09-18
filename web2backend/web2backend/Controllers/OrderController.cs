using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;
using Shared.Constants;
using Shared.DTOs;
using System.Diagnostics.Eventing.Reader;

namespace web2backend.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {

        public readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("newOrder")]
        [Authorize(Roles = "Buyer")]
        public IActionResult NewOrder([FromBody]NewOrderDTO orderDTO)
        {
            ResponsePackage<bool> response = _orderService.AddOrder(orderDTO);
            if (response.Status == ResponseStatus.OK)
                return Ok(response.Message);
            else
                return Problem(response.Message);
        }

        [HttpGet("myOrders")]
        public IActionResult GetOrders(long id) 
        {
            ResponsePackage<IEnumerable<OrderDTO>> response = _orderService.GetByUser(id);
            if (response.Status == ResponseStatus.OK)
                return Ok(response.Data);
            else
                return Problem(response.Message);
        }

        [HttpGet("newOrders")]
        public IActionResult GetSellersNew(long id)
        {
            ResponsePackage<IEnumerable<OrderDTO>> response = _orderService.GetNew(id);
            if (response.Status == ResponseStatus.OK)
                return Ok(response.Data);
            else
                return Problem(response.Message);
        }

        [HttpGet("allOrders")]
        [Authorize(Roles ="Admin")]
        public IActionResult GetAllOrders()
        {
            ResponsePackage<IEnumerable<OrderDTO>> response = _orderService.GetAll();
            if (response.Status == ResponseStatus.OK)
                return Ok(response.Data);
            else
                return Problem(response.Message);
        }

        [HttpGet("orderHistory")]
        public IActionResult GetSellersHistory(long id)
        {
            ResponsePackage<IEnumerable<OrderDTO>> response = _orderService.GetHistory(id);
            if (response.Status == ResponseStatus.OK)
                return Ok(response.Data);
            else
                return Problem(response.Message);
        }

        [HttpPost("sendItem")]
        
        public IActionResult SendItem(long id)
        {
            ResponsePackage<bool> response = _orderService.UpdateOrder(id);
            if (response.Status == ResponseStatus.OK)
                return Ok(response.Message);
            else
                return Problem(response.Message);
        }
    }
}
