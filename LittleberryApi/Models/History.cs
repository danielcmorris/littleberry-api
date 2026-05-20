using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LittleberryApi.Models;

[Table("History")]
public class History
{
    [Key]
    public int HistoryId { get; set; }
    public DateTime? CreateDate { get; set; }
    public string? CreateBy { get; set; }
    public string? Status { get; set; }
    public string? Detail { get; set; }
    public string? Notes { get; set; }
    public string? EventType { get; set; }
    public int? BookId { get; set; }
}
