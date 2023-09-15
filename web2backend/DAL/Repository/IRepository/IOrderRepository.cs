using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository.IRepository
{
    public interface IOrderRepository : IRepository<Order>
    {
        IEnumerable<Order> GetHistory(long id);
        IEnumerable<Order> GetNew(long Id);
        void Save();
    }
}
