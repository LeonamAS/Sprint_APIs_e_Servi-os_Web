using Microsoft.AspNetCore.Mvc;
using Sprint3.Models;

namespace Sprint3.Controllers;

[ApiController]
[Route("[controller]")]
public class FilmeController : ControllerBase
{
    private static List<Filme> filmes = new List<Filme>();

    [HttpPost]
    public void AdicionaFilme(Filme filme)
    {
        filmes.Add(filme);
        Console.WriteLine(filme.Titulo);
        Console.WriteLine(filme.Genero);
    }
}
