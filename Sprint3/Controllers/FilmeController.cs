using Microsoft.AspNetCore.Mvc;
using Sprint3.Data;
using Sprint3.Models;

namespace Sprint3.Controllers;

[ApiController]
[Route("[controller]")]
public class FilmeController : ControllerBase
{
    private FilmeContext _context;

    public FilmeController(FilmeContext context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult AdicionaFilme(Filme filme)
    {
        _context.Filmes.Add(filme);
        _context.SaveChanges();
        return CreatedAtAction(nameof(RecuperaFilmePorId), 
            new {id = filme.Id},
            filme);
    }

    [HttpGet]
    public IEnumerable<Filme> RecuperarFilmes([FromQuery] int skip = 0, [FromQuery] int take = 50)
    {
        return _context.Filmes.Skip(skip).Take(take);
    }

    [HttpGet("{id}")]
    public IActionResult RecuperaFilmePorId(int id)
    {
        var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
        if (filme == null) return NotFound();
        return Ok(filme);
    }
}
