using Shared.Common;
using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IOrderService
    {
        ResponsePackage<bool> AddOrder(NewOrderDTO orderDTO);
        ResponsePackage<bool> UpdateOrder(long id);
        ResponsePackage<IEnumerable<OrderDTO>> GetAll();
        ResponsePackage<IEnumerable<OrderDTO>> GetByUser(long UserId);
        ResponsePackage<IEnumerable<OrderDTO>> GetHistory(long UserId);
        ResponsePackage<IEnumerable<OrderDTO>> GetNew(long UserId);
        ResponsePackage<bool> CancelOrder(long id);
        bool MarkAsShiped(long id);
    }
}
