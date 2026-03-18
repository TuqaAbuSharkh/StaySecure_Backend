using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using StaySecure.BLL.Services.IServices;
using StaySecure.DAL.DTOs.Request;
using StaySecure.DAL.DTOs.Response;
using StaySecure.DAL.Models;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace StaySecure.BLL.Services
{
    public class AuthenticationService : IAuthinticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;

        public AuthenticationService(UserManager<ApplicationUser> userManager, IConfiguration configuration,
            IEmailSender emailSender, SignInManager<ApplicationUser> signInManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _emailSender = emailSender;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }
        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user is null)
                {
                    return new LoginResponse()
                    {
                        Success = false,
                        Message = "Invalid Email!",
                    };
                }

                if (await _userManager.IsLockedOutAsync(user))
                {
                    return new LoginResponse()
                    {
                        Success = false,
                        Message = "Accoun is locked! try again later",
                    };
                }
                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, true);
                if (result.IsLockedOut)
                {
                    return new LoginResponse()
                    {
                        Success = false,
                        Message = "Accoun is locked! try again later",
                    };
                }
                else if (result.IsNotAllowed)
                {
                    return new LoginResponse()
                    {
                        Success = false,
                        Message = "please confirm your email",
                    };
                }
                else if (!result.Succeeded)
                {
                    return new LoginResponse()
                    {
                        Success = false,
                        Message = "invalid password",
                    };
                }

                var accessToken = await _tokenService.GenerateAccessToken(user);
                var refreshToken = _tokenService.GenerateRefreshToken();
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
                await _userManager.UpdateAsync(user);

                return new LoginResponse()
                {
                    Success = true,
                    Message = "Login Successfully",
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                };

            }
            catch (Exception ex)
            {
                return new LoginResponse()
                {
                    Success = false,
                    Message = "An Exception Error!",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var user = request.Adapt<ApplicationUser>();

                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    return new RegisterResponse()
                    {
                        Success = false,
                        Message = "User Creation Faild!",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    };
                }
                await _userManager.AddToRoleAsync(user, "Student");
                user.UserName = user.Translations.FirstOrDefault(t => t.Language == "en").FullName;
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                token = Uri.EscapeDataString(token);
               var emailUrl = $"https://localhost:7254/api/auth/Account/ConfirmEmail?token={token}&userId={user.Id}";
              await _emailSender.SendEmailAsync(user.Email, "Welcome To StaySecure", $"<h1>Welcome {user.UserName}</h1>" +" \n Please Confirm Your Email"+
                    $"<a href='{emailUrl}'> Confirm Email </a>");

                return new RegisterResponse()
                {
                    Success = true,
                    Message = "Success"
                };
            }
            catch (Exception ex)
            {
                return new RegisterResponse()
                {
                    Success = false,
                    Message = "An Exception Error!",
                    Errors = new List<string> { ex.Message }
                };
            }

        }

        public async Task<bool> ConfirmEmailAsync(string token, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return false;
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
                return false;
            return true;
        }

        public async Task<ForgetPasswordResponse> RequestPassworsReset(ForgetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                return new ForgetPasswordResponse
                {
                    Success = false,
                    Message = "Email not found"
                };
            }

            var random = new Random();
            var code = random.Next(0000, 9999).ToString();
            user.CodeResetPassword = code;
            user.PasswordResetCodeExpiry = DateTime.UtcNow.AddMinutes(5);

            await _userManager.UpdateAsync(user);

            await _emailSender.SendEmailAsync(request.Email, "Reset Password", $"<p> code is {code}</p>");

            return new ForgetPasswordResponse
            {
                Success = true,
                Message = "code sent to your email"
            };
        }

        public async Task<ResetPasswordResponse> ResetPassword(ResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                return new ResetPasswordResponse
                {
                    Success = false,
                    Message = "Email not found"
                };
            }
            else if (user.CodeResetPassword != request.code)
            {
                return new ResetPasswordResponse
                {
                    Success = false,
                    Message = "invalid code"
                };
            }
            else if (user.PasswordResetCodeExpiry < DateTime.UtcNow)
            {
                return new ResetPasswordResponse
                {
                    Success = false,
                    Message = "code Expired"
                };
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, request.newPassword);

            if (!result.Succeeded)
            {
                return new ResetPasswordResponse
                {
                    Success = false,
                    Message = "password reset failed!",
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };
            }
            await _emailSender.SendEmailAsync(request.Email, "Changed Password", $"<p> your password is changed</p>");

            user.CodeResetPassword = null;
            await _userManager.UpdateAsync(user);
            return new ResetPasswordResponse
            {
                Success = true,
                Message = "password reset successfully"
            };
        }


        public async Task<LoginResponse> RefreshTokenAsync(TokenApiModel request)
        {
            string accessToken = request.AccessToken;
            string refreshToken = request.RefreshToken;

            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            var username = principal.Identity.Name;
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == username);

            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return new LoginResponse()
                {
                    Success = false,
                    Message = "invalid client request "
                };
            }
            var newAccessToken = await _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);

            return new LoginResponse()
            {
                Success = true,
                Message = "Token Refreshed ",
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }

    }
}

