using GeoRouting.AppLayer.DTO;
using GeoRouting.AppLayer.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GeoRouting.AppLayer.Services
{
    public interface IAttributeService
    {
        Task<IEnumerable<Category>> GetCategories();
        Task<Model.Entities.Attribute> AddPointAttribute(int userId, PointAttributeInput attributeInput);
        Task<Model.Entities.Attribute> AddLongAttribute(int userId, LongAttributeInput attributeInput);
        Task<WayAttributesDTO> GetUserAttributes(int userId);
        Task RemoveAttribute(int attributeId, int userId);
    }
}
