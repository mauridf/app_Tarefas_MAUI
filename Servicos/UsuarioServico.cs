using app_Tarefas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app_Tarefas.Servicos
{
    public class UsuarioServico
    {
        private static UsuarioServico _usuarioServico = new UsuarioServico();

        private List<Usuario> _usuarios = new List<Usuario>();

        public UsuarioServico() 
        {
            _usuarios.Add(new Usuario { Id = 1, Nome = "Maurício Dias de Carvalho" });
            _usuarios.Add(new Usuario { Id = 2, Nome = "Thais Pereira da Silva Oliveira" });
            _usuarios.Add(new Usuario { Id = 1, Nome = "Theo Pereira Carvalho" });
            _usuarios.Add(new Usuario { Id = 1, Nome = "Akiles Pereira Carvalho" });
            _usuarios.Add(new Usuario { Id = 1, Nome = "Thales Pereira Carvalho" });
        }

        public static UsuarioServico Instancia() 
        { 
            return _usuarioServico; 
        }
        public List<Usuario> Todos()
        {
             return _usuarios;
        }
    }
}
