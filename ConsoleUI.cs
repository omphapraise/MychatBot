using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CybersecurityAwarenessBot.Configuration;
using Microsoft.Extensions.Logging;

namespace CybersecurityAwarenessBot.UI
{
    public interface IConsoleUI
    {
        Task<string> GetUserInputAsync(string userName);
        Task TypeWithAnimationAsync(string text, int minDelay = 10, int maxDelay = 50, ConsoleColor? color = null, bool addNewLine = true);
        Task DisplayAsciiLogoAsync(string logoPath);
        Task<string> PromptForUserNameAsync();
        Task DisplayPersonalizedWelcomeAsync(string userName, IEnumerable<string> availableTopics);
        Task DisplayHelpAsync(IEnumerable<string> topics, IEnumerable<string> commands);
    }

    public class ConsoleUI : IConsoleUI
    {
        private readonly Random _random = new Random();
        private readonly ILogger<ConsoleUI> _logger;
        private readonly List<string> _commandHistory = new List<string>();
        private int _historyIndex = -1;
        private readonly AppSettings _appSettings;

        public ConsoleUI(ILogger<ConsoleUI> logger, AppSettings appSettings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        }

        public async Task<string> GetUserInputAsync(string userName)
        {
            Console.Write($"\n{userName}> ");

            // Variables for command history
            string input = string.Empty;
            int cursorLeft = Console.CursorLeft;
            int cursorTop = Console.CursorTop;
            int maxInputLength = Console.WindowWidth - cursorLeft - 2; // Prevent writing beyond screen width

            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                // Handle special keys
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine(); // Move to next line

                    // Add to history if not empty and not a duplicate of the most recent command
                    if (!string.IsNullOrWhiteSpace(input) &&
                        (_commandHistory.Count == 0 || _commandHistory[0] != input))
                    {
                        _commandHistory.Insert(0, input);
                        if (_commandHistory.Count > _appSettings.MaxCommandHistory)
                        {
                            _commandHistory.RemoveAt(_commandHistory.Count - 1);
                        }
                    }

                    _historyIndex = -1;
                    return input;
                }
                else if (keyInfo.Key == ConsoleKey.Backspace && input.Length > 0)
                {
                    input = input.Substring(0, input.Length - 1);

                    // Clear line safely
                    try
                    {
                        Console.SetCursorPosition(cursorLeft, cursorTop);
                        Console.Write(input + new string(' ', maxInputLength - input.Length));
                        Console.SetCursorPosition(cursorLeft + input.Length, cursorTop);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        // Handle potential console buffer issues
                        Console.Write("\b \b"); // Simple backspace if positioning fails
                    }
                }
                else if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    // Navigate command history upward
                    if (_commandHistory.Count > 0 && _historyIndex < _commandHistory.Count - 1)
                    {
                        _historyIndex++;
                        input = _commandHistory[_historyIndex];

                        try
                        {
                            Console.SetCursorPosition(cursorLeft, cursorTop);
                            Console.Write(input + new string(' ', maxInputLength - input.Length));
                            Console.SetCursorPosition(cursorLeft + input.Length, cursorTop);
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            // Fallback if positioning fails
                            Console.WriteLine($"\n{userName}> {input}");
                            cursorTop = Console.CursorTop;
                            cursorLeft = userName.Length + 2;
                        }
                    }
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    // Navigate command history downward
                    if (_historyIndex > 0)
                    {
                        _historyIndex--;
                        input = _commandHistory[_historyIndex];
                    }
                    else if (_historyIndex == 0)
                    {
                        _historyIndex = -1;
                        input = string.Empty;
                    }

