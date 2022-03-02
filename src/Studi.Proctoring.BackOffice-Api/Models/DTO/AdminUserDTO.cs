using System;
using System.Collections.Generic;
using Studi.Proctoring.Database.Context;

namespace Studi.Proctoring.BackOffice_Api.Models.DTO
{
    public class AdminUserDTO
    {
        public enum UserTypeEnum
        {
            Admin = 1,
            User = 2,
            Automation = 3
        };

        public int Id { get; set; }
        public string Login { get; set; }               // email, 50 characters max
        public string Password { get; set; }            // Password key b64, 255 characters max
        public string Salt { get; set; }                // Salt b64, used for password key calculation, 56 characters max
        public UserTypeEnum UserType { get; set; }
        public bool IsActive { get; set; }
        public DateTime? PasswordExpirationDate { get; set; }
    }

    public class AdminUsersPageDTO
    {
        public int PageIndex { get; set; }
        public int TotalItemsCount { get; set; }
        public IEnumerable<AdminUserDTO> Page { get; set; }
    }
}
