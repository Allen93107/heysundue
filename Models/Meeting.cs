namespace Heysundue.Models
{
public class Meeting
{
    public int ID { get; set; }
    public string Title { get; set; } // 会议名称
    public DateTime CreatedDate { get; set; }

    public string Place { get; set; }

    public DateTime MeetingDateTime { get; set; } // 新增會議時間屬性
    public ICollection<Sessionuser> Sessionusers { get; set; }
}
}