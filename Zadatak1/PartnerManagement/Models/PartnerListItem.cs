namespace PartnerManagement.Models;

public sealed class PartnerListItem
{
    public int PartnerId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Address { get; set; } = string.Empty;
    public string PartnerNumber { get; set; } = string.Empty;
    public string? CroatianPIN { get; set; }
    public int PartnerTypeId { get; set; }
    public string PartnerTypeName => PartnerTypeId == (int)PartnerType.Legal ? "Legal" : "Personal";
    public DateTime CreatedAtUtc { get; set; }
    public string CreatedByUser { get; set; } = string.Empty;
    public bool IsForeign { get; set; }
    public string ExternalCode { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public int PolicyCount { get; set; }
    public decimal TotalPolicyAmount { get; set; }
    public bool IsImportant => PolicyCount > 5 || TotalPolicyAmount > 5000m;
}