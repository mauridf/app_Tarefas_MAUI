using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app_Tarefas.Servicos
{
    public class DataBaseServico<T> where T : new()
    {
        private SQLiteAsyncConnection _database;

        public DataBaseServico(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<T>().Wait();
        }

        public Task<int> IncluirAsync(T item)
        {
            return _database.InsertAsync(item);
        }

        public Task<int> AlterarAsync(T item)
        {
            return _database.UpdateAsync(item);
        }

        public Task<int> DeleteAsync(T item)
        {
            return _database.DeleteAsync(item);
        }

        public Task<List<T>> TodosAsync() 
        { 
            return _database.Table<T>().ToListAsync();
        }

        public Task<int> QuantidadeAsync()
        {
            return _database.Table<T>().CountAsync();
        }
    }
}
