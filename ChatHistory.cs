using System;
using System.Collections.ObjectModel;

namespace demo2
{
    public static class ChatHistory
    {
        public static ObservableCollection<ChatLog> Messages { get; } = new();
        public static ObservableCollection<string> ActivityEntries { get; } = new();

        private static int activityIndex = 0;
        private static int chatIndex = 0;

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
            ActivityEntries.Insert(0, $"[{DateTime.Now:HH:mm}] {activity}");
        }

        // ACTIVITY pagination
        public static void ResetActivityIndex() => activityIndex = 0;

        public static List<string> GetNextActivities(int count = 5)
        {
            var entries = ActivityEntries.Skip(activityIndex).Take(count).ToList();
            activityIndex += entries.Count;
            return entries;
        }

        public static bool HasMoreActivity => activityIndex < ActivityEntries.Count;

        // CHAT pagination
        public static void ResetChatIndex() => chatIndex = 0;

        public static List<ChatLog> GetNextChatMessages(int count = 5)
        {
            var logs = Messages.Skip(chatIndex).Take(count).ToList();
            chatIndex += logs.Count;
            return logs;
        }

        public static bool HasMoreChat => chatIndex < Messages.Count;
    }


}
