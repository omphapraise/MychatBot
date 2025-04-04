using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Text.Json;
using System.Threading;
using CybersecurityAwarenessBot.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CybersecurityAwarenessBot.Configuration;
using CybersecurityAwarenessBot.UI;

namespace CybersecurityAwarenessBot
{
    internal class Program
    {
        private static readonly Random _random = new Random();
        private static readonly string _asciiLogoPath = "ascii_logo.txt";
        private static readonly string _welcomeAudioPath = "CybersecurityBotGreeting.wav";
        private static readonly string _responsesPath = "responses.json";
        private static readonly string _tipsPath = "cybertips.json"; // Optional file
        private static readonly string _jokesPath = "jokes.json"; // Optional file
        private static readonly string _challengesPath = "challenges.json"; // Optional file
        private static ChatbotService _chatbotService = null!; // Using null! with nullable enabled

        static void Main(string[] args)
        {
            try
            {
                Console.Title = "Cybersecurity Awareness Bot";
            }
            catch (Exception)
            {
                // Silently handle title setting error (e.g., when running in environments that don't support console titles)
            }

            InitializeChatbot();
            DisplayAsciiLogo();
            PlayWelcomeMessage();

            string userName = PromptForUserName();
            DisplayPersonalizedWelcome(userName);

            StartInteractiveSession(userName);
        }

        private static void InitializeChatbot()
        {
            try
            {
                // Check if optional files exist and pass them to the constructor only if they do
                string? tipsFilePath = File.Exists(_tipsPath) ? _tipsPath : null;
                string? jokesFilePath = File.Exists(_jokesPath) ? _jokesPath : null;
                string? challengesFilePath = File.Exists(_challengesPath) ? _challengesPath : null;

                _chatbotService = new ChatbotService(
                    _responsesPath,
                    tipsFilePath,
                    jokesFilePath,
                    challengesFilePath);
            }
            catch (FileNotFoundException)
            {
                TypeWithAnimation("Error: responses.json file not found. Creating a default file...", 5, 15);
                CreateDefaultResponsesFile();
                _chatbotService = new ChatbotService(_responsesPath);
            }
            catch (JsonException ex)
            {
                TypeWithAnimation($"Error parsing responses.json: {ex.Message}", 5, 15);
                TypeWithAnimation("Creating a default responses file...", 5, 15);
                CreateDefaultResponsesFile();
                _chatbotService = new ChatbotService(_responsesPath);
            }
            catch (Exception ex)
            {
                TypeWithAnimation($"Unexpected error initializing chatbot: {ex.Message}", 5, 15);
                Environment.Exit(1);
            }
        }

        private static void CreateDefaultResponsesFile()
        {
            var defaultResponses = new Dictionary<string, string>
            {
                ["how are you"] = "I'm functioning optimally and ready to help with cybersecurity awareness!",
                ["what's your purpose"] = "I'm designed to raise cybersecurity awareness and provide helpful tips to keep you safe online.",
                ["what can i ask you about"] = "You can ask me about password safety, phishing, safe browsing, or try commands like 'joke', 'challenge', and 'help'."
            };

            string json = JsonSerializer.Serialize(defaultResponses, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_responsesPath, json);
        }


