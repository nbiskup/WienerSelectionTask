using System.ComponentModel.DataAnnotations;
using PartnerManagement.Validation;

namespace PartnerManagement.Models;

public sealed class PartnerCreateViewModel : IValidatableObject
{
    [Required]
    [StringLength(255, MinimumLength = 2)]
    [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "First name must be alphanumeric.")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(255, MinimumLength = 2)]
    [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "Last name must be alphanumeric.")]
    public string LastName { get; set; } = string.Empty;

    [StringLength(500)]
    [RegularExpression(@"^[A-Za-z0-9 ]*$", ErrorMessage = "Address must be alphanumeric.")]
    public string? Address { get; set; }

    [Required]
    [RegularExpression(@"^\d{20}$", ErrorMessage = "Partner number must contain exactly 20 digits.")]
    public string PartnerNumber { get; set; } = string.Empty;

    [Display(Name = "Croatian PIN / OIB")]
    public string? CroatianPIN { get; set; }

    [Required]
    [Range(1, 2, ErrorMessage = "Partner type must be Personal or Legal.")]
    public int PartnerTypeId { get; set; } = 1;

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string CreatedByUser { get; set; } = string.Empty;

    [Required]
    public bool IsForeign { get; set; }

    [Required]
    [StringLength(20, MinimumLength = 10)]
    [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "External code must be alphanumeric.")]
    public string ExternalCode { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^[MFN]$", ErrorMessage = "Gender must be M, F or N.")]
    public string Gender { get; set; } = "N";

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrWhiteSpace(CroatianPIN) && !OibValidator.IsValid(CroatianPIN))
        {
            yield return new ValidationResult("Croatian PIN / OIB is not valid.", new[] { nameof(CroatianPIN) });
        }
    }
}