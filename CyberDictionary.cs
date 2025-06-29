﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace demo2
{
    public static class CyberDictionary
    {
        private static string taskStage = null; // Tracks which step we're in during task creation
        private static string pendingTaskName, pendingDesc;
        private static int pendingDays;

        private static readonly Dictionary<string, List<string>> Responses = new();
        private static readonly Dictionary<string, string> LastUsedResponse = new(); // Stores last used response to avoid repetition
        private static readonly Random random = new();

        // Expanded natural language triggers
        private static readonly string[] affirmatives = { "yes", "yeah", "yep", "sure", "okay", "ok", "affirmative", "certainly", "alright" };
        private static readonly string[] negatives = { "no", "nope", "nah", "never", "none", "skip" };

        static CyberDictionary()
        {
            LoadFromFile("Data/Cyber_Security-Info.txt");
        }

        // Try getting a single response directly by key
        public static bool TryGet(string key, out string value)
        {
            key = key.ToLower();
            value = null;

            if (Responses.TryGetValue(key, out var list) && list.Count > 0)
            {
                value = list[0];
                return true;
            }

            return false;
        }

        // Load all topic responses from a file
        public static void LoadFromFile(string path)
        {
            Responses.Clear();
            if (!File.Exists(path)) return;

            foreach (var line in File.ReadAllLines(path))
            {
                if (string.IsNullOrWhiteSpace(line) || !line.Contains("=")) continue;

                var parts = line.Split(new[] { '=' }, 2);
                var key = parts[0].Trim().ToLower();
                var value = parts[1].Trim();

                if (!Responses.ContainsKey(key))
                    Responses[key] = new List<string>();

                Responses[key].Add(value);
            }
        }

        // Return list of base topics like "phishing", "encryption"
        public static List<string> GetKnownTopics()
        {
            return Responses.Keys
                .Where(k => k.Contains("_"))
                .Select(k => k.Split('_')[0])
                .Distinct()
                .ToList();
        }

        // Core response logic
        public static string GetResponse(string input)
        {
            input = input.ToLower().Trim();

            string[] emotions = { "worried", "curious", "frustrated" };
            string[] simplifiedTriggers = { "explain", "simple", "in simple terms" };
            string[] interestTriggers = { "fun", "fact", "interesting" };

            // --- Pattern-based intent recognition ---
            if (Regex.IsMatch(input, @"\b(add|create|make)\s+(a\s+)?task\b"))
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
                // Match negative phrases like "no", "nah", "none"
                if (negatives.Any(n => input.Contains(n)))
                {
                    TASKS.AddTask(pendingTaskName, pendingDesc, null);
                    taskStage = null;
                    return "Got it! Task added without a reminder.";
                }

                // Try parse days
                if (int.TryParse(input, out pendingDays))
                {
                    TASKS.AddTask(pendingTaskName, pendingDesc, pendingDays);
                    taskStage = null;
                    return "Your task has been added successfully!";
                }

                return "Please enter a valid number of days, or type 'no' to skip.";
            }

            // --- Regex: Set reminder ---
            if (Regex.IsMatch(input, @"^set\s+reminder", RegexOptions.IgnoreCase))
            {
                var parts = input.Split("set reminder", StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0)
                    return "Please specify the task name to set a reminder for.";

                var taskName = parts[0].Trim();

                if (TASKS.TrySetReminderPrompt(taskName))
                {
                    taskStage = "reminder:" + taskName;
                    return $"How many days from now should I remind you for \"{taskName}\"?";
                }

                return $"I couldn't find a task named \"{taskName}\".";
            }

            // --- Set reminder stage ---
            if (taskStage != null && taskStage.StartsWith("reminder:"))
            {
                var taskName = taskStage.Split(':')[1];

                if (int.TryParse(input, out int days))
                {
                    if (TASKS.TrySetReminder(taskName, days))
                    {
                        taskStage = null;
                        return $"Reminder set for \"{taskName}\" in {days} day(s).";
                    }

                    taskStage = null;
                    return $"Failed to set reminder for \"{taskName}\".";
                }

                return "Please enter a valid number of days for the reminder.";
            }

            // --- Structured responses ---
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
                        if (key.EndsWith("_" + emotion) && input.Contains(emotion))
                            return GetNonRepeatingRandom(key);
                }
            }

            // --- Fallbacks ---
            foreach (var key in Responses.Keys)
            {
                if (!key.Contains("_") && input.Contains(key))
                    return GetNonRepeatingRandom(key);
            }

            return "Sorry, I don't have information on that topic. Try asking about phishing, MFA, encryption, or passwords!";
        }

        // Pick a random response and avoid repeating the last one
        private static string GetNonRepeatingRandom(string key)
        {
            if (!Responses.TryGetValue(key, out var options) || options.Count == 0)
                return "Sorry, I couldn't find a good response.";

            var lastUsed = LastUsedResponse.TryGetValue(key, out var last) ? last : null;
            var filtered = options.Count > 1 ? options.Where(r => r != lastUsed).ToList() : options;

            var selected = filtered[random.Next(filtered.Count)];
            LastUsedResponse[key] = selected;
            return selected;
        }
    }
}