        private static void DisplayAsciiLogo()
        {
            try
            {
                if (File.Exists(_asciiLogoPath))
                {
                    string asciiLogo = File.ReadAllText(_asciiLogoPath);
                    Console.WriteLine(asciiLogo);
                }
                else
                {
                    // Fallback ASCII art if file not found
                    Console.WriteLine(@"
_____       _                                      _ _         
 / ____|     | |                                    (_) |        
| |  __  ___ | |__   ___  _ __ ___   ___  ___ _   _ _| |_ _   _ 
| | |_ |/ _ \| '_ \ / _ \| '_ ` _ \ / _ \/ __| | | | | __| | | |
| |__| | (_) | |_) | (_) | | | | | |  __/ (__| |_| | | |_| |_| |
 \_____|\___/|_.__/ \___/|_| |_| |_|\___|\___|\__, |_|\__|\__, |
  _____         _                    _        __/ |       __/ |
 / ____|       | |                  | |      |___/       |___/ 
| (___   ___ _ | |__   ___ _ __   __| | ___ _ __                
 \___ \ / _ \| | '_ \ / _ \ '_ \ / _` |/ _ \ '__|               
 ____) |  __/| | |_) |  __/ | | | (_| |  __/ |                  
|_____/ \___|_|_.__/ \___|_| |_|\__,_|\___|_|                   
       _    _                                             
      | |  | |                                            
      | |__| | __ ___      _| | _____  _ __  ___  ___ ___ 
      |  __  |/ _` \ \ /\ / / |/ / _ \| '_ \/ __|/ __/ __|
      | |  | | (_| |\ V  V /|   < (_) | | | \__ \ (__\__ \
      |_|  |_|\__,_| \_/\_/ |_|\_\___/|_| |_|___/\___|___/
                                                          
                       +-+ +-+ +-+
                       |A| |B| |C|
                       +-+ +-+ +-+
");
                }
            }
            catch (Exception ex)
            {
                TypeWithAnimation($"Error displaying ASCII logo: {ex.Message}", 5, 15);
            }
        }

        private static void PlayWelcomeMessage()
        {
            try
            {
                if (File.Exists(_welcomeAudioPath))
                {
                    if (OperatingSystem.IsWindows())
                    {
                        using (SoundPlayer player = new SoundPlayer(_welcomeAudioPath))
                        {
                            player.Play();
                        }
                    }
                    else
                    {
                        TypeWithAnimation("Welcome! Audio playback is only supported on Windows.", 10, 30);
                    }
                }
                else
                {
                    TypeWithAnimation("Welcome audio file not found. Continuing silently.", 10, 30);
                }
            }
            catch (Exception ex)
            {
                TypeWithAnimation($"Unable to play welcome audio: {ex.Message}", 10, 30);
            }
        }

        private static string PromptForUserName()
        {
            string userName = string.Empty;
            int attempts = 0;
            const int maxAttempts = 3;

            while (string.IsNullOrWhiteSpace(userName) && attempts < maxAttempts)
            {
                attempts++;
                TypeWithAnimation("What's your name, cyber defender? ", 15, 40, false);
                userName = Console.ReadLine()?.Trim() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(userName) && attempts < maxAttempts)
                {
                    TypeWithAnimation("I didn't catch that. Let's try again.", 15, 40);
                }
            }

            // Provide a default name if user doesn't input one after max attempts
            return string.IsNullOrWhiteSpace(userName) ? "Cyber Ninja" : userName;
        }

        private static void DisplayPersonalizedWelcome(string userName)
        {
            string border = new string('=', 60);
            Console.WriteLine(border);
            TypeWithAnimation($"Welcome to Cybersecurity Awareness, {userName}!", 20, 50);
            TypeWithAnimation("I'm here to help you stay safe in the digital world.", 20, 50);
            Console.WriteLine(border);

            Console.WriteLine();
            TypeWithAnimation("Available topics:", 15, 40);
            Thread.Sleep(300);
            TypeWithAnimation("• password safety - Get password protection tips", 15, 30);
            TypeWithAnimation("• phishing - Learn about phishing prevention", 15, 30);
            TypeWithAnimation("• safe browsing - Discover internet safety strategies", 15, 30);

            Console.WriteLine();
            TypeWithAnimation("Type 'joke' for a humor break!", 15, 40);
            TypeWithAnimation("Type 'challenge' for a cybersecurity mini-challenge!", 15, 40);
            TypeWithAnimation("Type 'help' to see available commands", 15, 40);
            TypeWithAnimation("Type 'exit' to quit", 15, 40);
            Console.WriteLine();
        }

        private static void StartInteractiveSession(string userName)
        {
            while (true)
            {
                Console.Write($"\n{userName}> ");
                string input = Console.ReadLine()?.ToLower().Trim() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(input))
                {
                    TypeWithAnimation("Please enter a command or question. Type 'help' for options.", 10, 30);
                    continue;
                }

                switch (input)
                {
                    case "exit":
                        TypeWithAnimation($"Stay safe online, {userName}! Logging out...", 15, 40);
                        return;

                    case "help":
                        DisplayHelp();
                        break;

                    case "joke":
                        DisplayRandomJoke();
                        break;

                    case "challenge":
                        DisplayRandomChallenge();
                        break;

                    default:
                        // Check if input matches one of our predefined topics
                        if (_chatbotService.IsCyberSecurityTopic(input))
                        {
                            DisplayTips(input);
                        }
                        // Check if it's a general question that has a response in our JSON
                        else if (_chatbotService.HasResponse(input))
                        {
                            string response = _chatbotService.GetResponse(input);
                            TypeWithAnimation(response, 15, 40);
                        }
                        else
                        {
                            TypeWithAnimation($"I didn't quite understand '{input}'. Could you rephrase or type 'help' for available commands?", 15, 40);
                        }
                        break;
                }
            }
        }
        // This is a test change


        private static void DisplayHelp()
        {
            string border = new string('-', 50);
            Console.WriteLine("\n" + border);
            TypeWithAnimation("AVAILABLE COMMANDS:", 15, 30);
            Console.WriteLine(border);
            TypeWithAnimation("• password safety - Get password protection tips", 10, 25);
            TypeWithAnimation("• phishing - Learn about phishing prevention", 10, 25);
            TypeWithAnimation("• safe browsing - Discover internet safety strategies", 10, 25);
            TypeWithAnimation("• joke - Hear a cybersecurity-themed joke", 10, 25);
            TypeWithAnimation("• challenge - Take a cybersecurity mini-challenge", 10, 25);
            TypeWithAnimation("• help - Show this help menu", 10, 25);
            TypeWithAnimation("• exit - Close the bot", 10, 25);
            Console.WriteLine(border);
            TypeWithAnimation("You can also ask me questions like:", 15, 30);
            TypeWithAnimation("• How are you?", 10, 25);
            TypeWithAnimation("• What's your purpose?", 10, 25);
            TypeWithAnimation("• What can I ask you about?", 10, 25);
            Console.WriteLine(border);
        }

        private static void DisplayRandomJoke()
        {
            Console.WriteLine();
            TypeWithAnimation("Cybersecurity Joke Time!", 20, 50);
            Thread.Sleep(500);
            string joke = _chatbotService.GetRandomJoke();
            TypeWithAnimation(joke, 25, 60);
        }

        private static void DisplayRandomChallenge()
        {
            Console.WriteLine();
            TypeWithAnimation("Cybersecurity Challenge!", 20, 50);
            Thread.Sleep(500);
            string challenge = _chatbotService.GetRandomChallenge();
            TypeWithAnimation(challenge, 20, 50);
            TypeWithAnimation("\n(Hint: Think carefully about online safety!)", 15, 40);
        }

        private static void DisplayTips(string topic)
        {
            List<string> tips = _chatbotService.GetTipsForTopic(topic);
            if (tips.Count > 0)
            {
                string emoji = _chatbotService.GetRandomEmoji();
                Console.WriteLine();
                TypeWithAnimation($"{emoji} {topic.ToUpper()} TIPS:", 20, 50);
                foreach (var tip in tips)
                {
                    TypeWithAnimation($"• {tip}", 15, 40);
                    Thread.Sleep(200); // Small pause between tips
                }
            }
            else
            {
                TypeWithAnimation($"Sorry, I couldn't find tips for {topic}.", 15, 40);
            }
        }

        /// <summary>
        /// Creates a typing animation effect when displaying text to the console.
        /// </summary>
        /// <param name="text">The text to display with typing animation</param>
        /// <param name="minDelay">Minimum delay between characters in milliseconds</param>
        /// <param name="maxDelay">Maximum delay between characters in milliseconds</param>
        /// <param name="addNewLine">Whether to add a new line after the text</param>
        private static void TypeWithAnimation(string text, int minDelay = 10, int maxDelay = 50, bool addNewLine = true)
        {
            if (string.IsNullOrEmpty(text)) return;

            foreach (char c in text)
            {
                Console.Write(c);
                // Realistic typing: Random delay between characters
                Thread.Sleep(_random.Next(minDelay, maxDelay));

                // Add a small pause after punctuation for natural reading
                if (c == '.' || c == '!' || c == '?')
                {
                    Thread.Sleep(_random.Next(150, 300));
                }
                else if (c == ',' || c == ';' || c == ':')
                {
                    Thread.Sleep(_random.Next(50, 150));
                }
            }

            if (addNewLine)
            {
                Console.WriteLine(); // New line after the text is complete
            }
        }
    }
}