using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NAudio.Wave;

namespace demo2
{
    public partial class bot : Page
    {
        private readonly string botImagePath = "Data/PROGcHATIMAGE.png";
        private readonly string userImagePath = "Data/user-icon.png";
        private readonly string audioPath = "Data/Cyber Voice.m4a";

        private IWavePlayer outputDevice;
        private AudioFileReader audioFile;

        public bot()
        {
            InitializeComponent();
            ShowWelcomeMessage();
            PlayIntroAudio();
            RestorePreviousChat();
            this.Unloaded += Bot_Unloaded;
        }

        private void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            string userMessage = UserInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(userMessage)) return;

            ChatHistory.Add("User", userMessage);
            AddMessageWithIcon(userMessage, Colors.Black, "#EEE", HorizontalAlignment.Right, userImagePath);

            string lowerMsg = userMessage.ToLower();

            if (lowerMsg.Contains("chat log") || lowerMsg.Contains("chat activity"))
            {
                DisplayChatLog();
            }
            else if (lowerMsg.Contains("activity log") || lowerMsg.Contains("show activity"))
            {
                DisplayActivityLog();
            }
            else
            {
                string botResponse = CyberDictionary.GetResponse(userMessage);
                ChatHistory.Add("Bot", botResponse);
                AddMessageWithIcon(botResponse, Colors.White, "#333", HorizontalAlignment.Left, botImagePath);
            }

            UserInput.Text = string.Empty;
        }

        private void DisplayChatLog()
        {
            string intro = "Here's your recent chat log:";
            ChatHistory.Add("Bot", intro);
            AddMessageWithIcon(intro, Colors.White, "#333", HorizontalAlignment.Left, botImagePath);

            foreach (var log in ChatHistory.Messages)
            {
                string icon = log.Sender == "User" ? userImagePath : botImagePath;
                AddMessageWithIcon($"{log.Sender}: {log.Message}", Colors.LightGray, "#222", HorizontalAlignment.Left, icon);
            }
        }

        private void DisplayActivityLog()
        {
            string intro = "Here's your activity log:";
            ChatHistory.Add("Bot", intro);
            AddMessageWithIcon(intro, Colors.White, "#333", HorizontalAlignment.Left, botImagePath);

            foreach (var entry in ChatHistory.ActivityEntries)
            {
                AddMessageWithIcon(entry, Colors.LightGray, "#222", HorizontalAlignment.Left, botImagePath);
            }
        }

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

        private void ShowWelcomeMessage()
        {
            string asciiArt = @"
 __  __        __       __            __                                                    __  __ 
/  |/  |      /  |  _  /  |          /  |                                                  /  |/  |
$$ |$$ |      $$ | / \ $$ |  ______  $$ |  _______   ______   _____  ____    ______        $$ |$$ |
$$ |$$ |      $$ |/$  \$$ | /      \ $$ | /       | /      \ /     \/    \  /      \       $$ |$$ |
$$ |$$ |      $$ /$$$  $$ |/$$$$$$  |$$ |/$$$$$$$/ /$$$$$$  |$$$$$$ $$$$  |/$$$$$$  |      $$ |$$ |
$$/ $$/       $$ $$/$$ $$ |$$    $$ |$$ |$$ |      $$ |  $$ |$$ | $$ | $$ |$$    $$ |      $$/ $$/ 
 __  __       $$$$/  $$$$ |$$$$$$$$/ $$ |$$ \_____ $$ \__$$ |$$ | $$ | $$ |$$$$$$$$/        __  __ 
/  |/  |      $$$/    $$$ |$$       |$$ |$$       |$$    $$/ $$ | $$ | $$ |$$       |      /  |/  |
$$/ $$/       $$/      $$/  $$$$$$$/ $$/  $$$$$$$/  $$$$$$/  $$/  $$/  $$/  $$$$$$$/       $$/ $$/ 
";

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

        private void PlayIntroAudio()
        {
            try
            {
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, audioPath);
                if (File.Exists(fullPath))
                {
                    audioFile = new AudioFileReader(fullPath);
                    outputDevice = new WaveOutEvent();
                    outputDevice.Init(audioFile);
                    outputDevice.Play();
                }
                else
                {
                    MessageBox.Show("Intro audio file not found: " + fullPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error playing intro audio: " + ex.Message);
            }
        }

        private void AddMessageWithIcon(string text, Color textColor, string bgColor, HorizontalAlignment alignment, string imagePath)
        {
            StackPanel messagePanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = alignment,
                Margin = new Thickness(5)
            };

            if (!string.IsNullOrWhiteSpace(imagePath))
            {
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

        private void Bot_Unloaded(object sender, RoutedEventArgs e)
        {
            outputDevice?.Dispose();
            audioFile?.Dispose();
        }
    }
}
