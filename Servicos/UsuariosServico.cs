using SQLite;
using Tarefas.Models;

namespace Tarefas.Servicos;

public class UsuariosServico
{
    private static UsuariosServico _usuariosServico = new UsuariosServico();
    private List<Usuario> _usuarios = new List<Usuario>();

    private UsuariosServico()
    {
        _usuarios.Add(new Usuario { Id = 1, Nome = "Maurício Dias de Carvalho" });
        _usuarios.Add(new Usuario { Id = 2, Nome = "Thais Pereira da Silva Oliveira" });
    }

    public static UsuariosServico Instancia()
    {
        return _usuariosServico;
    }

    public List<Usuario> Todos()
    {
        return _usuarios;
    }
}