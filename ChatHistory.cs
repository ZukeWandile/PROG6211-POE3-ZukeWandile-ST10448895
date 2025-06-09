using System;
using System.Collections.ObjectModel;

namespace demo2
{
    public static class ChatHistory
    {
        public static ObservableCollection<ChatLog> Messages { get; } = new ObservableCollection<ChatLog>();

        public static void Add(string sender, string message)
        {
            Messages.Add(new ChatLog
            {
                Sender = sender,
                Message = message
                // Timestamp is automatically handled by ChatLogEntry
            });
        }
    }
}
