using System.Collections.Generic;

namespace Heysundue.Models
{
    public class Joinlist2ViewModel
    {
        public List<Joinlist> Joinlists { get; set; } = new List<Joinlist>();
        public Joinlist Joinlist { get; set; } = new Joinlist();

        public int CurrentPage { get; set; } // 當前頁碼
        public int TotalPages { get; set; } // 總頁數
    }
}
