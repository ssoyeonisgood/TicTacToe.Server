namespace TicTacToe.Server.Models
{
    public class GameState
    {
        public string GameId { get; set; } = Guid.NewGuid().ToString();
        public char[] Board { get; set; } = new char[9];
        public string PlayerX { get; set; }
        public string PlayerO { get; set; }
        public char CurrentTurn { get; set; } = 'X';
        public bool IsFinished { get; set; } = false;
        public string Winner { get; set; }
    }
}
