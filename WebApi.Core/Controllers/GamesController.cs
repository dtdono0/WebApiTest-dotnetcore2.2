using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebApi.Core.Contexts;
using WebApi.Core.Models;

namespace WebApi.Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly GamesContext _context;
        private readonly IConfiguration _config;

        public GamesController(GamesContext context, IConfiguration configuration)
        {
            _context = context;
            _config = configuration;
        }

        // GET: api/Games
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Game>>> GetGame()
        {
            // Test to read data from appsettings.json in a controller
            var appSettings = _config.GetSection("AppSettings");
            Debug.WriteLine("AppSettings.Key : " + appSettings.Key);
            Debug.WriteLine("MainGame : " + _config.GetValue<string>("AppSettings:MainGame"));

            Debug.WriteLine("MainBad : " + _config.GetValue<string>("AppSettings:BadGames:MainBad"));
            Debug.WriteLine("AltBad : " + _config.GetValue<string>("AppSettings:BadGames:AltBad"));
            Debug.WriteLine("---------------");
            
            foreach (KeyValuePair<string, string> pair in appSettings.GetSection("BadGames").AsEnumerable())
            {
                Debug.WriteLine(@"key : {0} value : {1}", pair.Key, pair.Value);
            }
            Debug.WriteLine("---------------");

            foreach (IConfigurationSection section in appSettings.GetChildren())
            {
                foreach (KeyValuePair<string, string> pair in section.AsEnumerable()) {
                    Debug.WriteLine(@"key : {0} value : {1}", pair.Key, pair.Value);
                }
            }
            
            return await _context.Game.ToListAsync();
        }

        // GET: api/Games/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Game>> GetGame(int id)
        {
            var game = await _context.Game.FindAsync(id);

            if (game == null)
            {
                return NotFound();
            }

            return game;
        }

        // PUT: api/Games/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGame(int id, Game game)
        {
            if (id != game.Id)
            {
                return BadRequest();
            }

            _context.Entry(game).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameExists(id))
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

        // POST: api/Games
        [HttpPost]
        public async Task<ActionResult<Game>> PostGame(Game game)
        {
            _context.Game.Add(game);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGame", new { id = game.Id }, game);
        }

        // DELETE: api/Games/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Game>> DeleteGame(int id)
        {
            var game = await _context.Game.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }

            _context.Game.Remove(game);
            await _context.SaveChangesAsync();

            return game;
        }

        private bool GameExists(int id)
        {
            return _context.Game.Any(e => e.Id == id);
        }
    }
}
