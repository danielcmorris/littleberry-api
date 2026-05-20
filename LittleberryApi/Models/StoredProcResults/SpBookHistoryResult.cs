namespace LittleberryApi.Models.StoredProcResults;

public class SpBookHistoryResult
{
    public int? BookId { get; set; }
    public string? CreateBy { get; set; }
    public DateTime? CreateDate { get; set; }
    public string? Detail { get; set; }
    public string? EventType { get; set; }
    public string? Status { get; set; }
}
