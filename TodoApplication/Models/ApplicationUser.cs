using Microsoft.AspNetCore.Identity;

namespace TodoApplication.Models
{
    public class ApplicationUser:IdentityUser
    {
        // Add custom properties for your ApplicationUser here
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        // Navigation property for ToDoItems
        public ICollection<ToDoItem>? ToDoItems { get; set; }

    }
}
