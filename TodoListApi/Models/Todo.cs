using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoListApi.Models
{
    [Index(nameof(Title))]
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