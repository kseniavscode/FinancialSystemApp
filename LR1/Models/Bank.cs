using System;
using System.Collections.Generic;
using System.Text;

namespace LR1.Models
{
    public class Bank
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }

        public Bank() { }
        public Bank(string name)
        {
            Name = name;
            Id = Guid.NewGuid();
        }

    }
}
