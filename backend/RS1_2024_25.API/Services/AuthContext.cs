using RS1_2024_25.API.Services.Interfaces;
using System.Security.Claims;

namespace RS1_2024_25.API.Services
{
    public class AuthContext : IAuthContext
    {

        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User;


        public int AppUserId => int.Parse(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException());

        public int TenantId => int.Parse(User?.FindFirst("tenant_id")?.Value ?? throw new UnauthorizedAccessException());

        public string Email => User?.FindFirst(ClaimTypes.Email)?.Value ?? throw new UnauthorizedAccessException();

        public string FullName => User?.FindFirst(ClaimTypes.Name)?.Value ?? throw new UnauthorizedAccessException();

        public string Role => User?.FindFirst(ClaimTypes.Role)?.Value ?? throw new UnauthorizedAccessException();
    }
}
