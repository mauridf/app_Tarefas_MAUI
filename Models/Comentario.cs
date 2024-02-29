using app_Tarefas.Enums;
using app_Tarefas.Servicos;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app_Tarefas.Models
{
    public class Comentario
    {
        public Comentario()
        {
            this.Data = DateTime.Now;
        }
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Texto { get; set; }
        public string Descricao { get; set; }
        public DateTime Data { get; set; }
        public int TarefaId { get; set; }
        public int UsuarioId { get; set; }
        [Ignore]
        public Usuario? Usuario
        {
            get
            {
                if (this.UsuarioId == 0) return null;
                return UsuarioServico.Instancia().Todos().Find(u => u.Id == this.UsuarioId);
            }
        }
        [Ignore]
        public string NomeUsuario
        {
            get
            {
                if (this.Usuario == null) return "Sem Usuário";
                return Usuario?.Nome;
            }
        }
    }
}
