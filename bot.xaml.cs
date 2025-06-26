using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NAudio.Wave;

namespace demo2
{
    public partial class bot : Page
    {
        // File paths for images, audio, and interest topic
        private readonly string botImagePath = "Data/PROGcHATIMAGE.png";
        private readonly string userImagePath = "Data/user-icon.png";
        private readonly string audioPath = "Data/Cyber Voice.m4a";
        private readonly string interestTopicPath = "Data/interest_topic.txt";

        // Audio playback
        private IWavePlayer outputDevice;
        private AudioFileReader audioFile;

        // Chat state flags
        private bool choosing = false;
        private bool waitingDays = false;
        private bool askReminder = false;
        private bool interestAsked = false;
        private bool waitingForInterestTopic = false;

        // Interest tracking
        private string interestTopic = null;
        private int nonInterestInteractions = 0;
        private int totalInteractions = 0;
        private bool interestingFactShown = false;

        // Task name used for reminder
        private string taskName = null;

        private int activityIndex = 0;

        public bot()
        {
            InitializeComponent();

            ShowWelcomeMessage();
            PlayIntroAudio();
            RestorePreviousChat();

            this.Unloaded += Bot_Unloaded;

            this.Loaded += async (s, e) =>
            {
                await Task.Delay(300);
                if (!interestAsked)
                {
                    AskCyberInterest();
                    interestAsked = true;
                }
            };
        }

        // Main method when user sends a message
        private void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            string userMessage = UserInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(userMessage)) return;

            ChatHistory.Add("User", userMessage);
            AddMessageWithIcon(userMessage, Colors.Black, "#EEE", HorizontalAlignment.Right, userImagePath);

            string lowerMsg = userMessage.ToLower();

            // Handle response to Cyber Security interest
            if (waitingForInterestTopic)
            {
                if (lowerMsg == "yes" || lowerMsg == "y")
                {
                    RespondBot("Awesome! What topic are you interested in?");
                    interestTopic = "waiting";
                }
                else if (lowerMsg == "no" || lowerMsg == "n")
                {
                    RespondBot("No worries, I’m still here to help you learn about Cyber Security!");
                }
                else
                {
                    RespondBot("Please answer with 'yes' or 'no'.");
                    return;
                }

                waitingForInterestTopic = false;
                return;
            }

            // User entering interest topic
            if (interestTopic == "waiting")
            {
                var knownTopics = CyberDictionary.GetKnownTopics();
                foreach (string topic in knownTopics)
                {
                    if (lowerMsg.Contains(topic))
                    {
                        interestTopic = topic;
                        RespondBot($"Nice! I find {interestTopic} fascinating too!");
                        return;
                    }
                }

                interestTopic = lowerMsg;
                RespondBot($"Got it! I’ll remember you’re into {interestTopic}.");
                return;
            }

            // Handling task reminder setup
            if (choosing)
            {
                taskName = userMessage;
                choosing = false;
                waitingDays = true;
                RespondBot($"How many days from now should I set the reminder for '{taskName}'?");
                return;
            }

            if (waitingDays)
            {
                if (int.TryParse(userMessage, out int days))
                {
                    TASKS.SetReminder(taskName, days);
                    ChatHistory.AddActivity($"Got it! Reminder set for '{taskName}' in {days} days.");
                    RespondBot($"Got it! Reminder set for '{taskName}' in {days} days.");
                }
                else
                {
                    RespondBot("Oops! Please enter a number (e.g., 3).");
                    return;
                }

                waitingDays = false;
                taskName = null;
                return;
            }

            // Ask user if they want to set reminder
            if (askReminder && taskName != null)
            {
                if (lowerMsg == "yes" || lowerMsg == "y")
                {
                    waitingDays = true;
                    RespondBot($"How many days from now should I set the reminder for '{taskName}'?");
                }
                else
                {
                    RespondBot("No problem! You can add a reminder later with 'set reminder'.");
                }

                askReminder = false;
                return;
            }

            // Handle "set reminder" command
            if (lowerMsg == "set reminder" || lowerMsg == "add reminder")
            {
                var noReminderTasks = TASKS.GetTasksWithoutReminders();
                if (noReminderTasks.Count == 0)
                {
                    RespondBot("All your tasks already have reminders!");
                }
                else
                {
                    string list = string.Join("\n", noReminderTasks.Select(t => $"- {t.Name}"));
                    RespondBot($"These tasks have no reminder:\n{list}\nType the name of the task you want to add a reminder to:");
                    choosing = true;
                }
                return;
            }

