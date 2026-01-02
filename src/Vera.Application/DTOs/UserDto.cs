namespace Vera.Application.DTOs;

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public UserProfileDto? Profile { get; set; }
}

public class UserProfileDto
{
    public string Id { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string Occupation { get; set; } = string.Empty;
    public string Education { get; set; } = string.Empty;
    public List<string> Interests { get; set; } = new();
    public List<string> Hobbies { get; set; } = new();
    public bool IsComplete { get; set; }
    public RelationshipPreferencesDto? Preferences { get; set; }
}

public class RelationshipPreferencesDto
{
    public string Id { get; set; } = string.Empty;
    public string PreferredGender { get; set; } = string.Empty;
    public int MinAge { get; set; }
    public int MaxAge { get; set; }
    public int MaxDistance { get; set; }
    public string RelationshipType { get; set; } = string.Empty;
    public List<string> DealBreakers { get; set; } = new();
    public List<string> MustHaves { get; set; } = new();
    public List<string> NiceToHaves { get; set; } = new();
}
