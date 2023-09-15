using DAL.Context;
using DAL.Model;
using DAL.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class ItemRepository : Repository<Item>, IItemRepository
    {
        private ApplicationDbContext _db;

        public ItemRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Delete(Item obj)
        {
            var objFromDb = _db.Items.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                _db.Items.Remove(objFromDb);
            }
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Update(Item obj)
        {
            var objFromDb = _db.Items.FirstOrDefault(u => u.Id == obj.Id);
            if(objFromDb != null)
            {
                objFromDb.Name = obj.Name;
                objFromDb.Price = obj.Price;
                objFromDb.Description = obj.Description;
                objFromDb.Amount = obj.Amount;
                objFromDb.PictureUrl = obj.PictureUrl;
            }
        }
    }
}
