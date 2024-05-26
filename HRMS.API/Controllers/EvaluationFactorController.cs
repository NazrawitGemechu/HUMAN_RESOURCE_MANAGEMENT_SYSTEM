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

    // GET: api/EvaluationFactors
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EvaluationFactor>>> GetEvaluationFactors()
    {
        return await _context.EvaluationFactors.ToListAsync();
    }

    // GET: api/EvaluationFactors/5
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

    // PUT: api/EvaluationFactors/5
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

    // POST: api/EvaluationFactors
    [HttpPost]
    public async Task<ActionResult<EvaluationFactor>> PostEvaluationFactor(EvaluationFactor evaluationFactor)
    {
        _context.EvaluationFactors.Add(evaluationFactor);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEvaluationFactor), new { id = evaluationFactor.Id }, evaluationFactor);
    }

    // DELETE: api/EvaluationFactors/5
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
