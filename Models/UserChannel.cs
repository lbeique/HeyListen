namespace HeyListen.Models
{
    public class UserChannel
    {
        public string UserId { get; set; }
        public User User { get; set; }
        public string ChannelId { get; set; }
        public Channel Channel { get; set; }
    }
}
