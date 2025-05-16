using Microsoft.IdentityModel.Tokens;
using RecipeApp.API.Data;
using RecipeApp.API.DTOs;
using RecipeApp.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Google.Apis.Auth;
using System.Net.Http.Headers;

namespace RecipeApp.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            // Check if user already exists
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
            {
                throw new ApplicationException("User with this email already exists");
            }

            // Create new user
            var user = new User
            {
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                FullName = registerDto.FullName,
                Role = "User",
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generate JWT token
            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Token = token,
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role
                }
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            // Find user by email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
            if (user == null)
            {
                throw new ApplicationException("Invalid email or password");
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                throw new ApplicationException("Invalid email or password");
            }

            // Generate JWT token
            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Token = token,
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role
                }
            };
        }

        public async Task DeleteClerkUserAsync(string clerkUserId)
        {
            var clerkSecretKey = "sk_test_QqvhQWSBBxurOezXAPp9rnaDccTJL5834lPRXDe7a1"; // Заміни на твій Clerk Secret Key!
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", clerkSecretKey);

                var url = $"https://api.clerk.com/v1/users/{clerkUserId}";
                var response = await client.DeleteAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    // опціонально: логувати помилку
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Clerk deletion error: {error}");
                }
            }
        }

        public async Task<AuthResponseDto> GoogleLoginWithClerkAsync(string clerkToken)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadJwtToken(clerkToken);

                // Отримання значень із claims
                var email = jsonToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
                var fullName = jsonToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
                var clerkId = jsonToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value; // sub вже присутній автоматично

                // Перевірка наявності обов'язкових полів
                if (string.IsNullOrEmpty(email))
                {
                    throw new UnauthorizedAccessException("Email not found in token claims");
                }

                if (string.IsNullOrEmpty(clerkId))
                {
                    throw new UnauthorizedAccessException("User ID not found in token claims");
                }

                // Пошук або створення користувача
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    user = new User
                    {
                        Email = email,
                        FullName = fullName ?? "Unknown",
                        GoogleId = clerkId,
                        Role = "User",
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.Users.Add(user);
                }
                else
                {
                    // Оновлення даних існуючого користувача
                    if (user.GoogleId != clerkId)
                    {
                        user.GoogleId = clerkId;
                    }

                    if (!string.IsNullOrEmpty(fullName) && user.FullName == "Unknown")
                    {
                        user.FullName = fullName;
                    }
                }

                await _context.SaveChangesAsync();
                if (!string.IsNullOrEmpty(clerkId))
                {
                    await DeleteClerkUserAsync(clerkId);
                }
                var token = GenerateJwtToken(user);
                return new AuthResponseDto
                {
                    Token = token,
                    User = new UserDto
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FullName = user.FullName,
                        Role = user.Role
                    }
                };
                
            }
            catch (Exception ex)
            {
                // Логування помилки
                throw new UnauthorizedAccessException("Authentication failed");
            }
        }



        public string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiryInMinutes"]));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
