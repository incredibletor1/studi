using System;
using System.Collections.Generic;

namespace Studi.Proctoring.BackOffice_Api.Models.VM
{
    public class AdminUserVM
    {
        public int Id { get; set; }
        public string Login { get; set; }       // email, 50 characters max
        public string UserType { get; set; }    
        public bool IsActive { get; set; }
        public DateTime? PasswordExpirationDate { get; set; }
    }

    public class AdminUsersPageVM
    {
        public int PageIndex { get; set; }
        public int TotalItemsCount { get; set; }
        public List<AdminUserVM> Page { get; set; }
    }
}
