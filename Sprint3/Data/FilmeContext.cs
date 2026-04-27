using Microsoft.EntityFrameworkCore;
using Sprint3.Models;

namespace Sprint3.Data;

public class FilmeContext : DbContext
{
    public FilmeContext(DbContextOptions<FilmeContext> options) : base(options)
    {
        
    }

    public DbSet<Filme> Filmes { get; set; }
}
