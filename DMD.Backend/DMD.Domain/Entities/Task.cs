

namespace DMD.Domain.Task{
    public class Task
    {
        public int TaskID { get; set; }
        public int? ParentTaskID { get; set; }
        public string TaskName { get; set; }
        public string Description { get; set; }
        public string Assignees { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        public string Status { get; set; }
        public decimal PlannedEffort { get; set; }
        public decimal ActualEffort { get; set; }
        public DateTime? CompletionDate { get; set; }

        public List<Task> SubTasks { get; set; } = new List<Task>();
    }
}