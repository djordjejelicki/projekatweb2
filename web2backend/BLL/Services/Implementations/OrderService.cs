using AutoMapper;
using BLL.Services.Interfaces;
using DAL.Model;
using DAL.Repository.IRepository;
using Shared.Common;
using Shared.Constants;
using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IShipmentService _shipmentService;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IShipmentService shipmentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _shipmentService = shipmentService;
        }

        public ResponsePackage<bool> AddOrder(NewOrderDTO orderDTO)
        {
            Order order = new Order();
            order.UserId = orderDTO.UserId;
            order.Address = orderDTO.Address;
            order.City = orderDTO.City;
            order.Zip = orderDTO.Zip;
            order.Comment = orderDTO.Comment;
            order.OrderItems = new List<OrderItem>();
            foreach (NewOrderItemDTO oi in orderDTO.Items)
            {
                try
                {
                    Item i = _unitOfWork.Item.GetFirstOrDefault(i => i.Id == oi.Id);
                    if (i.Amount < oi.Amount)
                        throw new Exception("Not enough items");
                    order.OrderItems.Add(new OrderItem()
                    {
                        ItemId = i.Id,
                        IsSent = false,
                        SellerId = i.UserId,
                        Amount = oi.Amount,
                    });
                    i.Amount -= oi.Amount;
                    _unitOfWork.Item.Update(i);

                }
                catch (Exception ex)
                {
                    return new ResponsePackage<bool>(false, ResponseStatus.InternalServerError, "There was an error while adding an order:" + ex.Message);
                }
            }

                try
                {
                    _unitOfWork.Orders.Add(order);
                    _unitOfWork.Save();

                    int arrivalMinutes = _shipmentService.ScheduleShipment(order.Id);

                    return new ResponsePackage<bool>(true, ResponseStatus.OK, $"Order successfully made");
                }
                catch (Exception ex)
                {
                    return new ResponsePackage<bool>(false, ResponseStatus.InternalServerError, "There was an error while adding an order:" + ex.Message);
                }
            
        }

        public ResponsePackage<bool> CancelOrder(long id)
        {
            throw new NotImplementedException();
        }

        public ResponsePackage<IEnumerable<OrderDTO>> GetAll()
        {
            var list = _unitOfWork.Orders.GetAll(includeProperties: "OrderItems");
            var retList = new List<OrderDTO>();
            foreach (var order in list)
            {
                OrderDTO retOrder = _mapper.Map<OrderDTO>(order);
                foreach (OrderItemDTO item in retOrder.OrderItems)
                {
                    Item i = _unitOfWork.Item.GetFirstOrDefault(i => i.Id == item.ItemId);
                    item.Item = _mapper.Map<ItemDTO>(i);
                    item.SellerId = i.UserId;
                }
                retOrder.Canceled = order.Canceled;
                retOrder.Shipped = retOrder.Shipped;
                retOrder.Username = _unitOfWork.User.GetFirstOrDefault(u => u.Id == order.UserId).UserName;
                retList.Add(retOrder);
            }

            return new ResponsePackage<IEnumerable<OrderDTO>>(retList, ResponseStatus.OK, "All orders");
        }

        public ResponsePackage<IEnumerable<OrderDTO>> GetByUser(long UserId)
        {
            var list = _unitOfWork.Orders.GetAll(o => o.UserId == UserId && !o.Canceled, includeProperties: "OrderItems");
            var retList = new List<OrderDTO>();
            foreach (var order in list)
            {
                OrderDTO retOrder = _mapper.Map<OrderDTO>(order);
                foreach (OrderItemDTO item in retOrder.OrderItems)
                {
                    Item i = _unitOfWork.Item.GetFirstOrDefault(i => i.Id == item.ItemId);
                    item.Item = _mapper.Map<ItemDTO>(i);
                    item.SellerId = i.UserId;
                }
                //Get remaining time
                TimeSpan t = _shipmentService.GetRemainingTime(order.Id);
                retOrder.Minutes =  (int)t.TotalMinutes;

                retOrder.Shipped = order.Shipped;
                retOrder.Canceled = order.Canceled;
                retOrder.Username = _unitOfWork.User.GetFirstOrDefault(u => u.Id == order.UserId).UserName;
                retList.Add(retOrder);
            }

            return new ResponsePackage<IEnumerable<OrderDTO>>(retList, ResponseStatus.OK, "All orders for user" + UserId);
        }

        public ResponsePackage<IEnumerable<OrderDTO>> GetHistory(long UserId)
        {
            var list = _unitOfWork.Orders.GetHistory(UserId);
            var retList = new List<OrderDTO>();
            foreach (var order in list)
            {
                OrderDTO retOrder = _mapper.Map<OrderDTO>(order);
                foreach (OrderItemDTO item in retOrder.OrderItems)
                {
                    if (item.SellerId != UserId)
                        retOrder.OrderItems.Remove(item);
                    else
                    {
                        Item i = _unitOfWork.Item.GetFirstOrDefault(i => i.Id == item.ItemId);
                        item.Item = _mapper.Map<ItemDTO>(i);
                        item.SellerId = i.UserId;
                    }
                }
                retOrder.Canceled = order.Canceled;
                retOrder.Shipped = retOrder.Shipped;
                retOrder.Username = _unitOfWork.User.GetFirstOrDefault(u => u.Id == order.UserId).UserName;
                retList.Add(retOrder);
            }

            return new ResponsePackage<IEnumerable<OrderDTO>>(retList, ResponseStatus.OK, "All previous orders for user" + UserId);
        }

        public ResponsePackage<IEnumerable<OrderDTO>> GetNew(long UserId)
        {
            var list = _unitOfWork.Orders.GetNew(UserId);
            var retList = new List<OrderDTO>();
            foreach (var order in list)
            {
                OrderDTO retOrder = _mapper.Map<OrderDTO>(order);
                foreach (OrderItemDTO item in retOrder.OrderItems)
                {
                    Item i = _unitOfWork.Item.GetFirstOrDefault(i => i.Id == item.ItemId);
                    item.Item = _mapper.Map<ItemDTO>(i);
                    item.SellerId = i.UserId;
                }
                //Get remaining time
                retOrder.Minutes = (int)_shipmentService.GetRemainingTime(order.Id).TotalMinutes;
                
                retOrder.Shipped = order.Shipped;
                retOrder.Canceled = order.Canceled;
                retOrder.Username = _unitOfWork.User.GetFirstOrDefault(u => u.Id == order.UserId).UserName;
                retList.Add(retOrder);
            }

            return new ResponsePackage<IEnumerable<OrderDTO>>(retList, ResponseStatus.OK, "All new orders for user" + UserId);
        }

        public bool MarkAsShiped(long id)
        {
            try
            {
                _unitOfWork.Orders.GetFirstOrDefault(o => o.Id == id).Shipped = true;
                _unitOfWork.Save();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public ResponsePackage<bool> UpdateOrder(long id)
        {
            List<Order> orders = _unitOfWork.Orders.GetAll(includeProperties: "OrderItems").ToList();
            foreach (Order order in orders)
            {
                foreach (OrderItem orderItem in order.OrderItems)
                {
                    if (orderItem.Id == id)
                    {
                        orderItem.IsSent = true;
                        _unitOfWork.Orders.Save();
                        return new ResponsePackage<bool>(true, ResponseStatus.OK, "Item sent");
                    }
                }
            }
            return new ResponsePackage<bool>(false, ResponseStatus.NotFound, "Item Not Found");
        }
    }
}
