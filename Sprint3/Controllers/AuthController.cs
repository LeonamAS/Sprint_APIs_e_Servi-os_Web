using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sprint3.Data;
using Sprint3.DTOs.Requests;
using Sprint3.Models;
using Sprint3.Services;

namespace Sprint3.Controllers;

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

    /// <summary>
    /// Registra um novo usuário no sistema.
    /// </summary>
    /// <response code="404">CPF não encontrado no sistema / usuário já cadastrado</response>
    [HttpPost("registrar")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Registrar([FromBody] CreateUsuarioDTO dto)
    {
        var usuarioJaExiste = await _context.Usuarios.AnyAsync(u => u.Cpf == dto.Cpf);
        if (usuarioJaExiste)
            return BadRequest(new { Mensagem = "Este CPF já possui uma conta de acesso ao sistema." });

        string tipoDefinido = "";

        var isAluno = await _context.Alunos.AnyAsync(a => a.Cpf == dto.Cpf);
        if (isAluno)
        {
            tipoDefinido = "aluno";
        }
        else
        {
            var isProfessor = await _context.Professores.AnyAsync(p => p.Cpf == dto.Cpf);
            if (isProfessor)
            {
                tipoDefinido = "professor";
            }
            else
            {
                return BadRequest(new { Mensagem = "CPF não encontrado na base da instituição. Procure a secretaria." });
            }
        }

        var novoUsuario = new Usuario
        {
            Cpf = dto.Cpf,
            Senha = dto.Senha,
            TipoUsuario = tipoDefinido
        };

        _context.Usuarios.Add(novoUsuario);
        await _context.SaveChangesAsync();

        return Ok(new { Mensagem = "Conta criada com sucesso! Você já pode fazer login." });
    }

    /// <summary>
    /// Realiza o login do usuário no sitema.
    /// </summary>
    /// <response code="200">Usuário autenticado.</response>
    /// <response code="401">Usuário não cadastrado.</response>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDTO dto)
    {
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Cpf == dto.Cpf && u.Senha == dto.Senha);

        if (usuario == null)
            return Unauthorized(new { Mensagem = "CPF ou senha inválidos." });

        var token = _tokenService.GerarToken(usuario);

        return Ok(new
        {
            usuario = usuario.Cpf,
            tipoUsuario = usuario.TipoUsuario,
            token = token
        });
    }

    /// <summary>
    /// Realiza o cadastro de um administrador.
    /// </summary>
    /// <remarks>
    /// Acesso permitido apenas para: **Administradores**.
    /// </remarks>
    /// <response code="200">Administrador cadastrado com sucesso.</response>
    [HttpPost("registrar-admin")]
    [Authorize(Roles = "administrador")]
    public async Task<IActionResult> RegistrarAdmin([FromBody] CreateAdminDTO dto)
    {
        var usuarioJaExiste = await _context.Usuarios.AnyAsync(u => u.Cpf == dto.Cpf);
        if (usuarioJaExiste)
            return BadRequest(new { Mensagem = "Este usuário/CPF já possui cadastro no sistema." });

        var novoAdmin = new Usuario
        {
            Cpf = dto.Cpf,
            Senha = dto.Senha,
            TipoUsuario = "administrador"
        };

        _context.Usuarios.Add(novoAdmin);
        await _context.SaveChangesAsync();

        return Ok(new { Mensagem = "Novo administrador cadastrado com sucesso!" });
    }
}