using System.Collections.Generic;

namespace Heysundue.Models
{
    public class SessionuserViewModel
    {
        public List<Sessionuser> Sessionusers { get; set; } = new List<Sessionuser>();
        public Sessionuser Sessionuser { get; set; } = new Sessionuser();

        public int CurrentPage { get; set; } // 當前頁碼
        public int TotalPages { get; set; } // 總頁數
    }
}
