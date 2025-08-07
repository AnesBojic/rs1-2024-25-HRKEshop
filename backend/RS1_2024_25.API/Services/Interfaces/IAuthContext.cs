namespace RS1_2024_25.API.Services.Interfaces
{
    public interface IAuthContext
    {
        int AppUserId { get;}
        int TenantId { get; }
        string Email { get; }
        string FullName { get; }
        string Role { get; }
    }
}
