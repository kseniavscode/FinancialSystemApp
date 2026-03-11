using LR1.Interfaces;
using LR1.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LR1.Services
{
    internal class ManagerService : IManagerService
    {
        public void ApproveUser(User user)
        {
            user.Status = ApprovalStatus.Approved;
            App.Database.Save();
        }
        public void RejectUser(User user)
        {
            user.Status = ApprovalStatus.Rejected;
            App.Database.Save();
        }

        public void ApproveSalaryInline(SalaryRequest salaryRequest)
        {
            salaryRequest.Status = ApprovalStatus.Approved;

            var enterprise = App.Database.Enterprises.FirstOrDefault(x => x.Id == salaryRequest.EnterpriseId);
            if (enterprise != null && !enterprise.EmployeeIds.Contains(salaryRequest.UserId))
            {
                enterprise.EmployeeIds.Add(salaryRequest.UserId);
            }
            App.Database.Save();
        }
    }
}
