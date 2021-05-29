using LaundryManagerAPIDomain.Contracts;
using LaundryManagerAPIDomain.Entities;
using LaundryManagerAPIDomain.Services;
using LaundryManagerWebUI.Dtos;
using LaundryManagerWebUI.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
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
                await UpdateRefreshToken(user);
                return new ResponseDto<AuthServiceResult, string>
                {
                    Result = AuthServiceResult.Succeeded,
                    Data = JsonConvert.SerializeObject(new { JwtToken = token, RefreshToken = user.RefreshToken }),
                };
            }

            return new ResponseDto<AuthServiceResult, string>
            {
                Result = AuthServiceResult.Failed,
                Data = JsonConvert.SerializeObject(new { Errors = "some error occurred" }),
            }; ;

        }

        public async Task<ResponseDto<AuthServiceResult, string>> RefreshJWtToken(JWTDto model)
        {
            try
            {
                var claim =_jwtmanager.GetPrincipalFromExpiredToken(model);
                var claims = claim.Claims.ToList();
                var user = await _userManager.FindByEmailAsync(claims[2].Value); 
                if (user.RefreshToken != model.RefreshToken) return new ResponseDto<AuthServiceResult, string>
                {
                    Result = AuthServiceResult.Failed,
                    Data = JsonConvert.SerializeObject(new
                    {
                        Errors = new
                        {
                            refreshToken = new string[] { "refresh token has being changed, go to login" }
                        },

                    })
                };

                model.UserEmail = user.Email;
                model.UserId = user.Id;
                model.UserRole = RoleNames.Owner;
                var newJwt = _jwtmanager.GetToken(model);
                await UpdateRefreshToken(user);

                return new ResponseDto<AuthServiceResult, string>
                {
                    Result = AuthServiceResult.Succeeded,
                    Data = JsonConvert.SerializeObject(new
                    {
                        JwtToken= newJwt,
                        RefreshToken=user.RefreshToken
                    })
                };
            }
            catch (SecurityTokenException)
            {
                return new ResponseDto<AuthServiceResult, string>
                {
                    Result = AuthServiceResult.Failed,
                    Data = JsonConvert.SerializeObject(new
                    {
                        Errors = new 
                        { 
                            JwtToken=  new string[] { "Invalid jwt token" }
                        },
                       
                    })
                };
            }
            catch
            {
                return new ResponseDto<AuthServiceResult, string>
                {
                    Result = AuthServiceResult.Failed,
                    Data = JsonConvert.SerializeObject(new
                    {
                        Errors = new
                        {
                            ServerError = new string[] { "Unknown server error occured" }
                        },

                    })
                };
            }
        }

        private async Task UpdateRefreshToken(ApplicationUser user)
        {
            var randomNumber = new byte[32];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomNumber);
                user.RefreshToken = Convert.ToBase64String(randomNumber);
                await _unitOFWork.SaveAsync();
            }
        }
    }

}
   
