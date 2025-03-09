using System;
using System.Collections.Generic;

namespace Heysundue.Models
{
    public class Sessionuser
    {
        public int ID { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string? Barcode { get; set; }

        public string? RegNo { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? ChineseName { get; set; }

        public string? Email { get; set; }

        public string? City { get; set; }

        public string? Country { get; set; }

        public bool RegistrationStatus { get; set; } = false;

        public string? IdentityType1 { get; set; }

        public string? IdentityType2 { get; set; }

        public string? IDNumber { get; set; }

        public string? Remark { get; set; }

        public int? MeetingID { get; set; }  // 外鍵關聯至會議

        public string? Speaker { get; set; } = "普通成員";

        public string? Phone { get; set; }
    }
}
