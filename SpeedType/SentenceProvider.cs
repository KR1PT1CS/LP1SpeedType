using System;
using System.IO;

namespace SpeedType
{
    public class SentenceProvider
    {
        private readonly string[] sentences;
        private readonly Random random;

        /// <summary>
        /// Initializes a new instance of the <see cref="SentenceProvider"/> 
        /// class. Reads sentences from a file and stores them in an array. 
        /// If the file is not found, a default sentence is used.
        /// </summary>
        /// <remarks>
        /// This constructor attempts to read a file named "sentences.txt" 
        /// located in the directory two levels above the application's base 
        /// directory. Each line of the file is processed using the 
        /// <see cref="Humanizer"/> library's <c>Humanize()</c> method before 
        /// being added to the array. If the file does not exist, the array 
        /// will contain a single default sentence: "No Sentences Found! Please 
        /// Provide a File."
        /// </remarks>
        public SentenceProvider()
        {
            random = new Random();
            //random = // ////////// => TO IMPLEMENT <= //////////// //
            
            string directoryPath = Path.GetFullPath(
                Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\"
                )
            );
            string filePath = Path.Combine(directoryPath, "sentences.txt");

            if (File.Exists(filePath))
            {
                StreamReader sr = File.OpenText(filePath);
                int countLines = sr.ReadToEnd().Split("\n").Length;
                sr.Close();
                sr = File.OpenText(filePath);
                sentences = new string[countLines];
                string s;
                int index = 0;
                while ((s = sr.ReadLine()) != null)
                {
                    sentences[index] = s;
                    index++;
                }
            }
            else
            {
                sentences = new[] { "No Sentences Found! Please Provide a"
                    + " File." };
            }
        }

        /// <summary>
        /// Retrieves a random sentence from the array of sentences.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> representing a randomly selected sentence 
        /// from the array of sentences.
        /// </returns>
        public string GetRandomSentence()
        {
            return sentences[random.Next(0, sentences.Length)];
        }
    }
}