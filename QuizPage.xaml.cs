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
        private List<QuizQuestion> questions; // List of quiz questions
        private int currentQuestionIndex;     // Current question number
        private int score;                    // User's score

        public QuizPage()
        {
            InitializeComponent();
            LoadQuestions(); // Load questions when page is created
        }

        // Define all quiz questions
        private void LoadQuestions()
        {
            questions = new List<QuizQuestion>
            {
                new QuizQuestion {
                    QuestionText = "What does MFA stand for?",
                    Options = new() { "Multi-Factor Authentication", "Malicious File Attack", "Mainframe Access" },
                    CorrectAnswer = "Multi-Factor Authentication",
                    Explanation = "MFA adds layers of security by requiring two or more forms of identity verification."
                },
                new QuizQuestion {
                    QuestionText = "What is phishing?",
                    Options = new() { "A firewall tool", "A social engineering attack", "A VPN type" },
                    CorrectAnswer = "A social engineering attack",
                    Explanation = "Phishing is a deceptive attempt to obtain sensitive information by pretending to be a trustworthy source."
                },
                new QuizQuestion {
                    QuestionText = "Which of these is a strong password?",
                    Options = new() { "123456", "Password", "T!g3r$2025!" },
                    CorrectAnswer = "T!g3r$2025!",
                    Explanation = "Strong passwords include a mix of letters, numbers, and symbols."
                },
                new QuizQuestion {
                    QuestionText = "Why should you use a VPN?",
                    Options = new() { "To speed up your connection", "To hide your IP address", "To increase WiFi range" },
                    CorrectAnswer = "To hide your IP address",
                    Explanation = "A VPN hides your real IP address and encrypts your internet traffic for privacy."
                },
                new QuizQuestion {
                    QuestionText = "What does encryption do?",
                    Options = new() { "Deletes data", "Scrambles data", "Duplicates files" },
                    CorrectAnswer = "Scrambles data",
                    Explanation = "Encryption protects data by converting it into unreadable code unless you have the key."
                },
                new QuizQuestion {
                    QuestionText = "Which is a secure Wi-Fi practice?",
                    Options = new() { "Use public Wi-Fi without VPN", "Connect to any open network", "Use WPA2 password protection" },
                    CorrectAnswer = "Use WPA2 password protection",
                    Explanation = "WPA2 encrypts Wi-Fi traffic and makes it harder for attackers to intercept your data."
                },
                new QuizQuestion {
                    QuestionText = "Why update software regularly?",
                    Options = new() { "To remove old features", "To reduce storage", "To patch security flaws" },
                    CorrectAnswer = "To patch security flaws",
                    Explanation = "Updates often include patches that fix known vulnerabilities."
                },
                new QuizQuestion {
                    QuestionText = "What is ransomware?",
                    Options = new() { "A backup tool", "A type of firewall", "Malware that demands payment" },
                    CorrectAnswer = "Malware that demands payment",
                    Explanation = "Ransomware locks your files and demands payment for their release."
                },
                new QuizQuestion {
                    QuestionText = "A strong password means your account is completely secure from hackers.",
                    Options = new() { "TRUE", "FALSE" },
                    CorrectAnswer = "FALSE",
                    Explanation = "Strong passwords help but aren't foolproof—use other protections like MFA."
                },
                new QuizQuestion {
                    QuestionText = "How often should you backup data?",
                    Options = new() { "Never", "Only when system crashes", "Regularly and automatically" },
                    CorrectAnswer = "Regularly and automatically",
                    Explanation = "Frequent backups help protect against data loss from attacks or accidents."
                }
            };
        }

        // When the quiz starts
        private void StartQuiz_Click(object sender, RoutedEventArgs e)
        {
            currentQuestionIndex = 0;
            score = 0;

            // Hide welcome and result panels, show the quiz
            WelcomePanel.Visibility = Visibility.Collapsed;
            ResultPanel.Visibility = Visibility.Collapsed;
            QuizPanel.Visibility = Visibility.Visible;

            LoadQuestion(); // Load first question
        }

        // Load and display the current question
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

            // Get current question and display text/options
            QuizQuestion q = questions[currentQuestionIndex];
            QuestionText.Text = q.QuestionText;
            OptionsGrid.Children.Clear();

            foreach (string option in q.Options)
            {
                var btn = new Button
                {
                    Content = option,
                    Tag = option == q.CorrectAnswer, // Tag stores correctness
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

        // Handle user clicking an answer
        private async void AnswerButton_Click(object sender, RoutedEventArgs e)
        {
            var clicked = sender as Button;
            bool isCorrect = (bool)clicked.Tag;
            var current = questions[currentQuestionIndex];

            // Highlight answers: correct = green, wrong = red
            foreach (Button btn in OptionsGrid.Children)
            {
                btn.IsEnabled = false;
                bool correct = (bool)btn.Tag;
                btn.Background = correct ? Brushes.Green :
                                 btn == clicked ? Brushes.Red :
                                 Brushes.Gray;
            }

            // Show feedback
            FeedbackText.Text = (isCorrect ? " Correct!\n" : " Incorrect.\n") + current.Explanation;
            FeedbackText.Visibility = Visibility.Visible;

            if (isCorrect) score++;

            await Task.Delay(2000); // Pause for 2 seconds

            FeedbackText.Visibility = Visibility.Collapsed;
            currentQuestionIndex++;
            LoadQuestion(); // Load next question
        }
    }
}
