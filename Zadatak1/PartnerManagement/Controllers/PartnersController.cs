using Microsoft.AspNetCore.Mvc;
using PartnerManagement.Data;
using PartnerManagement.Models;

namespace PartnerManagement.Controllers;

public sealed class PartnersController : Controller
{
    private readonly IPartnerRepository _partnerRepository;

    public PartnersController(IPartnerRepository partnerRepository)
    {
        _partnerRepository = partnerRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? highlightId = null)
    {
        ViewBag.HighlightId = highlightId;
        var partners = await _partnerRepository.GetAllAsync();
        return View(partners);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new PartnerCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PartnerCreateViewModel model)
    {
        if (await _partnerRepository.ExternalCodeExistsAsync(model.ExternalCode))
        {
            ModelState.AddModelError(nameof(model.ExternalCode), "External code already exists.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        int partnerId;
        try
        {
            partnerId = await _partnerRepository.CreateAsync(model);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(nameof(model.ExternalCode), ex.Message);
            return View(model);
        }

        TempData["SuccessMessage"] = "Partner was created successfully.";
        return RedirectToAction(nameof(Index), new { highlightId = partnerId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePolicy(PolicyCreateViewModel model)
    {
        if (!await _partnerRepository.PartnerExistsAsync(model.PartnerId))
        {
            ModelState.AddModelError(nameof(model.PartnerId), "Selected partner does not exist.");
        }

        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(item => item.Value?.Errors.Count > 0)
                .ToDictionary(
                    item => item.Key,
                    item => item.Value!.Errors.Select(error => error.ErrorMessage).ToArray());

            return BadRequest(new { success = false, errors });
        }

        var summary = await _partnerRepository.AddPolicyAsync(model);
        var isImportant = summary.PolicyCount > 5 || summary.TotalPolicyAmount > 5000m;

        return Json(new
        {
            success = true,
            partnerId = model.PartnerId,
            summary.FullName,
            summary.PolicyCount,
            totalPolicyAmount = summary.TotalPolicyAmount,
            policyTotalFormatted = summary.TotalPolicyAmount.ToString("N2"),
            isImportant
        });
    }
}