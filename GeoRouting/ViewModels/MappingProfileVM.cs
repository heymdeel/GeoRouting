using AutoMapper;
using GeoRouting.AppLayer.Model;
using GeoRouting.AppLayer.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeoRouting.ViewModels
{
    public class MappingProfileVM : Profile
    {
        public MappingProfileVM()
        {
            CreateMap<(string access_token, User user), TokenVM>()
                .ForMember(vm => vm.AccessToken, t => t.MapFrom(o => o.access_token))
                .ForMember(vm => vm.Id, t => t.MapFrom(o => o.user.Id))
                .ForMember(vm => vm.Roles, t => t.MapFrom(o => o.user.Roles));

            CreateMap<Category, CategoryVM>();
            CreateMap<AppLayer.Model.Entities.Attribute, CreatedAttributeVM>();
        }
    }
}
