namespace TicTacToe.Server.Models
{
    public class GameState
    {
        public string GameId { get; set; } = Guid.NewGuid().ToString();
        public char[] Board { get; set; } = new char[9];
        public User? Player1 { get; set; }
        public User? Player2 { get; set; }
        public User? CurrentTurn { get; set; }
        public bool IsFinished { get; set; } = false;
        public string? Winner { get; set; }
    }
}
