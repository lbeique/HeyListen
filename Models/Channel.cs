using System.Collections.Generic;

namespace HeyListen.Models
{
  public class Channel
  {
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string AuthorId { get; set; }
    public bool IsPrivate { get; set; }
    public User Author { get; set; }
    public List<Message> Messages { get; set; } = new List<Message>();
    public List<UserChannel> UserChannels { get; set; } = new List<UserChannel>();
  }
}