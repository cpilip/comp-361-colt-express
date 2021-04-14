using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;


namespace WebApi.Controllers
{
    [ApiController]
    public class LobbyServiceController : ControllerBase
    {
        private readonly IHostApplicationLifetime _applicationLifetime;

        private readonly ILogger<LobbyServiceController> _logger;

        public LobbyServiceController(ILogger<LobbyServiceController> logger, IHostApplicationLifetime applicationLifetime)
        {
            _logger = logger;
            _applicationLifetime = applicationLifetime;
        }

        [HttpPut("api/games/{gameID}")]
        public String StartGame(string gameID, [FromBody] JsonElement body)
        {
            string json = System.Text.Json.JsonSerializer.Serialize(body);
            Console.WriteLine(json);
            LaunchGameJson o = JObject.Parse(json).ToObject<LaunchGameJson>();
            _applicationLifetime.StopApplication();
            // MyTcpListener.playGame(o.savegame, o.players.Count);
            return "startgame";
        }

        [HttpDelete("api/games/{gameID}")]
        public String StopGame(string gameID)
        {
            Console.WriteLine("Stop game");
            return "Stop game";
        }
    }
}
