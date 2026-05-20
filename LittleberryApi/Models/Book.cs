using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LittleberryApi.Models;

[Table("Book")]
public class Book
{
    [Key]
    public int BookId { get; set; }
    public string? CallNumber { get; set; }
    public string? Author { get; set; }
    public string? Title { get; set; }
    public int? SubjectId { get; set; }
    public string? Prefix { get; set; }
    public int? BookNumber { get; set; }
    public string? Url { get; set; }
    public string? Notes { get; set; }
    public string? Status { get; set; }
    public DateTime? CreateDate { get; set; }
    public string? CreateBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }
}
