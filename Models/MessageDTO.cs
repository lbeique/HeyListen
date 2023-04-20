public class MessageDTO
{
    public int Id { get; set; }
    public string Text { get; set; }
    public DateTime Timestamp { get; set; }
    public string SenderId { get; set; }
    public UserDTO Sender { get; set; }
}