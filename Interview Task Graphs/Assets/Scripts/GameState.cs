namespace Game
{
    public class GameState
    {
        public int Score { get; private set; }

        public void AddScore(int score)
        {
            Score += score;
        }
    }
}