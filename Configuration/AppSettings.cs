// Create this file in: CybersecurityAwarenessBot/Configuration/AppSettings.cs
using System;

namespace CybersecurityAwarenessBot.Configuration
{
    public class AppSettings
    {
        public int MaxCommandHistory { get; set; } = 20;
        public string DefaultUserName { get; set; } = "Defender";
        public string FallbackLogo { get; set; } = @"
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
";
    }
}
