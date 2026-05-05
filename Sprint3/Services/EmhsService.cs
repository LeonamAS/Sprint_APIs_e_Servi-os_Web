using Microsoft.EntityFrameworkCore;
using Sprint3.Data;
using Sprint3.DTOs.Response;

namespace Sprint3.Services;

public class EmhsService
{
    private readonly EmhsDbContext _context;

    public EmhsService(EmhsDbContext context)
    {
        _context = context;
    }
}