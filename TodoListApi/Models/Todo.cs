using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoListApi.Models
{
    public class Todo
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public bool IsComplete { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual IdentityUser User { get; set; }
    }
}