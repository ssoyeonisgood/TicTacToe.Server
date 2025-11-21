using Microsoft.AspNetCore.Mvc;
using TicTacToe.Server.Models;
using TicTacToe.Server.Services;

namespace TicTacToe.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class GameController : ControllerBase
{
    private readonly GameService _gameService;
    public GameController(GameService gameService)
    {
        _gameService = gameService;
    }

    [HttpPost("create")]
    public IActionResult Create([FromBody] CreateGameRequest request)
    {
        if (string.IsNullOrEmpty(request.PlayerName))
            return BadRequest("Player name required");

        var game = _gameService.CreateGame(request.PlayerName);
        return Ok(game);
    }

    [HttpPost("move")]
    public ActionResult<GameState> Move([FromQuery] string gameId, [FromQuery] int index)
    {
        var game = _gameService.MakeMove(gameId, index);
        if (game == null)
            return NotFound("Can not find the game.");

        return Ok(game);
    }

    [HttpGet("games")]
    public ActionResult<IReadOnlyDictionary<string, GameState>> GetAllGames()
    {
        IReadOnlyDictionary<string, GameState> games = _gameService.GetAllGame();
        return Ok(games);
    }
}

public class CreateGameRequest
{
    public string PlayerName { get; set; }
}
    