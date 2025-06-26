# 🛡️ Cyber Security Assistant

A modern WPF desktop application designed to help users learn, stay secure, and manage personal cybersecurity goals. The app provides an interactive chatbot, a quiz module, and a task planner — all aimed at boosting cyber awareness.

---

## 🚀 Features

### 🤖 Cybersecurity Chatbot
- Friendly conversation style with clear explanations
- Covers key topics like phishing, MFA, password safety, and more
- Handles curiosity, confusion, and emotional reactions (e.g., worried, frustrated)
- Offers simplified answers, interesting facts, and helpful tips

### 📝 Task Manager
- Add, delete, and mark tasks as done
- Set reminders based on days (e.g., "remind me in 3 days")
- Visual list of all current security goals

### 🧠 Interactive Quiz
- Engaging multiple-choice questions
- Instant visual feedback: green for correct, red for incorrect
- Score tracker to encourage learning

### 🏠 Home Dashboard
- Clean and inviting welcome screen
- Overview of app functionality
- Clear call-to-action to explore features via sidebar

---


## 🛠️ Tech Stack

- **.NET 6+ / .NET Core**
- **WPF (Windows Presentation Foundation)**
- **C#**
- **XAML for UI Design**

---

## 📂 Folder Structure
demo2/
├── Data/
│ ├── Cyber_Security-Info.txt # Dictionary of cybersecurity topics & emotional responses
│ ├── Cyber Voice.m4a # Optional voice/audio clip
│ ├── security_icon.png # Icon used on welcome page
│ └── PROGCHATIMAGE.png, user-icon # Visuals for UI
├── bot.xaml / bot.xaml.cs # Chatbot UI and logic
├── ChatLog.cs # Maintains last 3–10 chat actions
├── ChatHistory.cs # Manages saved chat transcripts
├── CyberDictionary.cs # Loads structured dictionary from text
├── HomePage.xaml / HomePage.xaml.cs # Full-image welcome screen with guidance
├── MainWindow.xaml / MainWindow.xaml.cs # Sidebar layout with page navigation
├── QuizPage.xaml / QuizPage.xaml.cs # 10-question multiple-choice quiz
├── QuizQuestion.cs # Data model for quiz questions
├── tasking.xaml / tasking.xaml.cs # UI for adding, editing, and completing tasks
├── TASKS.cs # Data model for cybersecurity tasks
├── App.xaml / AssemblyInfo.cs # Application bootstrap and metadata
└── README.md # Project documentation
