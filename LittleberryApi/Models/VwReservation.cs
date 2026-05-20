using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LittleberryApi.Models;

[Table("vwReservation")]
public class VwReservation
{
    [Key]
    public int ReservationSubId { get; set; }
    public DateTime? RequestDate { get; set; }
    public DateTime? PackDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? ShipDate { get; set; }
    public DateTime? ReceivedDate { get; set; }
    public DateTime? ReshelveDate { get; set; }
    public string? TrackingLink { get; set; }
    public string? RequestByEmail { get; set; }
    public string? ReservationSubStatus { get; set; }
    public int AccountId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Zip { get; set; }
    public string? Country { get; set; }
    public string? AccountStatus { get; set; }
    public int? BookNumber { get; set; }
    public string? Prefix { get; set; }
    public int? SubjectId { get; set; }
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? BookStatus { get; set; }
}
