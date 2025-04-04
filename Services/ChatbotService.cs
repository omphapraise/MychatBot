using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace CybersecurityAwarenessBot.Services
{
    /// <summary>
    /// Service that provides cybersecurity awareness chatbot functionality
    /// </summary>
    public class ChatbotService
    {
        private readonly Random _random = new Random();
        private readonly Dictionary<string, List<string>> _cyberTips;
        private readonly Dictionary<string, string> _responses;
        private readonly List<string> _jokes;
        private readonly List<string> _challenges;
        private readonly string[] _emojis;

        /// <summary>
        /// Initializes a new instance of the ChatbotService class
        /// </summary>
        /// <param name="responsesFilePath">Path to the JSON file containing response data</param>
        /// <param name="tipsFilePath">Optional path to the JSON file containing cybersecurity tips</param>
        /// <param name="jokesFilePath">Optional path to the JSON file containing jokes</param>
        /// <param name="challengesFilePath">Optional path to the JSON file containing challenges</param>
        public ChatbotService(string responsesFilePath, string tipsFilePath = null, string jokesFilePath = null, string challengesFilePath = null)
        {
            // Initialize with default values
            _cyberTips = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

            // Original cybersecurity topics
            _cyberTips["password safety"] = new List<string>
            {
                "Create strong passwords: Use 12+ characters with complexity",
                "Mix uppercase, lowercase, numbers & symbols in passwords",
                "Never reuse passwords across different websites",
                "Use a reputable password manager to generate and store passwords",
                "Enable two-factor authentication for critical accounts"
            };

            _cyberTips["phishing"] = new List<string>
            {
                "Be extremely cautious of urgent or threatening messages",
                "Always carefully examine sender email addresses",
                "Hover over links to preview the actual destination before clicking",
                "Never share personal or financial information via email",
                "Independently verify requests through official contact channels"
            };

            _cyberTips["safe browsing"] = new List<string>
            {
                "Always ensure websites use HTTPS before entering sensitive information",
                "Keep browsers, operating systems, and software consistently updated",
                "Avoid conducting sensitive tasks on public or unsecured WiFi networks",
                "Use a reliable VPN when accessing the internet from public networks",
                "Install and regularly update comprehensive antivirus software"
            };

            // New cybersecurity topics
            _cyberTips["data protection"] = new List<string>
            {
                "Regularly backup important data to multiple locations",
                "Use encryption for sensitive files and communications",
                "Securely delete files you no longer need using specialized tools",
                "Be careful when sharing files online and check permission settings",
                "Use secure cloud storage solutions with strong authentication"
            };

            _cyberTips["social media security"] = new List<string>
            {
                "Review privacy settings regularly on all platforms",
                "Be selective about accepting connection requests from unknown individuals",
                "Avoid oversharing personal information that could be used for identity theft",
                "Be cautious about third-party applications requesting access to your accounts",
                "Use unique, strong passwords for each social media platform"
            };

            _cyberTips["mobile device security"] = new List<string>
            {
                "Keep your device and apps updated with the latest security patches",
                "Only download apps from official stores like Google Play or Apple App Store",
                "Review app permissions carefully and limit unnecessary access",
                "Enable remote tracking and wiping features in case your device is lost",
                "Use biometric authentication or strong PIN codes rather than simple patterns"
            };

            _cyberTips["public wifi safety"] = new List<string>
            {
                "Avoid accessing sensitive accounts or performing financial transactions on public WiFi",
                "Use a VPN when connecting to public networks to encrypt your traffic",
                "Verify network names before connecting to avoid evil twin attacks",
                "Turn off automatic WiFi connection to prevent connecting to rogue networks",
                "Disable file sharing when on public networks to prevent unauthorized access"
            };

            _cyberTips["malware prevention"] = new List<string>
            {
                "Keep antivirus and anti-malware software updated and run regular scans",
                "Scan email attachments before opening, even if they appear to be from trusted sources",
                "Be cautious about downloading free software, especially from unofficial sources",
                "Watch for signs of infection such as system slowness or unexpected pop-ups",
                "Use specific protection against ransomware, such as frequent backups and restricted permissions"
            };

            _cyberTips["identity protection"] = new List<string>
            {
                "Monitor your credit reports and financial statements regularly for suspicious activity",
                "Be careful about sharing personal identifiers like SSN, birth date, or address online",
                "Shred sensitive physical documents before disposing of them",
                "Consider using credit freezes or fraud alerts for additional protection",
                "Be alert for signs of identity theft such as unexpected bills or collection notices"
            };

            _cyberTips["remote work security"] = new List<string>
            {
                "Secure your home network with WPA3 encryption and a strong, unique password",
                "Use your company's VPN when accessing work resources remotely",
                "Keep work and personal activities on separate devices when possible",
                "Follow company security policies, even when working from home",
                "Be extra vigilant about phishing attempts targeting remote workers"
            };

            _cyberTips["iot device security"] = new List<string>
            {
                "Change default passwords on all smart devices immediately after setup",
                "Keep firmware and software updated on all connected devices",
                "Segment IoT devices on a separate network from your main home network",
                "Disable unnecessary features, services, and connectivity options",
                "Research security features and update policies before purchasing new smart devices"
            };

            // Override with file data if provided
            if (!string.IsNullOrEmpty(tipsFilePath))
            {
                try
                {
                    var customTips = LoadDataFromJson<Dictionary<string, List<string>>>(tipsFilePath);
                    if (customTips != null)
                    {
                        // Create a new case-insensitive dictionary and add all items
                        var tempDict = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
                        foreach (var kvp in customTips)
                        {
                            tempDict[kvp.Key] = kvp.Value;
                        }
                        _cyberTips = tempDict;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Warning: Failed to load custom tips: {ex.Message}");
                    // Continue with default tips
                }
            }

            // Initialize jokes
            _jokes = new List<string>
            {
                "My friend’s password was ‘incorrect’… now even his laptop roasts him daily!",
                "My antivirus caught a virus… now it needs therapy for trust issues!",
                "Why do cybersecurity experts make great detectives? They're always looking for suspicious activity!",
                "I told my WiFi we need to break up… but it begged me to stay connected!",
                "Why do hackers love dating apps? It’s the easiest way to steal your heart—and your data!"
            };

            // Override with file data if provided
            if (!string.IsNullOrEmpty(jokesFilePath))
            {
                try
                {
                    var customJokes = LoadDataFromJson<List<string>>(jokesFilePath);
                    if (customJokes != null && customJokes.Count > 0)
                    {
                        _jokes = customJokes;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Warning: Failed to load custom jokes: {ex.Message}");
                    // Continue with default jokes
                }
            }

            // Initialize challenges
            _challenges = new List<string>
            {
                "Create a password that's at least 16 characters long!",
                "Spot the potential phishing email in a mock scenario.",
                "Identify three signs of an unsecure website.",
                "List two ways to protect your personal information online.",
                "Explain what two-factor authentication is."
            };

            // Override with file data if provided
            if (!string.IsNullOrEmpty(challengesFilePath))
            {
                try
                {
                    var customChallenges = LoadDataFromJson<List<string>>(challengesFilePath);
                    if (customChallenges != null && customChallenges.Count > 0)
                    {
                        _challenges = customChallenges;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Warning: Failed to load custom challenges: {ex.Message}");
                    // Continue with default challenges
                }
            }

            // Initialize emojis (or emoji-like text representations)
            _emojis = new[]
            {
                "🛡️", "🔒", "⚠️", "💻", "🌐", "🔍", "🛡️", "🚫", "🤖", "🔐"
            };

            // Initialize responses dictionary
            _responses = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // Load responses from JSON file
            try
            {
                var loadedResponses = LoadResponsesFromJson(responsesFilePath);
                if (loadedResponses != null)
                {
                    foreach (var kvp in loadedResponses)
                    {
                        _responses[kvp.Key] = kvp.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: Failed to load responses: {ex.Message}");
                throw; // Re-throw because responses are critical for the chatbot
            }
        }

        // Rest of the class implementation remains unchanged
        private T LoadDataFromJson<T>(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            string jsonContent = File.ReadAllText(filePath);
            T data;

            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                data = JsonSerializer.Deserialize<T>(jsonContent, options);
            }
            catch (JsonException)
            {
                // Try with default options if the above fails
                data = JsonSerializer.Deserialize<T>(jsonContent);
            }

            if (data == null)
            {
                throw new JsonException($"Failed to deserialize data from {filePath}");
            }

            return data;
        }

        private Dictionary<string, string> LoadResponsesFromJson(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Response file not found: {filePath}");
            }

            string jsonContent = File.ReadAllText(filePath);
            Dictionary<string, string> responses;

            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                responses = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent, options);
            }
            catch (JsonException)
            {
                // Try with default options if the above fails
                responses = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent);
            }

            if (responses == null)
            {
                throw new JsonException("Failed to deserialize responses JSON data.");
            }

            return responses;
        }

        public bool IsCyberSecurityTopic(string topic)
        {
            if (string.IsNullOrWhiteSpace(topic))
            {
                return false;
            }

            string normalizedTopic = topic.Trim();
            return _cyberTips.ContainsKey(normalizedTopic);
        }

        public List<string> GetTipsForTopic(string topic)
        {
            if (string.IsNullOrWhiteSpace(topic))
            {
                return new List<string>();
            }

            string normalizedTopic = topic.Trim();
            if (_cyberTips.TryGetValue(normalizedTopic, out List<string> tips))
            {
                return tips;
            }

            return new List<string>(); // Return empty list instead of null
        }

        public bool HasResponse(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return false;
            }

            // Normalize the query (remove extra spaces)
            string normalizedQuery = query.Trim();
            return _responses.ContainsKey(normalizedQuery);
        }

        public string GetResponse(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return "I didn't receive a query. Please ask me something about cybersecurity.";
            }

            string normalizedQuery = query.Trim();

            if (_responses.TryGetValue(normalizedQuery, out string response))
            {
                return response;
            }

            return "I don't have a specific response for that. Try asking about cybersecurity topics like 'password safety', 'phishing', 'safe browsing', 'data protection', 'mobile device security', or 'identity protection'.";
        }

        public string GetRandomJoke()
        {
            if (_jokes.Count == 0)
            {
                return "Sorry, I don't have any jokes available at the moment.";
            }

            return _jokes[_random.Next(_jokes.Count)];
        }

        public string GetRandomChallenge()
        {
            if (_challenges.Count == 0)
            {
                return "Sorry, I don't have any challenges available at the moment.";
            }

            return _challenges[_random.Next(_challenges.Count)];
        }

        public string GetRandomEmoji()
        {
            if (_emojis.Length == 0)
            {
                return "🤔";
            }

            return _emojis[_random.Next(_emojis.Length)];
        }
    }
}