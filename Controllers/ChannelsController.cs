using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HeyListen.Models;
using HeyListen.Data;
using HeyListen.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace HeyListen.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
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
    public async Task<ActionResult<Channel>> GetChannel(string id)
    {
      var channel = await _context.Channels.FindAsync(id);

      if (channel == null)
      {
        return NotFound();
      }

      return channel;
    }

    // GET: api/Channels/5/Messages
    [HttpGet("{id}/Messages")]
    public async Task<ActionResult<IEnumerable<Message>>> GetChannelMessages(string id)
    {
      var channel = await _context.Channels.FindAsync(id);

      if (channel == null)
      {
        return NotFound();
      }

      return channel.Messages;
    }

    // PUT: api/Channels/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutChannel(string id, Channel channel)
    {
      if (id != channel.Id)
      {
        return BadRequest();
      }

      _context.Entry(channel).State = EntityState.Modified;

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!ChannelExists(id))
        {
          return NotFound();
        }
        else
        {
          throw;
        }
      }

      return NoContent();
    }

    // POST: api/Channels
    [HttpPost]
    public async Task<ActionResult<Channel>> PostChannel(Channel channel)
    {
      _context.Channels.Add(channel);
      await _context.SaveChangesAsync();

      // Send a message to all clients listening to the hub
      await _hub.Clients.All.SendAsync("ReceiveMessage", channel);

      return CreatedAtAction("GetChannel", new { id = channel.Id }, channel);
    }

    [HttpPost("{id}/Messages")]
    public async Task<Message> PostChannelMessage(string channelId, Message Message)
    {
      Message.ChannelId = channelId;
      _context.Messages.Add(Message);
      await _context.SaveChangesAsync();

      await _hub.Clients.Group(channelId).SendAsync("ReceiveMessage", Message);

      return Message;
    }



    // DELETE: api/Channels/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteChannel(string id)
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

    private bool ChannelExists(string id)
    {
      return _context.Channels.Any(e => e.Id == id);
    }
  }
}