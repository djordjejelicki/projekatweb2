using DAL.Context;
using DAL.Model;
using DAL.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        private ApplicationDbContext _db;
        public OrderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<Order> GetHistory(long id)
        {
            List<Order> allOrders = _db.Orders.Include("OrderItems").ToList();
            List<Order> retList = new List<Order>();
            foreach(Order order in allOrders)
            {
                foreach(OrderItem i in order.OrderItems)
                {
                    if(i.SellerId == id && i.IsSent)
                    {
                        retList.Add(order);
                        break;
                    }
                }
            }
            return retList;
        }

        public IEnumerable<Order> GetNew(long Id)
        {
            List<Order> allOrders = _db.Orders.Include("OrderItems").ToList();
            List<Order> retList = new List<Order>();
            foreach(Order order in allOrders)
            {
                foreach(OrderItem i in order.OrderItems)
                {
                    if(i.SellerId == Id && !i.IsSent)
                    {
                        retList.Add(order);
                        break;
                    }
                }
            }
            return retList;
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
