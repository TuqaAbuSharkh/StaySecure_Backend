using Azure;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StaySecure.BLL.Services.IServices;
using StaySecure.DAL.Data;
using StaySecure.DAL.DTOs.Request;
using StaySecure.DAL.DTOs.Response;
using StaySecure.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.BLL.Services
{
    public class AuthenticationService : IAuthinticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;

        public AuthenticationService(UserManager<ApplicationUser> userManager, IConfiguration configuration,
            IEmailSender emailSender, SignInManager<ApplicationUser> signInManager, ITokenService tokenService,
            IHttpContextAccessor httpContextAccessor, ApplicationDbContext context)
        {
            _userManager = userManager;
            _configuration = configuration;
            _emailSender = emailSender;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }
        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
                var user = await _userManager.FindByEmailAsync(request.Email);

                if (user == null)
                {
                    await LogLoginAttempt(null, request.Email, false, ipAddress, "Invalid Email");
                    return new LoginResponse { Success = false, Message = "Invalid Email!" };
                }

                if (await _userManager.IsLockedOutAsync(user))
                {
                    await LogLoginAttempt(user.Id, user.Email, false, ipAddress, "Account Locked");
                    return new LoginResponse { Success = false, Message = "Account is locked!" };
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, true);

                if (!result.Succeeded)
                {
                    await LogLoginAttempt(user.Id, user.Email, false, ipAddress, "Invalid Password");
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Invalid credentials"
                    };
                }

                if (user.TwoFactorEnabled)
                {
                    return new LoginResponse
                    {
                        Success = true,
                        RequiresTwoFactor = true,
                        Message = "2FA required",
                        UserId = user.Id
                    };
                }

                // تسجيل دخول عادي بدون 2FA
                return await GenerateTokens(user, ipAddress);
            }
            catch (Exception ex)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Exception Error!",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        // دالة مساعدة لتسجيل الـ login
        private async Task LogLoginAttempt(string userId, string email, bool success, string ipAddress, string failureReason = null)
        {
            _context.LoginLogs.Add(new LoginLog
            {
                UserId = userId,
                Email = email,
                AttemptTime = DateTime.UtcNow,
                Success = success,
                IpAddress = ipAddress,
                FailureReason = failureReason
            });

            await _context.SaveChangesAsync();
        }

        public async Task<LoginResponse> VerifyTwoFactorAsync(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

            if (user == null)
            {
                return new LoginResponse { Success = false, Message = "User not found" };
            }

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(
                user,
                _userManager.Options.Tokens.AuthenticatorTokenProvider,
                code
            );

            if (!isValid)
            {
                await LogLoginAttempt(user.Id, user.Email, false, ipAddress, "Invalid 2FA Code");

                return new LoginResponse
                {
                    Success = false,
                    Message = "Invalid 2FA code"
                };
            }
          

            return await GenerateTokens(user, ipAddress);
        }

        public async Task<object> Enable2FAAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                throw new Exception("User not found");

            var key = await _userManager.GetAuthenticatorKeyAsync(user);

            if (string.IsNullOrEmpty(key))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                key = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            var qrCodeUri = $"otpauth://totp/StaySecure:{user.Email}?secret={key}&issuer=StaySecure&digits=6";

            return new
            {
                sharedKey = key,
                qrCodeUri = qrCodeUri
            };
        }

        public async Task<string> Confirm2FAAsync(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return "User not found";

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(
                user,
                _userManager.Options.Tokens.AuthenticatorTokenProvider,
                code
            );

            if (!isValid)
                return "Invalid Code";

            await _userManager.SetTwoFactorEnabledAsync(user, true);

            return "2FA Enabled Successfully";
        }


        private async Task<LoginResponse> GenerateTokens(ApplicationUser user, string ipAddress)
        {
            var accessToken = await _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await _userManager.UpdateAsync(user);

            await LogLoginAttempt(user.Id, user.Email, true, ipAddress, null);

           
            return new LoginResponse
            {
                Success = true,
                Message = "Login Successfully",
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
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
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                var emailUrl = $"https://staysecure.runasp.net/api/auth/Account/ConfirmEmail?token={encodedToken}&userId={user.Id}";

                await _emailSender.SendEmailAsync(
                    user.Email,
                    "Welcome To StaySecure",
                    $"<h1>Welcome {user.UserName}</h1>" +
                    "<br/> Please Confirm Your Email<br/>" +
                    $"<a href='{emailUrl}'>Confirm Email</a>"
                );
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

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            return result.Succeeded;
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
            var refreshToken = _httpContextAccessor.HttpContext.Request.Cookies["refreshToken"];

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

        public async Task<bool> LogoutAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
                return false;

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            await _userManager.UpdateAsync(user);

            return true;
        }




    }
}

