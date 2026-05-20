using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LittleberryApi.Models;

[Table("ReservationSub")]
public class ReservationSub
{
    [Key]
    public int ReservationSubId { get; set; }
    public int? ReservationId { get; set; }
    public int? BookId { get; set; }
    public DateTime? RequestDate { get; set; }
    public DateTime? PackDate { get; set; }
    public DateTime? ShipDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? ReceivedDate { get; set; }
    public DateTime? ReshelveDate { get; set; }
    public string? TrackingLink { get; set; }
    public string? RequestByEmail { get; set; }
    public int? ShipmentId { get; set; }
}
