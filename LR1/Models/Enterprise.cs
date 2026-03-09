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
        public decimal MinSalary { get; set; }
        public decimal MaxSalary { get; set; }
        public Enterprise() { }
        public Enterprise(string name, decimal min = 800, decimal max = 2500)
        {
            Name = name;
            Id = Guid.NewGuid();
            MinSalary = min;
            MaxSalary = max;
        }
    }
}
