namespace DMD.Persistence.Services
{
    public static class TaskServiceHelper
    {
        public static bool IsValidStatus(string status)
        {
            var validStatuses = new[] { "Назначена", "Выполняется", "Приостановлена", "Завершена" };
            return validStatuses.Contains(status);
        }
    }
}
