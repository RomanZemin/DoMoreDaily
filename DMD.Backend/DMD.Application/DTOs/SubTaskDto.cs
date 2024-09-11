namespace DMD.Application.DTOs
{
    public class SubTaskDto
    {
        public int Id { get; set; }
        public string? TaskName { get; set; }
        public string? Description { get; set; }
        public string? Assignees { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string? Status { get; set; }
        public int PlannedEffort { get; set; }
        public int ActualEffort { get; set; }
        public int? ParentTaskID { get; set; }
    }
}
