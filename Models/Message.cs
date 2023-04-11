using System.Collections.Generic;

namespace HeyListen.Models
{
  public class Message
  {
    public string Id { get; set; }
    public string Content { get; set; }
    public string ChannelId { get; set; } 
    public Channel Channel { get; set; }
    public string SenderId { get; set; }
    public User Sender { get; set; }
    public DateTime Timestamp { get; set; }
  }
}