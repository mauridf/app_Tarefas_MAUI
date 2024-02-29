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
    public class Anexo
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Arquivo { get; set; }
        public int TarefaId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
