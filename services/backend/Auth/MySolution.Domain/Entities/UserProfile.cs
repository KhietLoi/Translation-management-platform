namespace MySolution.Domain.Entities;

public class UserProfile
{
    public Guid UserId { get; set; }
    public string? FullName { get; set; }
    public DateOnly? BirthDate { get; set; }
    public string? PhoneNumber { get; set; }
    public string? AvatarBlobName { get; set; }
    public string? Address { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public User User { get; set; } = null!;

    public bool IsCompleted =>
        !string.IsNullOrWhiteSpace(FullName) &&
        !string.IsNullOrWhiteSpace(PhoneNumber) &&
        BirthDate.HasValue;
}