using Shared.Common;
using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IItemService
    {
        ResponsePackage<ItemDTO> GetItem(int id, string? includeProperties = null);
        ResponsePackage<bool> AddItem(NewItemDTO itemDTO, string filePath);
        ResponsePackage<bool> UpdateItem(ItemDTO itemDTO);
        ResponsePackage<IEnumerable<ItemDTO>> GetAll(string? includeProperties = null);
        ResponsePackage<IEnumerable<ItemDTO>> GetByUser(int UserId, string? includeProperties = null);
        ResponsePackage<bool> DeleteItem(int id);
    }
}
