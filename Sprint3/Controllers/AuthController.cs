using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sprint3.Data;
using Sprint3.DTOs.Requests;
using Sprint3.Services;

namespace Sprint3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly EmhsDbContext _context;
        private readonly TokenService _tokenService;

        public AuthController(EmhsDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Login == login.Usuario);

            if (usuario == null)
                return Unauthorized(new { Mensagem = "Usuário ou senha inválidos." });

            bool senhaCorreta = login.Senha == usuario.SenhaHash;

            if (!senhaCorreta)
                return Unauthorized(new { Mensagem = "Usuário ou senha inválidos." });

            var tokenString = _tokenService.GerarToken(usuario);

            return Ok(new
            {
                Usuario = usuario.Login,
                Regra = usuario.Regra,
                Token = tokenString,
                ExpiraEm = DateTime.UtcNow.AddHours(2)
            });
        }
    }
}