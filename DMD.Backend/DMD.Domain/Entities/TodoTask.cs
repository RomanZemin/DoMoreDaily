namespace DMD.Domain.Entities
{
    public class TodoTask
    {
        public int Id { get; set; }
        public int? ParentTaskID { get; set; }
        public string? TaskName { get; set; }
        public string? Description { get; set; }
        public string? Assignees { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        public string? Status { get; set; }
        public decimal PlannedEffort { get; set; }
        public decimal ActualEffort { get; set; }
        public DateTime? CompletionDate { get; set; }
        public List<TodoTask> SubTasks { get; set; } = new List<TodoTask>();
    }
}