using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Humanizer;
using Spectre.Console;

namespace SpeedType
{
    public class Game
    {
        /// <summary>
        /// The provider responsible for generating the sentences for the game.
        /// </summary>
        private readonly SentenceProvider sentenceProvider;

        /// <summary>
        /// The evaluator responsible for calculating the user's performance 
        /// (WPM and accuracy).
        /// </summary>
        private readonly Evaluator evaluator;

        /// <summary>
        /// A list to store the last 5 game results for the game stats board.
        /// </summary>
        private readonly GameResult[] gameStats;

        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class.
        /// Sets up the sentence provider, evaluator, and initializes the game 
        /// stats board.
        /// </summary>
        public Game()
        {
            sentenceProvider = new SentenceProvider();
            gameStats = new GameResult[5];
            evaluator = new Evaluator();
        }

        /// <summary>
        /// Displays the main menu of the game and prompts the user to choose an
        /// option.
        /// The available choices are to start the game, view the game stats 
        /// board, or quit the game.
        /// </summary>
        /// <remarks>
        /// This method uses the <see cref="Spectre.Console"/> library to 
        /// display the main menu and handle user input for choosing different 
        /// options. The game will continue prompting 
        /// the user until they choose to quit.
        /// </remarks>
        public void ShowMenu()
        {
            while (true)
            {
                AnsiConsole.Clear();
                string choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold yellow]Speed Type[/]")
                        .AddChoices("Start Game", "View Game Stats", "View Bar Chart", "Quit"));

                switch (choice)
                {
                    case "Start Game":
                        StartGame();
                        break;
                    case "View Game Stats":
                        ShowGameStats();
                        break;
                    case "View Bar Chart":
                        ShowBarChart();
                        break;
                    case "Quit":
                        return;
                }
            }
        }

        /// <summary>
        /// Starts a new game round where the player types a randomly generated 
        /// sentence. The game measures the time taken and the accuracy of the 
        /// input.
        /// </summary>
        /// <remarks>
        /// In this method, a random sentence is generated using the 
        /// <see cref="SentenceProvider"/>.
        /// The player must type the sentence within the given time. After the 
        /// player submits their input, the game calculates the Words Per Minute
        /// (WPM) and accuracy, and then records the result in the game stats 
        /// board. The game stats board only stores the last 5 results.
        /// </remarks>
        private void StartGame()
        {
            // The sentence that will be presented to the player.
            string sentence = new SentenceProvider().GetRandomSentence().Humanize();

            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[bold green]Type This Sentence:[/]");
            AnsiConsole.MarkupLine($"[italic yellow]{sentence}[/]");
            AnsiConsole.Markup("\n[gray]Press Enter When Ready...[/]");
            Console.ReadLine();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string userInput = AnsiConsole.Ask<string>("\n[bold cyan]Start" +
                " Typing:[/] ");
            stopwatch.Stop();

            // The time taken by the user to type the sentence.
            double timeTaken = stopwatch.Elapsed.TotalSeconds;

            // The words per minute (WPM) calculated based on the time taken 
            // and the user input.
            double wpm = evaluator.CalculateWPM(userInput, timeTaken);
            
            // The accuracy percentage calculated based on the user's input and
            // the original sentence.
            int accuracy = evaluator.CalculateAccuracy(userInput, sentence);

            // Shift existing entries
            for (int i = gameStats.Length - 1; i > 0; i--)
            {
                // ////////// => TO IMPLEMENT <= //////////// //
                gameStats[i] = gameStats[i - 1];
            }

            // Add new result at the beginning
            gameStats[0] = new GameResult(wpm, accuracy, timeTaken);

            AnsiConsole.MarkupLine("\n[bold yellow]Results:[/]");
            AnsiConsole.MarkupLine($"[bold]Time Taken:[/] {timeTaken:F2} " +
                "Seconds");
            AnsiConsole.MarkupLine(
                $"[bold]Words Per Minute (WPM):[/] {wpm:F2}");
            AnsiConsole.MarkupLine($"[bold]Accuracy:[/] {accuracy}%");

            AnsiConsole.Markup("\n[bold green]Press Enter to Return to " +
                "Menu...[/]");
            Console.ReadLine();
        }

        /// <summary>
        /// Displays the game stats board showing the last 5 results with WPM, 
        /// accuracy, and time taken.
        /// </summary>
        /// <remarks>
        /// This method uses <see cref="Spectre.Console"/> to format and display
        /// a table with 
        /// the game stats board results. Each row displays the rank, WPM, 
        /// accuracy, and time taken for each entry.
        /// </remarks>
        private void ShowGameStats()
        {
            AnsiConsole.Clear();
            Table table = new Table();
            table.AddColumn("#");
            table.AddColumn("WPM");
            table.AddColumn("Accuracy");
            table.AddColumn("Time Taken (s)");

            for (int i = 0; i < gameStats.Length; i++)
            {
                if (gameStats[i] == null) break;
                table.AddRow($"{i+1}", $"{gameStats[i].WPM:F2}", $"{gameStats[i].Accuracy}%", $"{gameStats[i].TimeTaken:F2}");
            }

            AnsiConsole.Write(table);
            AnsiConsole.Markup("\n[bold green]Press Enter to Return to " +
                "Menu...[/]");
            Console.ReadLine();
        }

        private void ShowBarChart()
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("\n[bold yellow]Bar chart:[/]");
            BarChart barChart = new BarChart().Width(60);

            int[] numbers = new int[11];
            for (int i = 0; i < gameStats.Length; i++)
            {
                if (gameStats[i] == null) break;
                int pos = gameStats[i].Accuracy / 10;
                numbers[pos]++;
            }

            Random rand = new Random();
            string[] values = new string[11] {"0%-9%", "10%-19%", "20%-29%", "30%-39%", "40%-49%", "50%-59%", "60%-69%", "70%-79%", "80%-89%", "90%-99%", "100%"};
            for (int i = 0; i < 11; i++)
            {
                barChart.AddItem(values[i], numbers[i], new Color((byte)rand.Next(0, 255), (byte)rand.Next(0, 255), (byte)rand.Next(0, 255)));
            }
            
            AnsiConsole.Write(barChart);
            AnsiConsole.Markup("\n[bold green]Press Enter to Return to " +
                               "Menu...[/]");
            Console.ReadLine();
        }
    }
}