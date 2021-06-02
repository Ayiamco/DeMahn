using LaundryManagerAPIDomain.Contracts;
using LaundryManagerAPIDomain.Entities;
using LaundryManagerAPIDomain.Services;
using LaundryManagerAPIDomain.Services.EmailService;
using LaundryManagerWebUI.Dtos;
using LaundryManagerWebUI.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Threading.Tasks;

namespace LaundryManagerWebUI.Services
{
    public enum AppServiceResult
    {
        Succeeded,Failed,Unknown,Prohibited
    }
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signManager;
        private readonly IJWTManager _jwtmanager;
        private readonly IIdentityQuery _userRepo;
        private readonly IUnitOfWork _unitOFWork;
        private readonly IEmailSender _mailService;
        private readonly IConfiguration _config;
        private readonly IEmployeeInTransitQuery _employeeInTransitRepo;
        private readonly ILaundryQuery laundryRepo;

        public AuthService(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJWTManager jwtmanager,
            IIdentityQuery userRepo,
            IUnitOfWork unitOfWork,
            IEmailSender mailService,
            IConfiguration config,
            IEmployeeInTransitQuery employeeInTransitRepo,
            ILaundryQuery laundryRepo
            )
        {
            _userManager = userManager;
            _signManager = signInManager;
            _jwtmanager = jwtmanager;
            _userRepo = userRepo;
            _unitOFWork = unitOfWork;
            _mailService = mailService;
            _config = config;
            _employeeInTransitRepo = employeeInTransitRepo;
            this.laundryRepo = laundryRepo;
        }

        public async Task<ServiceResponse> CreateLaundry(RegisterDto model)
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
                            Country = model.Address.Country,
                            Street = model.Address.Street
                        }

                    }
                }
            };

            var result = await _userManager.CreateAsync(user, model.Password); ;
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, RoleNames.Owner);
                return new ServiceResponse { };
            }

            return new ServiceResponse
            {

            };
        }

        public async Task<ServiceResponse> Authenticate(LoginDto model)
        {
            var result = await _signManager.PasswordSignInAsync(model.Username, model.Password, false, false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Username);
                var userRole = _userRepo.GetUserRole(user);
                var token = _jwtmanager.GetToken(new JWTDto { UserEmail = model.Username, UserId = user.Id, UserRole = userRole });
                var laundry = await laundryRepo.GetLaundryByUserId(new Guid(user.Id));
                await UpdateRefreshToken(user);
                return new ServiceResponse
                {
                    Result = AppServiceResult.Succeeded,
                    Data = JsonConvert.SerializeObject(new { 
                        JwtToken = token,
                        RefreshToken = user.RefreshToken,
                        LaundryName = laundry.Name,
                        LaundryId= laundry.Id

                    }),
                };
            }

            return new ServiceResponse
            {
                Result = AppServiceResult.Failed,
                Data = JsonConvert.SerializeObject(new { Errors = "some error occurred" }),
            }; ;

        }

        public async Task<ServiceResponse> RefreshJWtToken(JWTDto model)
        {
            try
            {
                var claim =_jwtmanager.GetPrincipalFromExpiredToken(model);
                var user = await _userManager.FindByEmailAsync(claim.Identity.Name); 
                if (user.RefreshToken != model.RefreshToken) return new ServiceResponse
                {
                    Result = AppServiceResult.Failed,
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

                return new ServiceResponse
                {
                    Result = AppServiceResult.Succeeded,
                    Data = JsonConvert.SerializeObject(new
                    {
                        JwtToken= newJwt,
                        RefreshToken=user.RefreshToken
                    })
                };
            }
            catch (SecurityTokenException)
            {
                return new ServiceResponse
                {
                    Result = AppServiceResult.Failed,
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
                return new ServiceResponse
                {
                    Result = AppServiceResult.Unknown,
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

        public async Task SendResetPasswordLink(string username)
        {
            var user = await _userManager.FindByEmailAsync(username);
            var passwordresetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var url = _config[AppConstants.ClientBaseUrl] + $"account/confirm-password-reset/?passwordToken={passwordresetToken},username={user.Email}";
            await _mailService.SendEmailAsync(new Message(new List<string> { username },"Password Reset",
                $"<h3>Hi<h3>, <p>Please click <a href={url}>here</a>. Link expires in 10 mins"),
                IsHTML:true);
        }

        public async Task<ServiceResponse> ResetPassword(ConfirmPasswordResetDto model)
        {

            var user = await _userManager.FindByEmailAsync(model.Username);
            if (user == null) return new ServiceResponse
            {
                Result = AppServiceResult.Failed,
                Data= JsonConvert.SerializeObject( new 
                {
                    Errors = new
                    {
                        Email = new string[] { "User does not exist" }
                    },
                })
            };
                
            var resetPassResult = await _userManager.ResetPasswordAsync(user, model.PasswordToken, model.Password);
            var obj = new Dictionary<string, string[]>();
            if (!resetPassResult.Succeeded)
            {
                foreach (var error in resetPassResult.Errors)
                {
                    obj.Add(error.Code, new string[] { error.Description });
                }
                return new ServiceResponse
                {
                    Result = AppServiceResult.Failed,
                    Data = JsonConvert.SerializeObject(new { Errors = obj })
                };
            }

            return new ServiceResponse
            {
                Result=AppServiceResult.Succeeded,
                Data= JsonConvert.SerializeObject( new { Message="Password change was successful."})
            };
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
   
