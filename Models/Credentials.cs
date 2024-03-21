namespace Credentials.Models;


public class CredentialsObj {
    public string? Token { get; set; }
    public string? AesKey { get; set; }
    public string? AesIv { get; set; }
}

public class CredentialsRes {
    public string? AesKey { get; set; }
    public string? AesIv { get; set; }
}