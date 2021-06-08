using AutoMapper;
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
        private readonly IMapper mapper;

        public AuthService(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJWTManager jwtmanager,
            IIdentityQuery userRepo,
            IUnitOfWork unitOfWork,
            IEmailSender mailService,
            IConfiguration config,
            IEmployeeInTransitQuery employeeInTransitRepo,
            ILaundryQuery laundryRepo,
            IMapper mapper
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
            this.mapper = mapper;
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
                    UpdatedAt = DateTime.Now
                },
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
            };

            var result = await _userManager.CreateAsync(user, model.Password); ;
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, RoleNames.Owner);
                await SendConfirmationEmail(user);
                return new ServiceResponse
                { 
                    Result= AppServiceResult.Succeeded,
                    Data= user.Laundry.Id.ToString() 
                };
            }

            return new ServiceResponse
            {
                Result=AppServiceResult.Failed,
                Data= JsonConvert.SerializeObject( new { errors = GetErrors(result.Errors)})
            };
        }

        public async Task<ServiceResponse> Authenticate(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Username);
            var result = await _signManager.PasswordSignInAsync(model.Username, model.Password, false, false);
            if (result.Succeeded)
            {  
                var userRole = _userRepo.GetUserRole(user);
                var token = _jwtmanager.GetToken(new JWTDto { UserEmail = model.Username, UserId = user.Id, UserRole = userRole });
                var laundry = await laundryRepo.GetLaundryByUserId(new Guid(user.Id));
                await UpdateRefreshToken(user);
                return new ServiceResponse
                {
                    Result = AppServiceResult.Succeeded,
                    Data = JsonConvert.SerializeObject(new { data = new
                    {
                        jwtToken = token,
                        refreshToken = user.RefreshToken,
                        laundryName = laundry.Name,
                        laundryId = laundry.Id,
                        id=user.Id,
                        profileId=user.ProfileId

                    }}),
                };
            }

            if (user== null) return new ServiceResponse
            {
                Result = AppServiceResult.Failed,
                Data = JsonConvert.SerializeObject(new 
                { errors =  new { username= new string[] { "user does not exist" } } }),
            };

            var response = new ServiceResponse { Result = AppServiceResult.Failed };
            if (result.IsNotAllowed)
            {
                await SendConfirmationEmail(user);
                response.Data = JsonConvert.SerializeObject(new
                {
                    errors =
                    new
                    {
                        account = new string[] { "account is not confirmed" },
                        message = "check your mail for confirmation link"
                    }
                });
            }

            if (user != null && !result.IsLockedOut && !result.IsNotAllowed) return new ServiceResponse
            {
                Result = AppServiceResult.Failed,
                Data = JsonConvert.SerializeObject(new
                { errors = new { password = new string[] { "password is incorrect" } } }),
            };

            return new ServiceResponse
            {
                Result = AppServiceResult.Unknown,
                Data = JsonConvert.SerializeObject(new
                { errors = new { server = new string[] { "internal server error" } } }),
            }; ;

        }

        public async Task<ServiceResponse> ConfirmEmail(string token, string userId)
        {
            var user=await _userManager.FindByIdAsync(userId);
            if (user == null) return new ServiceResponse
            {
                Result = AppServiceResult.Failed,
                Data = JsonConvert.SerializeObject(new
                { errors = new { username = new string[] { "user does not exist" } } }),
            }; 
            var result= await _userManager.ConfirmEmailAsync(user,token);
            if (result.Succeeded) return new ServiceResponse
            {
                Result = AppServiceResult.Succeeded
            };

            return new ServiceResponse
            {
                Result = AppServiceResult.Unknown,
                Data = JsonConvert.SerializeObject(new
                { message="unknown error occured" }),
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
                        errors = new
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
                        data = new
                        {
                            jwtToken = newJwt,
                            refreshToken = user.RefreshToken
                        }
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
                        errors = new 
                        { 
                            jwtToken=  new string[] { "Invalid jwt token" }
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
                        errors = new
                        {
                            serverError = new string[] { "Unknown server error occured" }
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
                    errors = new
                    {
                       username = new string[] { "User does not exist" }
                    },
                })
            };
                
            var resetPassResult = await _userManager.ResetPasswordAsync(user, model.PasswordToken, model.Password);
            if (!resetPassResult.Succeeded)
            {
                return new ServiceResponse
                {
                    Result = AppServiceResult.Failed,
                    Data = JsonConvert.SerializeObject(new { errors = GetErrors(resetPassResult.Errors) })
                };
            }

            return new ServiceResponse
            {
                Result=AppServiceResult.Succeeded,
                Data= JsonConvert.SerializeObject( new {  message="Password change was successful."})
            };
        }

        public async Task<ServiceResponse> AddEmployee(NewEmployeeDto model)
        {
            var empInTransit=_employeeInTransitRepo.Find(x => x.Username == model.Username)
                .AsQueryable().SingleOrDefault();
            
            if (empInTransit == null || empInTransit.Id != model.Id) return new ServiceResponse
            {
                Result= AppServiceResult.Failed,
                Data= JsonConvert.SerializeObject(new
                {
                    errors= new
                    { 
                        Username= new string[] {"user has not being added by laundry owner"}
                    }
                    
                })
            };

            var user= mapper.Map<ApplicationUser>(model);
            user.LaundryId = empInTransit.LaundryId;
            user.Profile = new UserProfile {Name=model.Name };
            var result=await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, RoleNames.Employee);
                return new ServiceResponse
                {
                    Result = AppServiceResult.Succeeded
                };
            }


            return new ServiceResponse() { Result= AppServiceResult.Failed};
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

        private async Task SendConfirmationEmail(ApplicationUser user)
        {
            var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = $"{_config[AppConstants.ClientBaseUrl]}confirmEmail" +
                $"?confirmationToken={confirmEmailToken},id={user.Id}"; ;
            var message=new Message(new List<string> { user.Email },$"{user.Laundry.Name} email confirmation",
                @$"
                    <h4>Hi,</h4>
                    <p>please click on this <a href={confirmationLink}>link</a> to confirm your email and complete your laundry 
                        registration, thanks.
                    </p>
                    
                ");
            await _mailService.SendEmailAsync(message,IsHTML: true); 
        }
        private Dictionary<string,string[]> GetErrors(IEnumerable<IdentityError> errors)
        {
            var obj = new Dictionary<string, string[]>();
            foreach (var error in errors)
            {
                if (obj.ContainsKey(error.Code)) obj[error.Code] = (string[])obj[error.Code].Append(error.Description);
                else obj.Add(error.Code, new string[] { error.Description });
            }
            return obj;
        }
    }

}
   
