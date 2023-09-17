﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IUserRepository User { get; }
        IItemRepository Item { get; }
        IOrderRepository Orders { get; }
        void Save();
    }
}
