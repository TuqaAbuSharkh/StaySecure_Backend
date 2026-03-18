using StaySecure.DAL.DTOs.Request;
using StaySecure.DAL.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.BLL.Services.IServices
{
    public interface IAuthinticationService
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);
        Task<LoginResponse> LoginAsync(LoginRequest request);

        Task<bool> ConfirmEmailAsync(string token, string userId);

        Task<ForgetPasswordResponse> RequestPassworsReset(ForgetPasswordRequest request);

        Task<ResetPasswordResponse> ResetPassword(ResetPasswordRequest request);

        Task<LoginResponse> RefreshTokenAsync(TokenApiModel request);

    }
}
