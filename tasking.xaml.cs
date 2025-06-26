using System;
using System.Windows;
using System.Windows.Controls;

namespace demo2
{
    // This page handles the task management UI (add, complete, delete tasks)
    public partial class tasking : Page
    {
        public tasking()
        {
            InitializeComponent();

            // Bind the ListBox to the static task list
            TaskListBox.ItemsSource = TASKS.TaskList;
        }

        // Add a new task when the Add button is clicked
        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            string name = TaskNameBox.Text.Trim();   // Task name input
            string desc = TaskDescBox.Text.Trim();   // Task description input
            string daysText = TaskDaysBox.Text.Trim(); // Days until reminder (optional)

            // Validate name field
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Please enter a task name.");
                return;
            }

            // Try parse days into a nullable int
            int? days = null;
            if (int.TryParse(daysText, out int parsedDays))
            {
                days = parsedDays;
            }

            // Add task through TASKS manager
            TASKS.AddTask(name, desc, days);
            ChatHistory.AddActivity($"Task '{name}' added.");

            // Clear input fields after adding
            TaskNameBox.Text = "";
            TaskDescBox.Text = "";
            TaskDaysBox.Text = "";
        }

        // Mark a task as completed
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

        // Delete a selected task
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
