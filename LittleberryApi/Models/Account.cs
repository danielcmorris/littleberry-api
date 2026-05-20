using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LittleberryApi.Models;

[Table("Account")]
public class Account
{
    [Key]
    public int AccountId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? AddressTitle { get; set; }
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Zip { get; set; }
    public string? Country { get; set; }
    public string? FullAddress { get; set; }
    public string? AccountType { get; set; }
    public string? Status { get; set; }
    public DateTime? CreateDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public DateTime? ModifiedBy { get; set; }
    public string? CreateBy { get; set; }
    public string? Password { get; set; }
    public int? OfficeId { get; set; }
    public string? PostalCode { get; set; }

    [NotMapped]
    public string? SessionId { get; set; }
}
