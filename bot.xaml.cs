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
        // File paths for bot image, user image, intro audio, and interest topic storage
        private readonly string botImagePath = "Data/PROGcHATIMAGE.png";
        private readonly string userImagePath = "Data/user-icon.png";
        private readonly string audioPath = "Data/Cyber Voice.m4a";
        private readonly string interestTopicPath = "Data/interest_topic.txt";

        // Audio player variables
        private IWavePlayer outputDevice;
        private AudioFileReader audioFile;

        // State flags
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


        // Task reminder tracking
        private string taskName = null;

        private int activityIndex = 0; // Keeps track of how many entries shown


        public bot()
        {
            InitializeComponent();

            ShowWelcomeMessage();
            PlayIntroAudio();
            RestorePreviousChat();

            // Read and validate saved interest topic
           

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



        // Handle user message input
        private void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            string userMessage = UserInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(userMessage)) return;

            ChatHistory.Add("User", userMessage);
            AddMessageWithIcon(userMessage, Colors.Black, "#EEE", HorizontalAlignment.Right, userImagePath);

            string lowerMsg = userMessage.ToLower();

            
            // Ask for Cyber Security interest (memory-based only)
            if (waitingForInterestTopic)
            {
                if (lowerMsg == "yes" || lowerMsg == "y")
                {
                    RespondBot("Awesome! What topic are you interested in?");
                    interestTopic = "waiting"; // waiting for the topic itself
                    waitingForInterestTopic = false;
                    return;
                }
                else if (lowerMsg == "no" || lowerMsg == "n")
                {
                    RespondBot("No worries, I’m still here to help you learn about Cyber Security!");
                    waitingForInterestTopic = false;
                    return;
                }
                else
                {
                    RespondBot("Please answer with 'yes' or 'no'.");
                    return;
                }
            }

            // User is specifying interest topic
            if (interestTopic == "waiting")
            {
                var knownTopics = CyberDictionary.GetKnownTopics();
                foreach (string topic in knownTopics)
                {
                    if (lowerMsg.Contains(topic))
                    {
                        interestTopic = topic;
                        RespondBot($"Nice! I find {interestTopic} fascinating too!");
                        waitingForInterestTopic = false;
                        return;
                    }
                }

                interestTopic = lowerMsg.Trim();
                RespondBot($"Got it! I’ll remember you’re into {interestTopic}.");
                waitingForInterestTopic = false;
                return;
            }


            // Handle reminder setup
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

            // Handle set reminder command
            if (lowerMsg == "set reminder"|| lowerMsg =="add reminder")
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

            // Chat/activity logs
            if (lowerMsg.Contains("chat log") || lowerMsg.Contains("chat activity"))
            {
                DisplayChatLog();
                return;
            }

            if (lowerMsg.Contains("activity log") || lowerMsg.Contains("show activity"))
            {
                DisplayActivityLog();
                return;
            }

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


            // Main chatbot response
            string botResponse = CyberDictionary.GetResponse(userMessage);
            RespondBot(botResponse);
            TrackCyberInterest(userMessage.ToLower());

            // Handle optional reminder offer
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


        // Sends message from bot with styling
        private void RespondBot(string message)
        {
            ChatHistory.Add("Bot", message);
            AddMessageWithIcon(message, Colors.White, "#333", HorizontalAlignment.Left, botImagePath);
        }

        // Tracks interest topic mentions
        private void TrackCyberInterest(string message)
        {
            totalInteractions++;

            if (interestTopic == null || interestTopic == "none" || interestingFactShown)
                return;

            if (!message.Contains(interestTopic))
            {
                nonInterestInteractions++;

                if (nonInterestInteractions >= 3)
                {
                    ShowInterestingFact(interestTopic);
                    interestingFactShown = true; // 👈 show only once
                    nonInterestInteractions = 0;
                }
            }
            else
            {
                nonInterestInteractions = 0;
            }
        }

        // Extracts task name from bot response
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

        // Display previous chat messages
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

        // Display chat log
        private void DisplayChatLog()
        {
            ChatHistory.ResetChatIndex(); // Start from the beginning
            RespondBot("Here's your recent chat history (up to 5):");
            ShowNextChatMessages();
        }

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


        // Display activity log
        private void DisplayActivityLog()
        {
            ChatHistory.ResetActivityIndex(); // Reset when "activity log" is first typed
            RespondBot("Here's your activity log (latest 5):");
            ShowNextActivities();
        }

        private void ShowNextActivities()
        {
            var entries = ChatHistory.GetNextActivities(); // Only call once!
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




        // Show a fun fact after 3 unrelated messages
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

        // Ask user if they have an interest in Cyber Security
        private void AskCyberInterest()
        {
            RespondBot("Hey! Do you have a Cyber Security topic you're interested in? (yes/no)");
            waitingForInterestTopic = true;
        }



        // Intro message with ASCII art
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

        // Play intro audio file
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

        // Add message with image icon
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

        // Dispose audio resources when leaving
        private void Bot_Unloaded(object sender, RoutedEventArgs e)
        {
            outputDevice?.Dispose();
            audioFile?.Dispose();
        }
    }
}
