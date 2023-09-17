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
    public class ItemService : IItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ItemService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public ResponsePackage<bool> AddItem(NewItemDTO itemDTO, string filePath)
        {
            Item item = _mapper.Map<Item>(itemDTO);
            item.PictureUrl = filePath;
            try
            {
                _unitOfWork.Item.Add(item);
                _unitOfWork.Save();
                return new ResponsePackage<bool>(true, ResponseStatus.OK, "Item added successfully");
            }
            catch(Exception ex)
            {
                return new ResponsePackage<bool>(false, ResponseStatus.InternalServerError, "There was an error while adding an item:" + ex.Message);
            }
        }

        public ResponsePackage<bool> DeleteItem(int id)
        {
            throw new NotImplementedException();
        }

        public ResponsePackage<IEnumerable<ItemDTO>> GetAll(string? includeProperties = null)
        {
            var list = _unitOfWork.Item.GetAll(includeProperties:  includeProperties);
            var retList = new List<ItemDTO>();
            foreach (var item in list)
            {
                ItemDTO retItem = _mapper.Map<ItemDTO>(item);
                byte[] imageBytes = System.IO.File.ReadAllBytes(item.PictureUrl);
                retItem.PictureUrl = Convert.ToBase64String(imageBytes);
                retList.Add(retItem);
            }

            return new ResponsePackage<IEnumerable<ItemDTO>> (retList, ResponseStatus.OK, "All items");
        }

        public ResponsePackage<IEnumerable<ItemDTO>> GetByUser(int UserId, string? includeProperties = null)
        {
            throw new NotImplementedException();
        }

        public ResponsePackage<ItemDTO> GetItem(int id, string? includeProperties = null)
        {
            throw new NotImplementedException();
        }

        public ResponsePackage<bool> UpdateItem(ItemDTO itemDTO)
        {
            throw new NotImplementedException();
        }
    }
}
