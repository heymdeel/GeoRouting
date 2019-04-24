using AutoMapper;
using GeoRouting.AppLayer.DTO;
using GeoRouting.AppLayer.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeoRouting.AppLayer
{
    internal class MappingProfileApplayer : Profile
    {
        public MappingProfileApplayer()
        {
            CreateMap<SignUpInput, User>();
            CreateMap<PointAttributeInput, Model.Entities.Attribute>();
        }
    }
}
