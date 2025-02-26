using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Models
{
    public abstract class UserModel : Entity
    {
        public string Name { get; set; }
        public string PasswordH { get; set; }
        public string Email { get; set; }
        public DateTime Birthday { get; set; }
        public Gender Gender { get; set; }
        public string Phone { get; set; }
        public DateTime CreateTime { get; set; }
    }
}