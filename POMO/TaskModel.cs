using SQLite;

namespace POMO
{
    public class TaskModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public int NumSessions { get; set; }
        public bool IsCompleted { get; set; }
    }
}
