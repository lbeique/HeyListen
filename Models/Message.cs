using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeyListen.Models
{
  public class Message
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Text { get; set; }
    [Required]
    public int ChannelId { get; set; } 
    public Channel? Channel { get; set; }
    [Required]
    public string? SenderId { get; set; }
    public User? Sender { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
  }
}