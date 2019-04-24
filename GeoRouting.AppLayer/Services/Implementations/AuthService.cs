using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GeoRouting.AppLayer.Config;
using GeoRouting.AppLayer.DTO;
using GeoRouting.AppLayer.Exceptions;
using GeoRouting.AppLayer.Model;
using LinqToDB;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using GeoRouting.AppLayer.Model.Entities;

namespace GeoRouting.AppLayer.Services
{
    public class AuthService : IAuthService
    {
        private readonly IMapper mapper;
        private readonly AuthOptions authOptions;

        public AuthService(IMapper mapper, IOptions<AuthOptions> authOptions)
        {
            this.mapper = mapper;
            this.authOptions = authOptions.Value;
        }

        public async Task SignUpUserAsync(SignUpInput userData)
        {
            using (var db = new DbContext())
            {
                if (await db.Users.AnyAsync(u => u.Email == userData.EMail))
                {
                    throw new BadInputException(100, "user already exists");
                }
            }

            var user = mapper.Map<User>(userData);
            user.Roles = new[] { "client"};
            user.Hash = GeneratePassword(userData.EMail, userData.Password);
            user.DateOfRegistration = DateTime.Now;

            using (var db = new DbContext())
            {
                await db.InsertAsync(user);
            }
        }

        public async Task<User> SignInUserAsync(SignInInput userData)
        {
            using (var db = new DbContext())
            {
                string hash = GeneratePassword(userData.EMail, userData.Password);

                var user = await db.Users.FirstOrDefaultAsync(u => u.Email == userData.EMail && u.Hash == hash);
                if (user == null)
                {
                    throw new BadInputException(102, "user was not found");
                }

                return user;
            }
        }

        public string GenerateToken(User user)
        {
            ClaimsIdentity identity = GetIdentity(user);

            return GenerateToken(identity);
        }

        private ClaimsIdentity GetIdentity(User user)
        {
            var claims = new List<Claim>
            {
                new Claim("user_id", user.Id.ToString())
            };

            claims.AddRange(user.Roles.Select(r => new Claim(ClaimsIdentity.DefaultRoleClaimType, r)));

            return new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        }

        private string GenerateToken(ClaimsIdentity identity)
        {
            var jwt = new JwtSecurityToken(
                    issuer: authOptions.Issuer,
                    audience: authOptions.Audience,
                    notBefore: DateTime.Now,
                    claims: identity.Claims,
                    expires: DateTime.Now.AddMinutes(AuthOptions.ACCESS_LIFETIME),
                    signingCredentials: new SigningCredentials(authOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        private string GeneratePassword(string arg1, string arg2)
        {
            SHA512 sha512 = SHA512.Create();

            byte[] inputBytes = Encoding.ASCII.GetBytes(arg1 + "pepper" + arg2);
            byte[] hash = sha512.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }

            return sb.ToString();
        }
    }
}
