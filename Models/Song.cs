using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeyListen.Models
{
  public class Song
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; }
    public string UserId { get; set; }
    public int ChannelId { get; set; }
    public User? User { get; set; }
    public Channel? Channel { get; set; }
    
  }
}