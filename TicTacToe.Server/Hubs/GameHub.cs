using Microsoft.AspNetCore.SignalR;
using TicTacToe.Server.Models;
using TicTacToe.Server.Services;

namespace TicTacToe.Server.Hubs;

public class GameHub : Hub
{
    private readonly GameService _gameService;

    public GameHub(GameService gameService)
    {
        _gameService = gameService;
    }

    public async Task LoginGame(string playerName)
    {
        var isExist = _gameService.DoesUserExist(playerName);

        if (!isExist)
        {
            await Clients.Caller.SendAsync("Error", "User does not exist.");
            return;
        }
        await Clients.Caller.SendAsync("UserLogedIn", isExist);
    }

    public async Task SignUpGame(string playerName)
    {
        var isOk = _gameService.CreateUser(playerName);
        if (!isOk)
        {
            await Clients.Caller.SendAsync("Error", "Player name is already taken.");
            return;
        }

        await Clients.Caller.SendAsync("UserSignedUp", isOk);
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
        var player = new User
        {
            Name = playerName
        };

        game.Player2 = player;

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

        if (game.Player1.Symbol == '\0' || game.Player2.Symbol == '\0')
        {
            await Clients.Caller.SendAsync("Error", "All players have not decided the symbol yet.");
            return;
        }

        // Send updated game state to everyone in the room
        await Clients.Group(gameId).SendAsync("MoveMade", game);
    }
}

