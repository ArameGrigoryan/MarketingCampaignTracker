using Campaign.Domain.Entities;
using Campaign.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Campaign.Api.Controllers;

[ApiController]
[Route("conversion-types")]
[Authorize(Roles = "Marketing,Admin")]
public class ConversionTypesController : ControllerBase
{
    private readonly CampaignDb _db;
    public ConversionTypesController(CampaignDb db) => _db = db;

    [HttpGet]
    public async Task<IReadOnlyList<ConversionType>> List(CancellationToken ct)
        => await _db.ConversionTypes.OrderBy(x => x.Name).ToListAsync(ct);

    [HttpPost]
    public async Task<ActionResult<ConversionType>> Create([FromBody] ConversionType req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.Name)) return BadRequest("name required");
        _db.ConversionTypes.Add(req);
        await _db.SaveChangesAsync(ct);
        return Ok(req);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ConversionType req, CancellationToken ct)
    {
        var e = await _db.ConversionTypes.FindAsync(new object[] { id }, ct);
        if (e is null) return NotFound();
        e.Name = req.Name;
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var e = await _db.ConversionTypes.FindAsync(new object[] { id }, ct);
        if (e is null) return NotFound();
        _db.ConversionTypes.Remove(e);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}