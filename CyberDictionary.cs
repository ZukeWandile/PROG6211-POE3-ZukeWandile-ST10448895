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

        public static bool TryGet(string key, out string value)
        {
            value = null;
            key = key.ToLower();

            if (Responses.TryGetValue(key, out var list) && list.Count > 0)
            {
                value = list[0]; // you can change this to random if desired
                return true;
            }

            return false;
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

        public static List<string> GetKnownTopics()
        {
            return Responses.Keys
                .Where(k => k.Contains("_"))
                .Select(k => k.Split('_')[0])
                .Distinct()
                .ToList();
        }


        public static string GetResponse(string input)
        {
            input = input.ToLower().Trim();

            string[] emotions = { "worried", "curious", "frustrated" };
            string[] simplifiedTriggers = { "explain", "simple", "in simple terms" };
            string[] interestTriggers = { "fun", "fact", "interesting" };

            // Task creation flow
            if (input.Contains("add task"))
            {
                taskStage = "T-name";
                return "Great! What is the name of your task?";
            }

            if (taskStage == "T-name")
            {
                pendingTaskName = input;
                taskStage = "T_desc";
                return "Thanks! Now please give me a short description.";
            }

            if (taskStage == "T_desc")
            {
                pendingDesc = input;
                taskStage = "T_days";
                return "And how many days from now should I remind you? (or type 'no' to skip)";
            }

            if (taskStage == "T_days")
            {
                if (input == "no" || input == "none" || input == "skip")
                {
                    TASKS.AddTask(pendingTaskName, pendingDesc, null);
                    taskStage = null;
                    return "Got it! Task added without a reminder.";
                }
                else if (int.TryParse(input, out pendingDays))
                {
                    TASKS.AddTask(pendingTaskName, pendingDesc, pendingDays);
                    taskStage = null;
                    return "Your task has been added successfully!";
                }
                else
                {
                    return "Please enter a valid number of days, or type 'no' to skip.";
                }
            }

            // Set reminder command
            if (input.StartsWith("set reminder"))
            {
                string[] parts = input.Split("set reminder");
                if (parts.Length < 2 || string.IsNullOrWhiteSpace(parts[1]))
                {
                    return "Please specify the task name to set a reminder for.";
                }

                string taskName = parts[1].Trim();

                if (TASKS.TrySetReminderPrompt(taskName))
                {
                    taskStage = "reminder" + taskName;
                    return $"How many days from now should I remind you for \"{taskName}\"?";
                }
                else
                {
                    return $"I couldn't find a task named \"{taskName}\". Please check the name and try again.";
                }
            }

            // waiting for reminder days
            if (taskStage != null && taskStage.StartsWith("reminder"))
            {
                string taskName = taskStage.Split(':')[1];

                if (int.TryParse(input, out int days))
                {
                    if (TASKS.TrySetReminder(taskName, days))
                    {
                        taskStage = null;
                        return $"Reminder set for \"{taskName}\" in {days} day(s).";
                    }
                    else
                    {
                        taskStage = null;
                        return $"Failed to set reminder for \"{taskName}\".";
                    }
                }
                else
                {
                    return "Please enter a valid number of days for the reminder.";
                }
            }

            // Structured responses
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

            // General fallback
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
