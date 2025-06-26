using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace demo2
{
    public static class ChatHistory
    {
        public static ObservableCollection<ChatLog> Messages { get; } = new(); // Stores chat messages
        public static ObservableCollection<string> ActivityEntries { get; } = new(); // Stores activity logs

        private static int activityIndex = 0; // Index for activity pagination
        private static int chatIndex = 0;     // Index for chat pagination

        // Add a chat message (user or bot)
        public static void Add(string sender, string message)
        {
            Messages.Add(new ChatLog { Sender = sender, Message = message });
        }

        // Add an activity with timestamp
        public static void AddActivity(string activity)
        {
            ActivityEntries.Insert(0, $"[{DateTime.Now:HH:mm}] {activity}");
        }

        // Reset activity pagination index
        public static void ResetActivityIndex() => activityIndex = 0;

        // Get next group of activity entries
        public static List<string> GetNextActivities(int count = 5)
        {
            var entries = ActivityEntries.Skip(activityIndex).Take(count).ToList();
            activityIndex += entries.Count;
            return entries;
        }

        // Check if more activity entries are available
        public static bool HasMoreActivity => activityIndex < ActivityEntries.Count;

        // Reset chat pagination index
        public static void ResetChatIndex() => chatIndex = 0;

        // Get next group of chat messages
        public static List<ChatLog> GetNextChatMessages(int count = 5)
        {
            var logs = Messages.Skip(chatIndex).Take(count).ToList();
            chatIndex += logs.Count;
            return logs;
        }

        // Check if more chat messages are available
        public static bool HasMoreChat => chatIndex < Messages.Count;
    }
}
