using System;

namespace BullsAndCows
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Game game = new Game();
            Console.WriteLine("Hi. Hice to see you in game " + Game.GAME_NAME);

            game.StartGame();

            Console.ReadKey();
        }
    }
}