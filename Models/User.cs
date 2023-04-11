using System.Collections.Generic;

namespace HeyListen.Models
{
  public class User
  {
    public string Id { get; set; }
    public string CognitoUsername { get; set; }
    public List<UserChannel> UserChannels { get; set; } = new List<UserChannel>();
    public List<Message> ReceivedMessages { get; set; } = new List<Message>();
    public List<Message> Messages { get; set; } = new List<Message>();
  }
}
