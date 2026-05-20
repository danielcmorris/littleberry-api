using System.ComponentModel.DataAnnotations.Schema;

namespace LittleberryApi.Models;

[Table("Member")]
public class Member
{
    public int MemberID { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? AddressTitle { get; set; }
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Phone { get; set; }
    public string? PostalCode { get; set; }
    public string? Status { get; set; }
    public int CouncilID { get; set; }
    public string? SessionID { get; set; }
}
