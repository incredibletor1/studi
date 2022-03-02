namespace Studi.Api.Proctoring.Models.VM
{
    public class UserVM
    {
        public int Id { get; set; } // Id (Primary key)
        public string Civility { get; set; } // Civility (length: 20)
        public int LmsUserId { get; set; } // LmsUserId
        public string FirstName { get; set; } // FirstName (length: 100)
        public string LastName { get; set; } // LastName (length: 100)
        public string Email { get; set; } // Email (length: 100)
        public string TimeZoneId { get; set; } // TimeZoneId (length: 32)
    }
}
