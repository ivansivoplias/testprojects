using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BullsAndCows
{
    public class Game
    {
        public const string GAME_NAME = "Bulls and Cows";
        public const int GAME_BASE = 10;
        public const int MAX_NUMBERS = 10;
        public static Random random = new Random();
        private List<string> _gameSet;

        private readonly int _numberOfDigits = 4;

        public Game()
        {
            _gameSet = new List<string>();
        }

        public void StartGame()
        {
            GenerateGameSet();
            Play();
        }

        /// <summary>
        /// Generates all games combinations and writes it to list in increasing order
        /// </summary>
        private void GenerateGameSet()
        {
            double combinationsCount = GetCombinationsNumber();
            for (int i = 0; i < combinationsCount; i++)
            {
                _gameSet.Add(i.ToString("d" + _numberOfDigits));
            }
        }

        //Max - 10 000 combinations
        /// <summary>
        /// Calculates number of combinations in current game
        /// </summary>
        /// <returns>number of combinations</returns>
        private double GetCombinationsNumber()
        {
            return Power(GAME_BASE, _numberOfDigits);
        }

        private int Power(int number, int power)
        {
            if (power == 0) return 1;
            int y = 1;
            while (power > 1)
            {
                if (power % 2 == 0)
                {
                    number *= number;
                    power /= 2;
                }
                else
                {
                    y = number * y;
                    number *= number;
                    power = (power - 1) / 2;
                }
            }
            return number * y;
        }

        private void Play()
        {
            Console.WriteLine("Input answer like this: perhaps number was 1158, assumption is 1866.");
            Console.WriteLine("Than answer should be: \"1,1\", because we have one and it on right place(bull), and we have 8 and it on wrong place(cow).");

            string guess;
            int index = -1;

            while (_gameSet.Count > 1)
            {
                index = random.Next(_gameSet.Count);
                guess = _gameSet[index];

                int bulls, cows;
                do
                {
                    Console.Write("My guess is {0}. How many bulls, cows? ", guess);
                } while (!ReadUserAnswer(out bulls, out cows));

                KnuthCheck(guess, bulls, cows);
            }
            if (_gameSet.Count == 1)
            {
                Console.WriteLine("Hooray! The answer is {0}!", _gameSet[0]);
            }
            else
            {
                Console.WriteLine("No possible answer fits the scores you gave.");
            }
        }

        private bool ReadUserAnswer(out int bulls, out int cows)
        {
            var result = true;
            string[] input = Console.ReadLine().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            bulls = cows = 0;
            if (input.Length == 2)
            {
                int tempBulls = -1, tempCows = -1;
                bool parseFirst = int.TryParse(input[0], out tempBulls);
                bool parseSecond = int.TryParse(input[1], out tempCows);
                if (parseFirst && parseSecond && tempBulls >= 0 && tempCows >= 0)
                {
                    if (tempBulls + tempCows <= _numberOfDigits)
                    {
                        bulls = tempBulls;
                        cows = tempCows;
                    }
                    else
                    {
                        Console.WriteLine("Sum of bulls and cows can't be greater than " + _numberOfDigits);
                        result = false;
                    }
                }
                else
                {
                    Console.WriteLine("Wrong input. Please try input positive numeric values, like \"0,1\" and send it again.");
                    result = false;
                }
            }
            else
            {
                result = false;
                Console.WriteLine("Wrong input length. Please input values like: \"0,1\" and submit again.");
            }
            return result;
        }

        private void KnuthCheck(string guess, int bulls, int cows)
        {
            var newList = new List<string>();
            for (int i = _gameSet.Count - 1; i >= 0; i--)
            {
                int countBulls = 0, countCows = 0;
                string temp = _gameSet[i];
                string tempGuess = guess;
                for (int numIndex = 0; numIndex < _numberOfDigits; numIndex++)
                {
                    if (temp[numIndex] == tempGuess[numIndex])
                    {
                        countBulls++;
                        temp = ReplaceChar(temp, numIndex);
                        tempGuess = ReplaceChar(tempGuess, numIndex, '?');
                    }
                }

                for (int numIndex = 0; numIndex < _numberOfDigits; numIndex++)
                {
                    int cowIndex = temp.IndexOf(tempGuess[numIndex]);
                    if (cowIndex != -1)
                    {
                        countCows++;
                        temp = ReplaceChar(temp, cowIndex);
                    }
                }

                if ((countBulls == bulls) && (countCows == cows))
                {
                    newList.Add(_gameSet[i]);
                }
            }
            _gameSet = newList;
        }

        private string ReplaceChar(string source, int index, char newValue = '*')
        {
            var sourceArray = source.ToCharArray();
            sourceArray[index] = newValue;
            return new string(sourceArray);
        }
    }
}