using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;
using System.Text.Json.Serialization;

namespace LR1.Models
{
    public class User
    {
        public Guid IdUser { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;
        public UserRole Role { get; set; }

        public User() { }
        public User(string name, string password, UserRole role)
        {
            IdUser = Guid.NewGuid();
            Name = name;
            Password = password;
            Role = role;
            if (role == UserRole.Admin || role == UserRole.Manager)
            {
                Status = ApprovalStatus.Approved;
            }
        }
        public void Approve()
        {
            Status = ApprovalStatus.Approved;
        }
    }
}
