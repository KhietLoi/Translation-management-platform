using MySolution.Domain.Enums;

namespace MySolution.Domain.Entities;

/// <summary>
///     Represents a user in the system
/// </summary>
public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = string.Empty;
    public UserStatus Status { get; set; } = UserStatus.NonActive;
    public bool IsEmailVerified { get; set; }
    public int PasswordVersion { get; set; } = 1;
    public string SecurityStamp { get; set; } = Guid.CreateVersion7().ToString();
    public DateTime CreatedAt { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    //Add OrganizationMember
    public ICollection<OrganizationMember> OrganizationMembers { get; set; } = new List<OrganizationMember>();
    public UserProfile Profile { get; set; } = null!;
}