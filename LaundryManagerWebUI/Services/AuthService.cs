using LaundryManagerAPIDomain.Contracts;
using LaundryManagerAPIDomain.Entities;
using LaundryManagerAPIDomain.Services;
using LaundryManagerWebUI.Dtos;
using LaundryManagerWebUI.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace LaundryManagerWebUI.Services
{
    public enum AuthServiceResult
    {
        Succeeded,Failed
    }
    public class AuthService : IAuthService
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signManager;
        private IJWTManager _jwtmanager;
        private IIdentityQuery _userRepo;
        private ISaveChanges _unitOFWork;
        public AuthService(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJWTManager jwtmanager,
            IIdentityQuery userRepo,
            ISaveChanges unitOfWork)
        {
            _userManager = userManager;
            _signManager = signInManager;
            _jwtmanager = jwtmanager;
            _userRepo = userRepo;
            _unitOFWork = unitOfWork;
        }

        public async Task<ResponseDto<AuthServiceResult, string>> CreateLaundry(RegisterDto model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Username,
                Profile = new UserProfile()
                {
                    Name = model.OwnerName,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Laundry = new Laundry
                    {
                        Name = model.LaundryName,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        Address = new Location
                        {
                            State = model.Address.State,
                            LGA = model.Address.State,
                            Country = "Nigeria",
                            Street = model.Address.Street
                        }

                    }
                }
            };

            var result = await _userManager.CreateAsync(user, model.Password); ;
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, RoleNames.Owner);
                return new ResponseDto<AuthServiceResult, string> { };
            }

            return new ResponseDto<AuthServiceResult, string>
            {

            };
        }


        public async Task<ResponseDto<AuthServiceResult, string>> Authenticate(LoginDto model)
        {
            var result = await _signManager.PasswordSignInAsync(model.Username, model.Password, false, false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Username);
                var userRole = _userRepo.GetUserRole(user);
                var token = _jwtmanager.GetToken(new JWTDto { UserEmail = model.Username, UserId = user.Id, UserRole = userRole });

                var randomNumber = new byte[32];
                using (var generator = RandomNumberGenerator.Create())
                {
                    generator.GetBytes(randomNumber);
                    user.RefreshToken = Convert.ToBase64String(randomNumber);
                    await _unitOFWork.SaveAsync();
                }

                return new ResponseDto<AuthServiceResult, string>
                {
                    Result = AuthServiceResult.Succeeded,
                    Data = JsonConvert.SerializeObject(new { Token = token, RefreshToken = user.RefreshToken }),
                };
            }

            return new ResponseDto<AuthServiceResult, string>
            {
                Result = AuthServiceResult.Failed,
                Data = JsonConvert.SerializeObject(new { Errors = "some error occurred" }),
            }; ;

        }
    }
}
