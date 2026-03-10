using System;
using System.Collections.Generic;
using System.Text;

namespace LR1.Models
{
    public class TransactionAction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime DateTime { get; set; } = DateTime.Now;
        public Guid UserIdFrom { get; set; }
        public Guid UserIdTo { get; set; }
        public TransactionType Type { get; set; }

        public string Description { get; set; }
        public Guid? SourceAccountId { get; set; }
        public Guid? TargetAccountId { get; set; }
        public decimal Amount { get; set; }
        public bool IsCancelled { get; set; } = false;

    }
}
