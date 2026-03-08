using System;
using System.Collections.Generic;
using System.Text;

namespace LR1.Models
{
    public class Enterprise
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Guid> EmployeeIds { get; set; } = new List<Guid>();

        public Enterprise() { }
        public Enterprise(string name)
        {
            Name = name;
            Id = Guid.NewGuid();
        }
    }
}
