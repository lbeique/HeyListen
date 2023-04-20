public class SongDTO
{
  public int Id { get; set; }
  public string Name { get; set; }
  public string UserId { get; set; }
  public int ChannelId { get; set; }
  public UserDTO User { get; set; }
  public ChannelDTO Channel { get; set; }
}
