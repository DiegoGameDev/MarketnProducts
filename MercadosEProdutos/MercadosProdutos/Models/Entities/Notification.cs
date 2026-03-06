namespace DBModel;

public class Notification
{
    public Guid ID { get; set; }
    public string UserID { get; set; }
    public string TargetID { get; set; } // ID do mercado ou usuario alvo à notificação
    public string? Title { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
}