                    try
                    {
                        Console.SetCursorPosition(cursorLeft, cursorTop);
                        Console.Write(input + new string(' ', maxInputLength - input.Length));
                        Console.SetCursorPosition(cursorLeft + input.Length, cursorTop);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        // Fallback if positioning fails
                        Console.WriteLine($"\n{userName}> {input}");
                        cursorTop = Console.CursorTop;
                        cursorLeft = userName.Length + 2;
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Tab)
                {
                    // Simple tab completion implementation could be added here
                    await Task.CompletedTask; // Keep compiler happy with async method
                }
                else if (!char.IsControl(keyInfo.KeyChar) && input.Length < maxInputLength)
                {
                    // Add regular character (with bounds checking)
                    input += keyInfo.KeyChar;
                    Console.Write(keyInfo.KeyChar);
                }
            }
        }

        public async Task TypeWithAnimationAsync(string text, int minDelay = 10, int maxDelay = 50, ConsoleColor? color = null, bool addNewLine = true)
        {
            if (string.IsNullOrEmpty(text)) return;

            ConsoleColor originalColor = Console.ForegroundColor;

            if (color.HasValue)
            {
                Console.ForegroundColor = color.Value;
            }

            try
            {
                foreach (char c in text)
                {
                    Console.Write(c);
                    // Realistic typing: Random delay between characters
                    await Task.Delay(_random.Next(minDelay, maxDelay));

                    // Add a small pause after punctuation for natural reading
                    if (c == '.' || c == '!' || c == '?')
                    {
                        await Task.Delay(_random.Next(150, 300));
                    }
                    else if (c == ',' || c == ';' || c == ':')
                    {
                        await Task.Delay(_random.Next(50, 150));
                    }
                }

                if (addNewLine)
                {
                    Console.WriteLine(); // New line after the text is complete
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during animated typing");
            }
            finally
            {
                // Restore original color
                Console.ForegroundColor = originalColor;
            }
        }

        public async Task DisplayAsciiLogoAsync(string logoPath)
        {
            try
            {
                if (!string.IsNullOrEmpty(logoPath) && File.Exists(logoPath))
                {
                    string asciiLogo = await File.ReadAllTextAsync(logoPath);
                    Console.WriteLine(asciiLogo);
                }
                else
                {
                    _logger.LogWarning("ASCII logo file not found: {LogoPath}", logoPath);
                    // Fallback ASCII art
                    Console.WriteLine(_appSettings.FallbackLogo);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error displaying ASCII logo");
                await TypeWithAnimationAsync($"Error displaying ASCII logo: {ex.Message}", 5, 15, ConsoleColor.Red);
            }
        }

        public async Task<string> PromptForUserNameAsync()
        {
            string userName = string.Empty;
            int attempts = 0;
            const int maxAttempts = 3;

            while (string.IsNullOrWhiteSpace(userName) && attempts < maxAttempts)
            {
                attempts++;
                await TypeWithAnimationAsync("What's your name, cyber defender? ", 15, 40, null, false);

                try
                {
                    userName = Console.ReadLine()?.Trim() ?? string.Empty;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error reading username input");
                    userName = string.Empty;
                }

                if (string.IsNullOrWhiteSpace(userName) && attempts < maxAttempts)
                {
                    await TypeWithAnimationAsync("I didn't catch that. Let's try again.", 15, 40, ConsoleColor.Yellow);
                }
            }

            // Provide a default name if user doesn't input one after max attempts
            if (string.IsNullOrWhiteSpace(userName))
            {
                await TypeWithAnimationAsync($"I'll call you {_appSettings.DefaultUserName}.", 15, 40, ConsoleColor.Yellow);
                userName = _appSettings.DefaultUserName;
            }

            return userName;
        }

        public async Task DisplayPersonalizedWelcomeAsync(string userName, IEnumerable<string> availableTopics)
        {
            if (availableTopics == null)
            {
                availableTopics = new List<string>();
                _logger.LogWarning("No topics provided for welcome message");
            }

            string border = new string('=', 60);
            Console.WriteLine(border);
            await TypeWithAnimationAsync($"Welcome to Cybersecurity Awareness, {userName}!", 20, 50, ConsoleColor.Green);
            await TypeWithAnimationAsync("I'm here to help you stay safe in the digital world.", 20, 50);
            Console.WriteLine(border);

            Console.WriteLine();
            await TypeWithAnimationAsync("Available topics:", 15, 40, ConsoleColor.Cyan);
            await Task.Delay(300);

            // Display available topics
            foreach (var topic in availableTopics)
            {
                await TypeWithAnimationAsync($"• {topic} - Get tips on {topic}", 15, 30);
            }

            Console.WriteLine();
            await TypeWithAnimationAsync("Type 'joke' for a humor break!", 15, 40);
            await TypeWithAnimationAsync("Type 'challenge' for a cybersecurity mini-challenge!", 15, 40);
            await TypeWithAnimationAsync("Type 'help' to see available commands", 15, 40);
            await TypeWithAnimationAsync("Type 'exit' to quit", 15, 40);
            Console.WriteLine();
        }

        public async Task DisplayHelpAsync(IEnumerable<string> topics, IEnumerable<string> commands)
        {
            if (topics == null) topics = new List<string>();
            if (commands == null) commands = new List<string>();

            string border = new string('-', 50);
            Console.WriteLine("\n" + border);
            await TypeWithAnimationAsync("AVAILABLE COMMANDS:", 15, 30, ConsoleColor.Cyan);
            Console.WriteLine(border);

            // Display available topics
            foreach (var topic in topics)
            {
                await TypeWithAnimationAsync($"• {topic} - Get tips on {topic}", 10, 25);
            }

            // Display available commands
            foreach (var command in commands)
            {
                string description = command switch
                {
                    "joke" => "Hear a cybersecurity-themed joke",
                    "challenge" => "Take a cybersecurity mini-challenge",
                    "help" => "Show this help menu",
                    "exit" => "Close the bot",
                    _ => "Command"
                };

                await TypeWithAnimationAsync($"• {command} - {description}", 10, 25);
            }

            Console.WriteLine(border);
            await TypeWithAnimationAsync("You can also ask me questions like:", 15, 30, ConsoleColor.Cyan);
            await TypeWithAnimationAsync("• How are you?", 10, 25);
            await TypeWithAnimationAsync("• What's your purpose?", 10, 25);
            await TypeWithAnimationAsync("• What can I ask you about?", 10, 25);
            Console.WriteLine(border);
        }
    }
}