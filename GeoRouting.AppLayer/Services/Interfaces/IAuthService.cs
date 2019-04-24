using GeoRouting.AppLayer.DTO;
using GeoRouting.AppLayer.Model;
using GeoRouting.AppLayer.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GeoRouting.AppLayer.Services
{
    public interface IAuthService
    {
        Task SignUpUserAsync(SignUpInput userData);
        Task<User> SignInUserAsync(SignInInput userData);
        string GenerateToken(User user);
    }
}
