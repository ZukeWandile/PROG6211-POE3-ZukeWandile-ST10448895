using System;
using System.Windows;
using System.Windows.Controls;

namespace demo2
{
    public partial class tasking : Page
    {
        public tasking()
        {
            InitializeComponent();
            TaskListBox.ItemsSource = TASKS.TaskList; // Bind the ListBox to the task list
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            string name = TaskNameBox.Text.Trim();
            string desc = TaskDescBox.Text.Trim();
            string daysText = TaskDaysBox.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Please enter a task name.");
                return;
            }

            int? days = null;
            if (int.TryParse(daysText, out int parsedDays))
            {
                days = parsedDays;
            }

            TASKS.AddTask(name, desc, days);
            ChatHistory.AddActivity($"Task '{name}' added.");

            // Clear input boxes
            TaskNameBox.Text = "";
            TaskDescBox.Text = "";
            TaskDaysBox.Text = "";
        }

        private void MarkDone_Click(object sender, RoutedEventArgs e)
        {
            if (TaskListBox.SelectedItem is TaskItem selectedTask)
            {
                if (!selectedTask.IsCompleted)
                {
                    TASKS.MarkAsDone(selectedTask);
                    ChatHistory.AddActivity($"Task '{selectedTask.Name}' marked as completed.");
                }
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (TaskListBox.SelectedItem is TaskItem selectedTask)
            {
                TASKS.DeleteTask(selectedTask);
                ChatHistory.AddActivity($"Task '{selectedTask.Name}' deleted.");
            }
        }
    }
}
