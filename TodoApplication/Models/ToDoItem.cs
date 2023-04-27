namespace TodoApplication.Models
{

    public enum Priority
    {
        Low,
        Medium,
        High
    }

    public class ToDoItem
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public bool IsCompleted { get; set; } = false;
        public DateTime? DueDate { get; set; }
        public Priority Priority { get; set; } = Priority.Medium;

        public string? ApplicationUserId { get; set; } // Foreign Key for ApplicationUser
        public ApplicationUser? ApplicationUser { get; set; } // Navigation property for ApplicationUser
    }
}
