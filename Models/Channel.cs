using System.Collections.Generic;

namespace HeyListen.Models
{
  public class Channel
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string AuthorId { get; set; }
    public bool IsPrivate { get; set; }
    public User Author { get; set; }
    public List<Message> Messages { get; set; } = new List<Message>();
    public List<Song> Songs { get; set; } = new List<Song>();
    public List<UserChannel> UserChannels { get; set; } = new List<UserChannel>();
  }
}