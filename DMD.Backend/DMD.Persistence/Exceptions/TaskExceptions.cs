namespace DMD.Persistence.Exceptions
{
    public class TaskNotFoundException : Exception
    {
        public TaskNotFoundException(string message) : base(message) { }
    }

    public class InvalidTaskStatusException : Exception
    {
        public InvalidTaskStatusException(string message) : base(message) { }
    }
}
