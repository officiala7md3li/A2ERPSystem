using DomainDrivenERP.Domain.Abstractions.Infrastructure;
using DomainDrivenERP.Domain.Entities.Localization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DomainDrivenERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocalizationController : ControllerBase
{
    private readonly ILocalizationService _localizationService;

    public LocalizationController(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
    }

    [HttpGet("languages")]
    public async Task<ActionResult<List<Language>>> GetAvailableLanguages(CancellationToken cancellationToken)
    {
        return await _localizationService.GetAvailableLanguages(cancellationToken);
    }

    [HttpGet("translations/{languageCode}")]
    public async Task<ActionResult<Dictionary<string, string>>> GetTranslations(string languageCode, CancellationToken cancellationToken)
    {
        return await _localizationService.GetTranslations(languageCode, cancellationToken);
    }

    [HttpGet("translate")]
    public async Task<ActionResult<string>> Translate(
        [FromQuery] string key,
        [FromQuery] string languageCode,
        [FromQuery] string defaultValue = "",
        CancellationToken cancellationToken = default)
    {
        return await _localizationService.Translate(key, languageCode, defaultValue, cancellationToken);
    }

    [HttpPost("languages")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateLanguage([FromBody] Language language, CancellationToken cancellationToken)
    {
        await _localizationService.AddLanguage(language, cancellationToken);
        return Ok(language);
    }

    [HttpPost("languages/{languageCode}/translations")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UploadTranslations(
        string languageCode,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        if (!file.FileName.EndsWith(".json"))
            return BadRequest("Only JSON files are supported");

        using var reader = new StreamReader(file.OpenReadStream());
        string jsonContent = await reader.ReadToEndAsync(cancellationToken);

        try
        {
            await _localizationService.ImportTranslationsFromJson(languageCode, jsonContent, User.Identity?.Name ?? "system", cancellationToken);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error importing translations: {ex.Message}");
        }
    }

    [HttpPost("import")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ImportTranslations(
        [FromQuery] string languageCode,
        [FromBody] string jsonContent,
        CancellationToken cancellationToken)
    {
        await _localizationService.ImportTranslationsFromJson(languageCode, jsonContent, User.Identity?.Name ?? "system", cancellationToken);
        return Ok();
    }

    [HttpPost("export/{languageCode}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ExportTranslations(string languageCode, CancellationToken cancellationToken)
    {
        await _localizationService.ExportTranslationsToJson(languageCode, User.Identity?.Name ?? "system", cancellationToken);
        return Ok();
    }
}