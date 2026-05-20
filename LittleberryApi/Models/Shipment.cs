using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LittleberryApi.Models;

[Table("Shipment")]
public class Shipment
{
    [Key]
    public int ShipmentId { get; set; }
    public DateTime? ShipmentDate { get; set; }
    public int? AccountId { get; set; }
    public string? AddressTitle { get; set; }
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Zip { get; set; }
    public string? Country { get; set; }
    public string? FullAddress { get; set; }
    public string? Status { get; set; }
    public DateTime? CreateDate { get; set; }
    public string? CreateBy { get; set; }
    public string? Notes { get; set; }
    public string? TrackingNumber { get; set; }
    public string? ShippingCompanyCode { get; set; }
    public string? TrackingUrl { get; set; }
}
