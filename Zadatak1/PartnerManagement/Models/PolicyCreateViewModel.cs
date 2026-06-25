using System.ComponentModel.DataAnnotations;

namespace PartnerManagement.Models;

public sealed class PolicyCreateViewModel
{
    [Required]
    [Range(1, int.MaxValue)]
    public int PartnerId { get; set; }

    [Required]
    [StringLength(15, MinimumLength = 10)]
    [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "Policy number must be alphanumeric.")]
    public string PolicyNumber { get; set; } = string.Empty;

    [Required]
    [Range(0.01, 999999999.99, ErrorMessage = "Policy amount must be greater than zero.")]
    public decimal PolicyAmount { get; set; }
}