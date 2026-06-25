using PartnerManagement.Models;

namespace PartnerManagement.Data;

public interface IPartnerRepository
{
    Task<IReadOnlyList<PartnerListItem>> GetAllAsync();
    Task<IReadOnlyList<PartnerOption>> GetPartnerOptionsAsync();
    Task<int> CreateAsync(PartnerCreateViewModel partner);
    Task<bool> ExternalCodeExistsAsync(string externalCode);
    Task<bool> PartnerExistsAsync(int partnerId);
    Task<(int PolicyCount, decimal TotalPolicyAmount, string FullName)> AddPolicyAsync(PolicyCreateViewModel policy);
}