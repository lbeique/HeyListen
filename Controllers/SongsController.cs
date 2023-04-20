using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using HeyListen.Models;
using HeyListen.Hubs;
using HeyListen.Data;

namespace HeyListen.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SongsController : ControllerBase
    {
        private readonly HeyListenDbContext _context;
        private readonly IHubContext<MusicHub> _hub;

        public SongsController(HeyListenDbContext context, IHubContext<MusicHub> hub)
        {
            _context = context;
            _hub = hub;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Song>>> GetSongs()
        {
            return await _context.Songs.Include(s => s.User).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Song>> GetSong(int id)
        {
            var song = await _context.Songs.FindAsync(id);

            if (song == null)
            {
                return NotFound();
            }

            return song;
        }

        [HttpPost]
        public async Task<ActionResult<Song>> PostSong([FromBody] Song song)
        {
            var channel = await _context.Channels.FindAsync(song.ChannelId);

            if (channel == null)
            {
                return NotFound("Channel not found");
            }

            _context.Songs.Add(song);
            await _context.SaveChangesAsync();

            var newSong = await _context.Songs.Where(s => s.Id == song.Id).Include(s => s.User).FirstOrDefaultAsync();

            await _hub.Clients.Group(song.ChannelId.ToString()).SendAsync("ReceiveSong", newSong);

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutSong(int id, Song song)
        {
            if (id != song.Id)
            {
                return BadRequest();
            }

            var channel = await _context.Channels.FindAsync(song.ChannelId);

            if (channel == null)
            {
                return NotFound("Channel not found");
            }

            _context.Entry(song).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            await _hub.Clients.Group(song.ChannelId.ToString()).SendAsync("SongUpdated", song);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSong(int id)
        {
            var song = await _context.Songs.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }

            _context.Songs.Remove(song);
            await _context.SaveChangesAsync();

            await _hub.Clients.Group(song.ChannelId.ToString()).SendAsync("SongDeleted", song.Id);

            return NoContent();
        }
    }
}