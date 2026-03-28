using System.ComponentModel.DataAnnotations;

namespace DBModel;

public class Alert
{
    public int ID { get; set; }
    [Required]
    public string UserID { get; set; }
    public string TargetID { get; set; } // ID do mercado ou usuario alvo à notificação
    public string? Title { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
}