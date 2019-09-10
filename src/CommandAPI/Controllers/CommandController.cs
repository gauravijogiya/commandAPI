using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using CommandAPI.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace CommandAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly CommandContext _context;
        private IHostingEnvironment _hostEnv;
        public CommandsController(CommandContext context, IHostingEnvironment hostEnv)
        {
            _context = context;
            _hostEnv = hostEnv;
        }
        [HttpGet]
        public ActionResult<IEnumerable<Command>> Get()
        {
            if (Response != null)
                Response.Headers.Add("Environment", _hostEnv.EnvironmentName);
            return _context.commandItems;

        }
        [HttpGet("{id}")]

        public ActionResult<Command> Get(int id)
        {
            var result = _context.commandItems.Find(id);
            if (result == null)
                return NotFound();
            return result;
        }
        [HttpPost]
        public ActionResult<Command> PostCommandItem(Command command)
        {
            _context.commandItems.Add(command);
            try
            {

                _context.SaveChanges();
            }
            catch
            {
                return BadRequest();
            }

            return CreatedAtAction("Get", new Command { Id = command.Id }, command);

        }

        [HttpPut("{id}")]
        public ActionResult PutCommandItem(int id, Command command)
        {
            if (id != command.Id)
                return BadRequest();
            _context.Entry(command).State = EntityState.Modified;
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult<Command> DeleteCommandItem(int id)
        {
            var commandItem = _context.commandItems.Find(id);

            if (commandItem == null)
                return NotFound();

            _context.commandItems.Remove(commandItem);
            _context.SaveChanges();
            return commandItem;
        }
    }
}