            // Show chat log
            if (lowerMsg.Contains("chat log") || lowerMsg.Contains("chat activity"))
            {
                DisplayChatLog();
                return;
            }

            // Show activity log
            if (lowerMsg.Contains("activity log") || lowerMsg.Contains("show activity"))
            {
                DisplayActivityLog();
                return;
            }

            // Scroll activity or chat
            if (lowerMsg == "show more activity")
            {
                ShowNextActivities();
                return;
            }

            if (lowerMsg == "show more chat")
            {
                ShowNextChatMessages();
                return;
            }

            // Main bot response
            string botResponse = CyberDictionary.GetResponse(userMessage);
            RespondBot(botResponse);
            TrackCyberInterest(lowerMsg);

            // Ask to set reminder after task is added
            if (botResponse.StartsWith("Task added: '") && botResponse.Contains("(no due date set)"))
            {
                string extracted = ExtractTaskName(botResponse);
                if (!string.IsNullOrEmpty(extracted))
                {
                    taskName = extracted;
                    askReminder = true;
                    RespondBot($"Would you like to set a reminder for '{taskName}' now? (yes/no)");
                }
            }

            UserInput.Text = string.Empty;
        }

        // Display bot message in chat
        private void RespondBot(string message)
        {
            ChatHistory.Add("Bot", message);
            AddMessageWithIcon(message, Colors.White, "#333", HorizontalAlignment.Left, botImagePath);
        }

        // Track mentions related to interest topic
        private void TrackCyberInterest(string message)
        {
            totalInteractions++;

            if (string.IsNullOrWhiteSpace(interestTopic) || interestTopic == "none" || interestingFactShown)
                return;

            if (!message.Contains(interestTopic))
            {
                nonInterestInteractions++;

                if (nonInterestInteractions >= 3)
                {
                    ShowInterestingFact(interestTopic);
                    interestingFactShown = true;
                    nonInterestInteractions = 0;
                }
            }
            else
            {
                nonInterestInteractions = 0;
            }
        }

        // Extract task name from bot message
        private string ExtractTaskName(string message)
        {
            try
            {
                int start = message.IndexOf("'") + 1;
                int end = message.IndexOf("'", start);
                return message.Substring(start, end - start);
            }
            catch
            {
                return null;
            }
        }

        // Restore previous chat messages to UI
        private void RestorePreviousChat()
        {
            foreach (var entry in ChatHistory.Messages)
            {
                AddMessageWithIcon(
                    entry.Message,
                    entry.Sender == "User" ? Colors.Black : Colors.White,
                    entry.Sender == "User" ? "#EEE" : "#333",
                    entry.Sender == "User" ? HorizontalAlignment.Right : HorizontalAlignment.Left,
                    entry.Sender == "User" ? userImagePath : botImagePath
                );
            }
        }

        // Display recent chat log
        private void DisplayChatLog()
        {
            ChatHistory.ResetChatIndex();
            RespondBot("Here's your recent chat history (up to 5):");
            ShowNextChatMessages();
        }

        // Show more chat log entries
        private void ShowNextChatMessages()
        {
            var nextLogs = ChatHistory.GetNextChatMessages(5);
            if (nextLogs.Count == 0)
            {
                RespondBot("No more chat messages to show.");
                return;
            }

            foreach (var log in nextLogs)
            {
                string icon = log.Sender == "User" ? userImagePath : botImagePath;
                AddMessageWithIcon($"{log.Sender}: {log.Message}", Colors.LightGray, "#222", HorizontalAlignment.Left, icon);
            }

            if (ChatHistory.HasMoreChat)
            {
                RespondBot("Type 'show more chat' to see more.");
            }
        }

        // Display recent activity log
        private void DisplayActivityLog()
        {
            ChatHistory.ResetActivityIndex();
            RespondBot("Here's your activity log (latest 5):");
            ShowNextActivities();
        }

        // Show next batch of activities
        private void ShowNextActivities()
        {
            var entries = ChatHistory.GetNextActivities();
            if (entries.Count == 0)
            {
                RespondBot("No more activity to show.");
                return;
            }

            foreach (var entry in entries)
            {
                AddMessageWithIcon(entry, Colors.LightGray, "#222", HorizontalAlignment.Left, botImagePath);
            }

            if (ChatHistory.HasMoreActivity)
            {
                RespondBot("Type 'show more activity' to see more.");
            }
        }

        // Show interesting fact about interest topic
        private void ShowInterestingFact(string topic)
        {
            if (CyberDictionary.TryGet($"{topic}_INTEREST", out string fact))
            {
                RespondBot($"By the way, here's something cool about {topic}: {fact}");
            }
            else
            {
                RespondBot($"By the way, {topic} is really interesting! I hope to learn more about it with you.");
            }
        }

        // Ask if user has a cybersecurity interest
        private void AskCyberInterest()
        {
            RespondBot("Hey! Do you have a Cyber Security topic you're interested in? (yes/no)");
            waitingForInterestTopic = true;
        }

        // Show welcome ASCII banner
        private void ShowWelcomeMessage()
        {
            string asciiArt = @"__  __        __       __            __                                                    __  __ 
/  |/  |      /  |  _  /  |          /  |                                                  /  |/  |
$$ |$$ |      $$ | / \ $$ |  ______  $$ |  _______   ______   _____  ____    ______        $$ |$$ |
$$ |$$ |      $$ |/$  \$$ | /      \ $$ | /       | /      \ /     \/    \  /      \       $$ |$$ |
$$ |$$ |      $$ /$$$  $$ |/$$$$$$  |$$ |/$$$$$$$/ /$$$$$$  |$$$$$$ $$$$  |/$$$$$$  |      $$ |$$ |
$$/ $$/       $$ $$/$$ $$ |$$    $$ |$$ |$$ |      $$ |  $$ |$$ | $$ | $$ |$$    $$ |      $$/ $$/ 
 __  __       $$$$/  $$$$ |$$$$$$$$/ $$ |$$ \_____ $$ \__$$ |$$ | $$ | $$ |$$$$$$$$/        __  __ 
/  |/  |      $$$/    $$$ |$$       |$$ |$$       |$$    $$/ $$ | $$ | $$ |$$       |      /  |/  |
$$/ $$/       $$/      $$/  $$$$$$$/ $$/  $$$$$$$/  $$$$$$/  $$/  $$/  $$/  $$$$$$$/       $$/ $$/ ";

            ChatMessages.Children.Add(new TextBlock
            {
                Text = asciiArt,
                FontFamily = new FontFamily("Consolas"),
                FontSize = 10,
                Foreground = Brushes.HotPink,
                Background = Brushes.Black,
                Padding = new Thickness(10),
                Margin = new Thickness(5),
                TextWrapping = TextWrapping.NoWrap,
                HorizontalAlignment = HorizontalAlignment.Left,
                MaxWidth = 800
            });
        }

        // Play intro audio when page loads
        private void PlayIntroAudio()
        {
            try
            {
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, audioPath);
                if (!File.Exists(fullPath))
                {
                    MessageBox.Show("Intro audio file not found: " + fullPath);
                    return;
                }

                audioFile = new AudioFileReader(fullPath);
                outputDevice = new WaveOutEvent();
                outputDevice.Init(audioFile);
                outputDevice.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error playing intro audio: " + ex.Message);
            }
        }

        // Add message with image in chat
        private void AddMessageWithIcon(string text, Color textColor, string bgColor, HorizontalAlignment alignment, string imagePath)
        {
            var messagePanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = alignment,
                Margin = new Thickness(5)
            };

            string fullImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imagePath);
            if (File.Exists(fullImagePath))
            {
                messagePanel.Children.Add(new Image
                {
                    Source = new BitmapImage(new Uri(fullImagePath, UriKind.Absolute)),
                    Width = 40,
                    Height = 40,
                    Margin = new Thickness(5)
                });
            }

            messagePanel.Children.Add(new TextBlock
            {
                Text = text,
                Foreground = new SolidColorBrush(textColor),
                Background = (SolidColorBrush)new BrushConverter().ConvertFromString(bgColor),
                FontSize = 16,
                Padding = new Thickness(10),
                Margin = new Thickness(5),
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = 600
            });

            ChatMessages.Children.Add(messagePanel);
        }

        // Dispose audio resources
        private void Bot_Unloaded(object sender, RoutedEventArgs e)
        {
            outputDevice?.Dispose();
            audioFile?.Dispose();
        }
    }
}
