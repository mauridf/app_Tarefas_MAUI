using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app_Tarefas.Constantes
{
    public struct Db
    {
        public static string DB_PATH
        {
            get
            {
                return Path.Combine(FileSystem.AppDataDirectory, "tarefas.db3");
            }
        }
    }
}
