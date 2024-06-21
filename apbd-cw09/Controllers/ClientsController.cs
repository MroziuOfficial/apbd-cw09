using Microsoft.AspNetCore.Mvc;
using apbd_cw09.Data;
using apbd_cw09.Models;
using Microsoft.EntityFrameworkCore;

namespace apbd_cw09.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClientsController : ControllerBase
{
    private readonly Apbd09Context _context;

    public ClientsController(Apbd09Context context)
    {
        _context = context;
    }

    [HttpDelete("{idClient}")]
    public async Task<IActionResult> DeleteClient(int idClient)
    {
        var client = await _context.Clients
            .Include(c => c.ClientTrips)
            .FirstOrDefaultAsync(c => c.IdClient == idClient);

        if (client == null)
        {
            return NotFound();
        }

        if (client.ClientTrips.Any())
        {
            return BadRequest("Client has assigned trips and cannot be deleted.");
        }

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}