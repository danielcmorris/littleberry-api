namespace LittleberryApi.Models.DTOs;

public class LoginCredentials
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}

public class PasswordUpdate
{
    public string? NewPassword { get; set; }
    public int AccountId { get; set; }
}

public class QuickRequest
{
    public int BookId { get; set; }
    public string? RequestByEmail { get; set; }
    public DateTime? RequestDate { get; set; }
    public string? SessionId { get; set; }
}

public class QuickRequestUpdate
{
    public int ReservationSubId { get; set; }
    public string? Type { get; set; }
    public bool? OnOff { get; set; }
    public DateTime? StatusDate { get; set; }
    public string? SessionId { get; set; }
}

public class CustomCommandObject
{
    public string? ProcedureName { get; set; }
    public string? Parameters { get; set; }
}

public class SecurityReturnObject
{
    public string? SessionID { get; set; }
    public string? UserLevel { get; set; }
    public int AccountID { get; set; }
    public bool Authorized { get; set; }
    public string? SystemMessage { get; set; }
}
