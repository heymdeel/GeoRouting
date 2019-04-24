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
    }
}
