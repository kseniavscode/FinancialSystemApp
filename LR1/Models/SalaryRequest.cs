using System;
using System.Collections.Generic;
using System.Text;

namespace LR1.Models
{
    public class SalaryRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public Guid EnterpriseId { get; set; }
        public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;

        public SalaryRequest() { }
        public SalaryRequest(Guid userId, Guid enterpriseId)
        {
            UserId = userId;
            EnterpriseId = enterpriseId;
        }
    }
}
