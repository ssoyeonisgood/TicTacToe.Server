using Microsoft.AspNetCore.SignalR;
using TicTacToe.Server.Services;

namespace TicTacToe.Server.Hubs;

public class GameHub : Hub
{
    private readonly GameService _gameService;

    public GameHub(GameService gameService)
    {
        _gameService = gameService;
    }

    public async Task CreateGame(string playerName)
    {
        var game = _gameService.CreateGame(playerName);

        // Add creator to a SignalR group with the gameId
        await Groups.AddToGroupAsync(Context.ConnectionId, game.GameId);

        await Clients.Caller.SendAsync("GameCreated", game);
    }

    public async Task JoinGame(string gameId, string playerName)
    {
        var game = _gameService.GetGame(gameId);

        if (game == null)
        {
            await Clients.Caller.SendAsync("Error", "Game not found.");
            return;
        }

        game.PlayerO = playerName;

        // Add joining user to the group
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);

        // Notify both X and O players
        await Clients.Group(gameId).SendAsync("GameJoined", game);
    }

    public async Task MakeMove(string gameId, int index)
    {
        var game = _gameService.MakeMove(gameId, index);

        if (game == null)
        {
            await Clients.Caller.SendAsync("Error", "Game not found.");
            return;
        }

        // Send updated game state to everyone in the room
        await Clients.Group(gameId).SendAsync("MoveMade", game);
    }
}

