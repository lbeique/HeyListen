public class ChannelDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsPrivate { get; set; }
    public ICollection<UserDTO> Users { get; set; }
    public ICollection<MessageDTO> Messages { get; set; }
    public ICollection<SongDTO> Songs { get; set; }
}