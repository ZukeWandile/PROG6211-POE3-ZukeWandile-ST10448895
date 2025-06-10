using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace demo2
{
    public partial class QuizPage : Page
    {
        private List<QuizQuestion> questions;
        private int currentQuestionIndex;
        private int score;

        public QuizPage()
        {
            InitializeComponent();
            LoadQuestions();
        }

        private void LoadQuestions()
        {
            questions = new List<QuizQuestion>
            {
                new QuizQuestion { QuestionText = "What does MFA stand for?", Options = new List<string> { "Multi-Factor Authentication", "Malicious File Attack", "Mainframe Access" }, CorrectAnswer = "Multi-Factor Authentication" },
                new QuizQuestion { QuestionText = "What is phishing?", Options = new List<string> { "A firewall tool", "A social engineering attack", "A VPN type" }, CorrectAnswer = "A social engineering attack" },
                new QuizQuestion { QuestionText = "Which of these is a strong password?", Options = new List<string> { "123456", "Password", "T!g3r$2025!" }, CorrectAnswer = "T!g3r$2025!" },
                new QuizQuestion { QuestionText = "Why should you use a VPN?", Options = new List<string> { "To speed up your connection", "To hide your IP address", "To increase WiFi range" }, CorrectAnswer = "To hide your IP address" },
                new QuizQuestion { QuestionText = "What does encryption do?", Options = new List<string> { "Deletes data", "Scrambles data", "Duplicates files" }, CorrectAnswer = "Scrambles data" },
                new QuizQuestion { QuestionText = "Which is a secure Wi-Fi practice?", Options = new List<string> { "Use public Wi-Fi without VPN", "Connect to any open network", "Use WPA2 password protection" }, CorrectAnswer = "Use WPA2 password protection" },
                new QuizQuestion { QuestionText = "Why update software regularly?", Options = new List<string> { "To remove old features", "To reduce storage", "To patch security flaws" }, CorrectAnswer = "To patch security flaws" },
                new QuizQuestion { QuestionText = "What is ransomware?", Options = new List<string> { "A backup tool", "A type of firewall", "Malware that demands payment" }, CorrectAnswer = "Malware that demands payment" },
                new QuizQuestion { QuestionText = "What's a digital footprint?", Options = new List<string> { "A physical device", "Your online activity trail", "A password generator" }, CorrectAnswer = "Your online activity trail" },
                new QuizQuestion { QuestionText = "How often should you backup data?", Options = new List<string> { "Never", "Only when system crashes", "Regularly and automatically" }, CorrectAnswer = "Regularly and automatically" }
            };
        }

        private void StartQuiz_Click(object sender, RoutedEventArgs e)
        {
            currentQuestionIndex = 0;
            score = 0;
            WelcomePanel.Visibility = Visibility.Collapsed;
            ResultPanel.Visibility = Visibility.Collapsed;
            QuizPanel.Visibility = Visibility.Visible;
            LoadQuestion();
        }

        private void LoadQuestion()
        {
            if (currentQuestionIndex >= questions.Count)
            {
                QuizPanel.Visibility = Visibility.Collapsed;
                FinalScoreText.Text = $"Quiz Finished!\nYour Score: {score}/{questions.Count}";
                ResultPanel.Visibility = Visibility.Visible;

                ChatHistory.AddActivity($"Completed cybersecurity quiz with score {score}/{questions.Count}");
                return;
            }

            QuizQuestion q = questions[currentQuestionIndex];
            QuestionText.Text = q.QuestionText;
            OptionsGrid.Children.Clear();

            foreach (string option in q.Options)
            {
                Button btn = new Button
                {
                    Content = option,
                    Tag = q.CorrectAnswer,
                    Margin = new Thickness(5),
                    Padding = new Thickness(10),
                    FontSize = 16,
                    Background = Brushes.Gray,
                    Foreground = Brushes.White
                };

                btn.Click += AnswerButton_Click;
                OptionsGrid.Children.Add(btn);
            }

            ScoreText.Text = $"Question {currentQuestionIndex + 1} of {questions.Count}";
        }

        private void AnswerButton_Click(object sender, RoutedEventArgs e)
        {
            Button clicked = sender as Button;
            string selected = clicked.Content.ToString();
            string correct = clicked.Tag.ToString();

            foreach (Button btn in OptionsGrid.Children)
            {
                btn.IsEnabled = false;
                if (btn.Content.ToString() == correct)
                    btn.Background = Brushes.Green;
                else if (btn == clicked)
                    btn.Background = Brushes.Red;
            }

            if (selected == correct)
                score++;

            currentQuestionIndex++;
            Dispatcher.InvokeAsync(async () =>
            {
                await Task.Delay(1200);
                LoadQuestion();
            });
        }
    }
}
