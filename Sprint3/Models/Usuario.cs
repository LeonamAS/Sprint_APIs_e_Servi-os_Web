namespace Sprint3.Models;

public class Usuario
{
    public int Id { get; set; }
    public string Login { get; set; }
    public string SenhaHash { get; set; }
    public string Regra { get; set; }
}
