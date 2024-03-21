namespace User.Models;

public class UserDto {
    public string? Id { get; set; }
    public string? Password { get; set; }
}

public class Settings {
    public DateTime? Deadline { get; set; }
}