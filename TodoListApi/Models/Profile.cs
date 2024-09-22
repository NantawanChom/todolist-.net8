using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoListApi.Models
{
    public class Profile
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("User")]
        public required string UserId { get; set; }
        public required IdentityUser User { get; set; }

        [MaxLength(100)]
        public string? FirstName { get; set; }
        [MaxLength(100)]
        public string? LastName { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [MaxLength(255)]
        public string? Address { get; set; }
    }
}