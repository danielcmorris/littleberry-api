namespace LittleberryApi.Models.StoredProcResults;

public class SpBookResult
{
    public int BookId { get; set; }
    public string? Number { get; set; }  // CallNumber from sp_Book
    public string? Author { get; set; }
    public string? Title { get; set; }
    public string? Notes { get; set; }
    public int? SubjectId { get; set; }
    public string? Status { get; set; }
    public DateTime? DateAdded { get; set; }  // CreateDate from sp_Book
    public int? BookNumber { get; set; }
    public string? Prefix { get; set; }
    public string? Subject { get; set; }
    public string? Url { get; set; }
}
