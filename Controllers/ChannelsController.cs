using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HeyListen.Models;
using HeyListen.Data;
using HeyListen.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace HeyListen.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class ChannelsController : ControllerBase
  {
    private readonly HeyListenDbContext _context;
    private readonly IHubContext<ChatHub> _hub;

    public ChannelsController(HeyListenDbContext context, IHubContext<ChatHub> hub)
    {
      _context = context;
      _hub = hub;
    }

    // GET: api/Channels
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Channel>>> GetChannels()
    {
      return await _context.Channels.ToListAsync();
    }

    // GET: api/Channels/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ChannelDTO>> GetChannel(int id)
    {
      var channel = await _context.Channels
          .Include(c => c.UserChannels).ThenInclude(uc => uc.User)
          .Include(c => c.Messages).ThenInclude(m => m.Sender)
          .Include(c => c.Songs).ThenInclude(s => s.User) // Include Songs and related User
          .SingleOrDefaultAsync(c => c.Id == id);

      if (channel == null)
      {
        return NotFound();
      }

      var channelDTO = new ChannelDTO
      {
        Id = channel.Id,
        Name = channel.Name,
        Description = channel.Description,
        IsPrivate = channel.IsPrivate,
        Users = channel.UserChannels.Select(uc => new UserDTO
        {
          sub = uc.User.sub,
          username = uc.User.username
        }).ToList(),
        Messages = channel.Messages.Select(m => new MessageDTO
        {
          Id = m.Id,
          Text = m.Text,
          Timestamp = m.Timestamp,
          SenderId = m.SenderId,
          Sender = new UserDTO
          {
            sub = m.Sender.sub,
            username = m.Sender.username
          }
        }).ToList(),
        Songs = channel.Songs.Select(s => new SongDTO // Map Songs to SongDTO
        {
          Id = s.Id,
          Name = s.Name,
          UserId = s.UserId,
          ChannelId = s.ChannelId,
          User = new UserDTO
          {
            sub = s.User.sub,
            username = s.User.username
          }
        }).ToList()
      };

      return channelDTO;
    }


    // GET: api/Channels/Users/5
    [HttpGet("Users/{id}")]
    public async Task<ActionResult<IEnumerable<ChannelDTO>>> GetChannelsByUser(string id)
    {
      var channels = await _context.Channels
          .Include(c => c.UserChannels).ThenInclude(uc => uc.User)
          .Where(c => c.UserChannels.Any(cu => cu.UserId == id))
          .ToListAsync();

      if (channels == null)
      {
        return NotFound();
      }

      var channelDTOs = channels.Select(channel => new ChannelDTO
      {
        Id = channel.Id,
        Name = channel.Name,
        Description = channel.Description,
        IsPrivate = channel.IsPrivate,
        Users = channel.UserChannels.Select(uc => new UserDTO
        {
          sub = uc.User.sub,
          username = uc.User.username
        }).ToList()
        // ,
        // Messages = channel.Messages.Select(m => new MessageDTO
        // {
        //     Id = m.Id,
        //     Text = m.Text,
        //     Timestamp = m.Timestamp,
        //     SenderId = m.SenderId,
        //     Sender = new UserDTO
        //     {
        //         Id = m.Sender.Id,
        //         CognitoUsername = m.Sender.CognitoUsername
        //         // Assign other properties
        //     }
        //     // Assign other properties
        // }).ToList()
      }).ToList();

      return channelDTOs;
    }



    // // GET: api/Channels/5/Messages
    // [HttpGet("{id}/Messages")]
    // public async Task<ActionResult<IEnumerable<Message>>> GetChannelMessages(string id)
    // {
    //   var channel = await _context.Channels.FindAsync(id);

    //   if (channel == null)
    //   {
    //     return NotFound();
    //   }

    //   return channel.Messages;
    // }

    // // PUT: api/Channels/5
    // [HttpPut("{id}")]
    // public async Task<IActionResult> PutChannel(string id, Channel channel)
    // {
    //   if (id != channel.Id)
    //   {
    //     return BadRequest();
    //   }

    //   _context.Entry(channel).State = EntityState.Modified;

    //   try
    //   {
    //     await _context.SaveChangesAsync();
    //   }
    //   catch (DbUpdateConcurrencyException)
    //   {
    //     if (!ChannelExists(id))
    //     {
    //       return NotFound();
    //     }
    //     else
    //     {
    //       throw;
    //     }
    //   }

    //   return NoContent();
    // }

    // // POST: api/Channels
    // [HttpPost]
    // public async Task<ActionResult<Channel>> PostChannel(Channel channel)
    // {
    //   _context.Channels.Add(channel);
    //   await _context.SaveChangesAsync();

    //   // Send a message to all clients listening to the hub
    //   await _hub.Clients.All.SendAsync("ReceiveMessage", channel);

    //   return CreatedAtAction("GetChannel", new { id = channel.Id }, channel);
    // }

    // POST: api/Channels/5/Users
    [HttpPost("{channelId}/Users")]
    public async Task<IActionResult> PostChannelUser(int channelId, User user)
    {
      var channel = await _context.Channels.FindAsync(channelId);

      if (channel == null || user == null)
      {
        return NotFound();
      }

      var userChannel = new UserChannel { UserId = user.sub, ChannelId = channelId };

      _context.UserChannels.Add(userChannel);
      await _context.SaveChangesAsync();

      await _hub.Clients.Group(channelId.ToString()).SendAsync("UserJoined", user);

      return Ok();
    }



    // DELETE: api/Channels/5
    [HttpDelete("{channelId}/Users/{userId}")]
    public async Task<IActionResult> DeleteChannelUser(int channelId, string userId)
    {
      var userChannel = await _context.UserChannels
          .Where(cu => cu.ChannelId == channelId && cu.UserId == userId)
          .FirstOrDefaultAsync();

      if (userChannel == null)
      {
        return NotFound();
      }

      _context.UserChannels.Remove(userChannel);
      await _context.SaveChangesAsync();

      var user = await _context.Users.FindAsync(userId);

      var userDTO = new UserDTO
      {
        sub = user.sub,
        username = user.username
      };

      await _hub.Clients.Group(channelId.ToString()).SendAsync("UserLeft", userDTO);

      return Ok();
    }


    // DELETE: api/Channels/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteChannel(int id)
    {
      var channel = await _context.Channels.FindAsync(id);
      if (channel == null)
      {
        return NotFound();
      }

      _context.Channels.Remove(channel);
      await _context.SaveChangesAsync();

      return NoContent();
    }

    private bool ChannelExists(int id)
    {
      return _context.Channels.Any(e => e.Id == id);
    }
  }
}