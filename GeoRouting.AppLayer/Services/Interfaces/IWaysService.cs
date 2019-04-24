using GeoRouting.AppLayer.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GeoRouting.AppLayer.Services
{
    public interface IWaysService
    {
        Task<WayAttributesDTO> GetWaysAttributes(int wayId);
    }
}
