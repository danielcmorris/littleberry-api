using System.ComponentModel.DataAnnotations.Schema;

namespace LittleberryApi.Models;

[Table("Council")]
public class Council
{
    public int CouncilID { get; set; }
    public string? CouncilNumber { get; set; }
    public string? CouncilName { get; set; }
    public string? AddressTitle { get; set; }
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? PrimaryContact { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public int SortOrder { get; set; }
}
