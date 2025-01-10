using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POMO
{
    public class TaskDatabase
    {
        private readonly SQLiteAsyncConnection _database;

        public TaskDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<TaskModel>().Wait();
        }

        public Task<List<TaskModel>> GetTasksAsync()
        {
            return _database.Table<TaskModel>().ToListAsync();
        }

        public Task<TaskModel> GetTaskAsync(int id)
        {
            return _database.Table<TaskModel>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        public Task<int> SaveTaskAsync(TaskModel task)
        {
            if (task.Id != 0)
            {
                return _database.UpdateAsync(task);
            }
            else
            {
                return _database.InsertAsync(task);
            }
        }

        public Task<int> DeleteTaskAsync(TaskModel task)
        {
            return _database.DeleteAsync(task);
        }
    }
}
