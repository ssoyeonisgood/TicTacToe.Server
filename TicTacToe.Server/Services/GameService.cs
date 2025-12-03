using TicTacToe.Server.Models;
using System.Collections.Concurrent;

namespace TicTacToe.Server.Services;

public class GameService
{


    // ConcurrentDictionary is thread-safe, allowing multiple users to play simultaneously
    private readonly ConcurrentDictionary<string, GameState> _games = new();
    private readonly List<User> _users = new List<User>();


    public bool CreateUser(string playerName)
    {
        var existedUser = DoesUserExist(playerName);
        if (existedUser != null)
        {
            return false;
        }
        var player = new User
        {
            Name = playerName
        };

        _users.Add(player);

        return true;
    }
    public GameState CreateGame(string playerName)
    {
        var user = _users.FirstOrDefault(u => u.Name == playerName);
        var game = new GameState
        {
            Player1 = user
        };

        _games[game.GameId] = game;
        return game;
    }

    public List<User> AddUser(User user)
    {
        _users.Add(user);
        return _users;
    }

    public User? SetSymbol(string playerName, char symbol)
    {
        var user = _users.FirstOrDefault(u => u.Name == playerName);
        if (user == null)
            return null;

        user.Symbol = symbol;
        return user;
    }

    public GameState? SetFirstTurn(string gameId, User user)
    {
        if (!_games.TryGetValue(gameId, out var game))
            return null;

        game.CurrentTurn = user;
        return game;
    }

    public User DoesUserExist(string playerName)
    {
        var user = _users.FirstOrDefault(u => u.Name == playerName);
        return user;
    }

    public GameState? MakeMove(string gameId, int index)
    {
        if (!_games.TryGetValue(gameId, out var game))
            return null;

        // '\0' represents an empty cell (no X or O placed yet)
        if (game.IsFinished || game.Board[index] != '\0')
            return game;

        game.Board[index] = game.CurrentTurn.Symbol == 'X' ? 'X' : 'O';

        CheckWinner(game);

        if (!game.IsFinished)
            game.CurrentTurn = game.CurrentTurn == game.Player1 ? game.Player2 : game.Player1;

        return game;
    }

    private void CheckWinner(GameState game)
    {
        // All 8 winning combinations
        int[][] wins = {
            new[]{0,1,2}, new[]{3,4,5}, new[]{6,7,8}, // Horizontal
            new[]{0,3,6}, new[]{1,4,7}, new[]{2,5,8}, // Vertical
            new[]{0,4,8}, new[]{2,4,6}               // Diagonal
        };

        foreach (var w in wins)
        {
            if (game.Board[w[0]] != '\0' &&             // First cell must not be empty
                game.Board[w[0]] == game.Board[w[1]] && // First equals second
                game.Board[w[1]] == game.Board[w[2]])   // Second equals third
            {
                game.IsFinished = true;
                // game.Board[w[0]] is a char ('X' or 'O'), convert to string for Winner
                game.Winner = game.Board[w[0]].ToString();
                return;
            }
        }

        // Draw check (no empty cells left)
        if (game.Board.All(c => c != '\0'))
        {
            game.IsFinished = true;
            game.Winner = "Draw";
        }
    }

    public GameState? GetGame(string gameId)
    {
        _games.TryGetValue(gameId, out var game);
        return game;
    }

    public IReadOnlyDictionary<string, GameState> GetAllGame()
    {
        return _games;
    }
}

