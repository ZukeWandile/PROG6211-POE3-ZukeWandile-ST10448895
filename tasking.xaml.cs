using System;
using System.Windows;
using System.Windows.Controls;
using demo2;


namespace demo2
{
    public partial class tasking : Page
    {
        public tasking()
        {
            InitializeComponent();
            TaskListBox.ItemsSource = TASKS.TaskList; // Bind once — ObservableCollection handles the rest
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(TaskDaysBox.Text, out int days))
            {
                TASKS.AddTask(TaskNameBox.Text, TaskDescBox.Text, days);
                TaskNameBox.Clear();
                TaskDescBox.Clear();
                TaskDaysBox.Clear();
            }
            else
            {
                MessageBox.Show("Please enter a valid number of days.");
            }
        }

        private void MarkDone_Click(object sender, RoutedEventArgs e)
        {
            if (TaskListBox.SelectedItem is TaskItem task)
            {
                TASKS.MarkAsDone(task);
                // No need to refresh the list manually
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (TaskListBox.SelectedItem is TaskItem task)
            {
                TASKS.DeleteTask(task);
            }
        }
    }
}
