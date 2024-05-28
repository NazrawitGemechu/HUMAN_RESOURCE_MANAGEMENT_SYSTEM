using HRMS.API.Data;
using HRMS.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class EvaluationFactorController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public EvaluationFactorController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EvaluationFactor>>> GetEvaluationFactors()
    {
        return await _context.EvaluationFactors.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EvaluationFactor>> GetEvaluationFactor(int id)
    {
        var evaluationFactor = await _context.EvaluationFactors.FindAsync(id);

        if (evaluationFactor == null)
        {
            return NotFound();
        }

        return evaluationFactor;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutEvaluationFactor(int id, EvaluationFactor evaluationFactor)
    {
        if (id != evaluationFactor.Id)
        {
            return BadRequest();
        }

        _context.Entry(evaluationFactor).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!EvaluationFactorExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<EvaluationFactor>> PostEvaluationFactor(EvaluationFactor evaluationFactor)
    {
        _context.EvaluationFactors.Add(evaluationFactor);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEvaluationFactor), new { id = evaluationFactor.Id }, evaluationFactor);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEvaluationFactor(int id)
    {
        var evaluationFactor = await _context.EvaluationFactors.FindAsync(id);
        if (evaluationFactor == null)
        {
            return NotFound();
        }

        _context.EvaluationFactors.Remove(evaluationFactor);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool EvaluationFactorExists(int id)
    {
        return _context.EvaluationFactors.Any(e => e.Id == id);
    }
}
