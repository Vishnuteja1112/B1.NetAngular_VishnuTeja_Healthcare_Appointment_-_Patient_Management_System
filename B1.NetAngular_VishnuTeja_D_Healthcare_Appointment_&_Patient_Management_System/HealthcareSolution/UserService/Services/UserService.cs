using UserService.DTOs;
using UserService.Helpers;
using UserService.Models;
using UserService.Repository;

namespace UserService.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly JwtHelper _jwtHelper;

        public UserService(IUserRepository repo, JwtHelper jwtHelper)
        {
            _repo = repo;
            _jwtHelper = jwtHelper;
        }

        public async Task<string?> RegisterAsync(RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username))
                return "Username is required";

            if (string.IsNullOrWhiteSpace(dto.Password))
                return "Password is required";

            if (dto.Role != "Patient")
                return "Only Patient registration allowed";

            var existing = await _repo.GetByUsernameAsync(dto.Username);
            if (existing != null)
                return "Username already exists";

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                Username = dto.Username,
                Password = hashedPassword,
                Role = dto.Role
            };

            await _repo.AddUser(user);

            return "Patient Registered Successfully";
        }

        public async Task<string?> LoginAsync(LoginDto dto)
        {
            var user = await _repo.GetByUsernameAsync(dto.Username);

            if (user == null)
                return null;

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                return null;

            return _jwtHelper.GenerateToken(user);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _repo.GetAllUsersAsync();
        }

        public async Task<string?> CreateUserByAdminAsync(RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username))
                return "Username is required";

            if (string.IsNullOrWhiteSpace(dto.Password))
                return "Password is required";

            if (dto.Role != "Admin" && dto.Role != "Doctor" && dto.Role != "Patient")
                return "Invalid Role";

            var existing = await _repo.GetByUsernameAsync(dto.Username);
            if (existing != null)
                return "Username already exists";

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                Username = dto.Username,
                Password = hashedPassword,
                Role = dto.Role
            };

            await _repo.AddUser(user);

            return $"{dto.Role} created successfully by Admin";
        }
    }
}