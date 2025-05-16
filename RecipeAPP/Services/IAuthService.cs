using RecipeApp.API.DTOs;
using RecipeApp.API.Models;

namespace RecipeApp.API.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto> GoogleLoginWithClerkAsync(string clerkToken);

        string GenerateJwtToken(User user);
    }
}
