using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HeyListen.Models
{
  public class User
  {
    [Key]
    public string sub { get; set; }
    public string username { get; set; }
    public List<UserChannel> UserChannels { get; set; } = new List<UserChannel>();
    public List<Message> ReceivedMessages { get; set; } = new List<Message>();
    public List<Message> Messages { get; set; } = new List<Message>();
  }
}
