using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using HeyListen.Models;
using HeyListen.Hubs;
using HeyListen.Data;
using Newtonsoft.Json;

namespace HeyListen.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
  private readonly HeyListenDbContext _context;

  private readonly IHubContext<ChatHub> _hub;

  public MessagesController(HeyListenDbContext context, IHubContext<ChatHub> hub)
  {
    _context = context;
    _hub = hub;
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<Message>>> GetMessages()
  {
    return await _context.Messages.ToListAsync();
  }


  [HttpGet("{id}")]
  public async Task<ActionResult<Message>> GetMessage(int id)
  {
    var Message = await _context.Messages.FindAsync(id);

    if (Message == null)
    {
      return NotFound();
    }

    return Message;
  }


  [HttpPost]
  public async Task<ActionResult<Message>> PostMessage([FromBody] Message message)
  {
    var channel = await _context.Channels.FindAsync(message.ChannelId);

    if (channel == null)
    {
      return NotFound("Channel not found");
    }

    _context.Messages.Add(message);
    await _context.SaveChangesAsync();

    var newMessage = await _context.Messages.Where(m => m.Id == message.Id).Include(m => m.Sender).FirstOrDefaultAsync();

    await _hub.Clients.Group(message.ChannelId.ToString()).SendAsync("ReceiveMessage", newMessage);

    return Ok();
  }

  [HttpPut("{id}")]
  public async Task<IActionResult> PutMessage(int id, Message message)
  {
    if (id != message.Id)
    {
      return BadRequest();
    }

    var channel = await _context.Channels.FindAsync(message.ChannelId);

    if (channel == null)
    {
      return NotFound("Channel not found");
    }

    _context.Entry(message).State = EntityState.Modified;
    await _context.SaveChangesAsync();

    await _hub.Clients.Group(message.ChannelId.ToString()).SendAsync("MessageUpdated", message);

    return NoContent();
  }


  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteMessage(int id)
  {
    var Message = await _context.Messages.FindAsync(id);
    if (Message == null)
    {
      return NotFound();
    }

    _context.Messages.Remove(Message);
    await _context.SaveChangesAsync();

    await _hub.Clients.Group(Message.ChannelId.ToString()).SendAsync("MessageDeleted", Message.Id);

    return NoContent();
  }
}

