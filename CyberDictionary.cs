using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace demo2
{
    public static class CyberDictionary
    {
        private static string taskStage = null;
        private static string pendingTaskName, pendingDesc;
        private static int pendingDays;
        private static readonly Dictionary<string, List<string>> Responses = new();
        private static readonly Dictionary<string, string> LastUsedResponse = new(); // To track last responses per key
        private static readonly Random random = new();

        static CyberDictionary()
        {
            LoadFromFile("Data/Cyber_Security-Info.txt");
        }

        public static void LoadFromFile(string path)
        {
            Responses.Clear();
            if (!File.Exists(path)) return;

            foreach (var line in File.ReadAllLines(path))
            {
                if (string.IsNullOrWhiteSpace(line) || !line.Contains("=")) continue;

                var parts = line.Split(new[] { '=' }, 2);
                string key = parts[0].Trim().ToLower();
                string value = parts[1].Trim();

                if (!Responses.ContainsKey(key))
                    Responses[key] = new List<string>();

                Responses[key].Add(value);
            }
        }

        public static string GetResponse(string input)
        {
            input = input.ToLower();

            string[] emotions = { "worried", "curious", "frustrated" };
            string[] simplifiedTriggers = { "explain", "simple", "in simple terms" };
            string[] interestTriggers = { "fun", "fact", "interesting" };

            if (input.Contains("add task"))
            {
                taskStage = "awaiting_name";
                return "Great! What is the name of your task?";
            }
            if (taskStage == "awaiting_name")
            {
                pendingTaskName = input;
                taskStage = "awaiting_desc";
                return "Thanks! Now please give me a short description.";
            }
            if (taskStage == "awaiting_desc")
            {
                pendingDesc = input;
                taskStage = "awaiting_days";
                return "And how many days from now should I remind you?";
            }
            if (taskStage == "awaiting_days")
            {
                if (int.TryParse(input, out pendingDays))
                {
                    TASKS.AddTask(pendingTaskName, pendingDesc, pendingDays);
                    taskStage = null;
                    return "Your task has been added successfully!";
                }
                else
                {
                    return "Please enter a valid number of days.";
                }
            }

            // Check for structured intents first
            foreach (var key in Responses.Keys)
            {
                if (!key.Contains("_")) continue;

                var baseKey = key.Split('_')[0];

                if (input.Contains(baseKey))
                {
                    if (key.EndsWith("_simple") && simplifiedTriggers.Any(t => input.Contains(t)))
                        return GetNonRepeatingRandom(key);

                    if (key.EndsWith("_interest") && interestTriggers.Any(t => input.Contains(t)))
                        return GetNonRepeatingRandom(key);

                    foreach (var emotion in emotions)
                    {
                        if (key.EndsWith("_" + emotion) && input.Contains(emotion))
                            return GetNonRepeatingRandom(key);
                    }
                }
            }

            // General fallback match
            foreach (var key in Responses.Keys)
            {
                if (!key.Contains("_") && input.Contains(key))
                    return GetNonRepeatingRandom(key);
            }

            return "Sorry, I don't have information on that topic. Try asking about phishing, MFA, encryption, or passwords!";
        }

        private static string GetNonRepeatingRandom(string key)
        {
            if (!Responses.ContainsKey(key) || Responses[key].Count == 0)
                return "Sorry, I couldn't find a good response.";

            var options = Responses[key];

            string lastUsed = LastUsedResponse.ContainsKey(key) ? LastUsedResponse[key] : null;

            // Filter out last used, unless there's only one option
            var filtered = options.Count > 1
                ? options.Where(r => r != lastUsed).ToList()
                : options;

            string selected = filtered[random.Next(filtered.Count)];

            LastUsedResponse[key] = selected;

            return selected;
        }
    }
}
