using DAL.Context;
using DAL.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            User = new UserRepository(_db);
            Item = new ItemRepository(_db);
            Orders = new OrderRepository(_db);
        }

        public IUserRepository User { get; set; }
        public IItemRepository Item { get; set; }
        public IOrderRepository Orders { get; set; }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
