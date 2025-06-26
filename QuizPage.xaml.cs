using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Threading.Tasks;

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
                new QuizQuestion { QuestionText = "What does MFA stand for?", Options = new List<string> { "Multi-Factor Authentication", "Malicious File Attack", "Mainframe Access" }, CorrectAnswer = "Multi-Factor Authentication", Explanation = "MFA adds layers of security by requiring two or more forms of identity verification." },
                new QuizQuestion { QuestionText = "What is phishing?", Options = new List<string> { "A firewall tool", "A social engineering attack", "A VPN type" }, CorrectAnswer = "A social engineering attack", Explanation = "Phishing is a deceptive attempt to obtain sensitive information by pretending to be a trustworthy source." },
                new QuizQuestion { QuestionText = "Which of these is a strong password?", Options = new List<string> { "123456", "Password", "T!g3r$2025!" }, CorrectAnswer = "T!g3r$2025!", Explanation = "Strong passwords include a mix of letters, numbers, and symbols." },
                new QuizQuestion { QuestionText = "Why should you use a VPN?", Options = new List<string> { "To speed up your connection", "To hide your IP address", "To increase WiFi range" }, CorrectAnswer = "To hide your IP address", Explanation = "A VPN hides your real IP address and encrypts your internet traffic for privacy." },
                new QuizQuestion { QuestionText = "What does encryption do?", Options = new List<string> { "Deletes data", "Scrambles data", "Duplicates files" }, CorrectAnswer = "Scrambles data", Explanation = "Encryption protects data by converting it into unreadable code unless you have the key." },
                new QuizQuestion { QuestionText = "Which is a secure Wi-Fi practice?", Options = new List<string> { "Use public Wi-Fi without VPN", "Connect to any open network", "Use WPA2 password protection" }, CorrectAnswer = "Use WPA2 password protection", Explanation = "WPA2 encrypts Wi-Fi traffic and makes it harder for attackers to intercept your data." },
                new QuizQuestion { QuestionText = "Why update software regularly?", Options = new List<string> { "To remove old features", "To reduce storage", "To patch security flaws" }, CorrectAnswer = "To patch security flaws", Explanation = "Updates often include patches that fix known vulnerabilities." },
                new QuizQuestion { QuestionText = "What is ransomware?", Options = new List<string> { "A backup tool", "A type of firewall", "Malware that demands payment" }, CorrectAnswer = "Malware that demands payment", Explanation = "Ransomware locks your files and demands payment for their release." },
                new QuizQuestion { QuestionText = "A strong password means your account is completely secure from hackers.", Options = new List<string> { "TRUE", "FALSE" }, CorrectAnswer = "FALSE", Explanation = "Strong passwords help but aren't foolproof—use other protections like MFA." },
                new QuizQuestion { QuestionText = "How often should you backup data?", Options = new List<string> { "Never", "Only when system crashes", "Regularly and automatically" }, CorrectAnswer = "Regularly and automatically", Explanation = "Frequent backups help protect against data loss from attacks or accidents." }
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
                    Tag = (option == q.CorrectAnswer), // Tag stores whether this button is correct
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

        private async void AnswerButton_Click(object sender, RoutedEventArgs e)
        {
            Button clicked = sender as Button;
            bool isCorrect = (bool)clicked.Tag;
            QuizQuestion current = questions[currentQuestionIndex];

            foreach (Button btn in OptionsGrid.Children)
            {
                btn.IsEnabled = false;

                bool thisCorrect = (bool)btn.Tag;
                btn.Background = thisCorrect ? Brushes.Green :
                                 btn == clicked ? Brushes.Red :
                                 Brushes.Gray;
            }

            if (isCorrect)
            {
                score++;
                FeedbackText.Text = " Correct!\n" + current.Explanation;
            }
            else
            {
                FeedbackText.Text = " Incorrect.\n" + current.Explanation;
            }

            FeedbackText.Visibility = Visibility.Visible;
            await Task.Delay(2000);
            FeedbackText.Visibility = Visibility.Collapsed;

            currentQuestionIndex++;
            LoadQuestion();
        }
    }

    
}
