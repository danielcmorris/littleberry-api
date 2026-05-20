using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LittleberryApi.Models;

[Table("subject_lookup")]
public class Subject
{
    [Key]
    [Column("id")]
    public int SubjectId { get; set; }

    [Column("subject")]
    public string? Name { get; set; }

    [Column("prefix")]
    public string? Prefix { get; set; }

    [Column("last_id")]
    public int? LastId { get; set; }

    [Column("status")]
    public string? Status { get; set; }
}
