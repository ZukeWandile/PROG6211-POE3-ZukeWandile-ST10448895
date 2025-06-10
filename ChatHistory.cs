using System;
using System.Collections.ObjectModel;

namespace demo2
{
    public static class ChatHistory
    {
        public static ObservableCollection<ChatLog> Messages { get; } = new();
        public static ObservableCollection<string> ActivityEntries { get; } = new();

        public static void Add(string sender, string message)
        {
            Messages.Add(new ChatLog
            {
                Sender = sender,
                Message = message
            });
        }

        public static void AddActivity(string activity)
        {
            ActivityEntries.Add($"[{DateTime.Now:HH:mm}] {activity}");
        }
    }
}
