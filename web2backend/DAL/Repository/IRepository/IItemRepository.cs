using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository.IRepository
{
    public interface IItemRepository : IRepository<Item>
    {
        void Update(Item obj);
        void Delete(Item obj);
        void Save();
    }
}
