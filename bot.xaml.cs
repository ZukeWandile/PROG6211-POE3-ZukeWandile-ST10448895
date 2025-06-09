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
            if (string.IsNullOrEmpty(userMessage)) return;

            ChatHistory.Add("User", userMessage);
            AddMessageWithIcon(userMessage, Colors.Black, "#EEE", HorizontalAlignment.Right, userImagePath);

            if (userMessage.ToLower() == "show log" || userMessage.ToLower() == "show activity log" || userMessage.ToLower() == "what have you done for me")
            {
                string logIntro = "Here’s your activity log:";
                ChatHistory.Add("Bot", logIntro);
                AddMessageWithIcon(logIntro, Colors.White, "#333", HorizontalAlignment.Left, botImagePath);

                var total = ChatHistory.Messages.Count;
                int logCount = Math.Min(Math.Max(3, total), 10); // Ensure at least 3, max 10

                foreach (var entry in ChatHistory.Messages.Skip(Math.Max(0, total - logCount)))
                {
                    string icon = entry.Sender == "User" ? userImagePath : botImagePath;
                    string logLine = $"[{entry.Timestamp}] {entry.Sender}: {entry.Message}";
                    AddMessageWithIcon(logLine, Colors.LightGray, "#222", HorizontalAlignment.Left, icon);
                }


                UserInput.Text = string.Empty;
                return;
            }

            string botResponse = CyberDictionary.GetResponse(userMessage);
            ChatHistory.Add("Bot", botResponse);
            AddMessageWithIcon(botResponse, Colors.White, "#333", HorizontalAlignment.Left, botImagePath);

            UserInput.Text = string.Empty;
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

            TextBlock asciiText = new TextBlock
            {
                Text = asciiArt,
                FontFamily = new FontFamily("Consolas"),
                FontSize = 10,
                Foreground = new SolidColorBrush(Colors.HotPink),
                Background = new SolidColorBrush(Colors.Black),
                Padding = new Thickness(10),
                Margin = new Thickness(5),
                TextWrapping = TextWrapping.NoWrap,
                HorizontalAlignment = HorizontalAlignment.Left,
                MaxWidth = 800
            };

            ChatMessages.Children.Add(asciiText);
        }

        private void PlayIntroAudio()
        {
            try
            {
                string fullAudioPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, audioPath);
                if (File.Exists(fullAudioPath))
                {
                    audioFile = new AudioFileReader(fullAudioPath);
                    outputDevice = new WaveOutEvent();
                    outputDevice.Init(audioFile);
                    outputDevice.Play();
                }
                else
                {
                    MessageBox.Show("Intro audio file not found: " + fullAudioPath);
                }
            }
            catch
            {
                MessageBox.Show("Error playing intro audio.");
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

            // Only try to combine path if imagePath is provided
            if (!string.IsNullOrWhiteSpace(imagePath))
            {
                string fullImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imagePath);
                if (File.Exists(fullImagePath))
                {
                    Image avatar = new Image
                    {
                        Source = new BitmapImage(new Uri(fullImagePath, UriKind.Absolute)),
                        Width = 40,
                        Height = 40,
                        Margin = new Thickness(5)
                    };
                    messagePanel.Children.Add(avatar);
                }
            }

            TextBlock messageText = new TextBlock
            {
                Text = text,
                Foreground = new SolidColorBrush(textColor),
                Background = (SolidColorBrush)new BrushConverter().ConvertFromString(bgColor),
                FontSize = 16,
                Padding = new Thickness(10),
                Margin = new Thickness(5),
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = 600
            };

            messagePanel.Children.Add(messageText);
            ChatMessages.Children.Add(messagePanel);
        }

        private void Bot_Unloaded(object sender, RoutedEventArgs e)
        {
            outputDevice?.Dispose();
            audioFile?.Dispose();
        }
    }
}
