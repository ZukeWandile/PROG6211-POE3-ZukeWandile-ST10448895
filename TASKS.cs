using System;
using System.Collections.ObjectModel;

namespace demo2
{
    public class TaskItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }  // Nullable
        public bool IsCompleted { get; set; }

        public string Status => IsCompleted ? "✅ Done" : "⏳ Pending";

        public override string ToString()
        {
            string due = DueDate.HasValue ? $" (Due: {DueDate:dd MMM yyyy})" : " (No due date)";
            return $"{Name} - {Description}{due} [{Status}]";
        }
    }

    public static class TASKS
    {
        public static ObservableCollection<TaskItem> TaskList { get; } = new ObservableCollection<TaskItem>();

        public static void AddTask(string name, string description, int? daysFromNow = null)
        {
            TaskList.Add(new TaskItem
            {
                Name = name,
                Description = description,
                DueDate = daysFromNow.HasValue ? DateTime.Now.AddDays(daysFromNow.Value) : (DateTime?)null,
                IsCompleted = false
            });
        }

        public static void MarkAsDone(TaskItem task)
        {
            task.IsCompleted = true;
        }

        public static void DeleteTask(TaskItem task)
        {
            TaskList.Remove(task);
        }

        public static void SetReminder(string taskName, int daysFromNow)
        {
            var task = TaskList.FirstOrDefault(t => t.Name.Equals(taskName, StringComparison.OrdinalIgnoreCase));
            if (task != null && !task.DueDate.HasValue)
            {
                task.DueDate = DateTime.Now.AddDays(daysFromNow);
            }
        }

        public static ObservableCollection<TaskItem> GetTasksWithoutReminders()
        {
            return new ObservableCollection<TaskItem>(TaskList.Where(t => !t.DueDate.HasValue));
        }

        public static bool TrySetReminderPrompt(string name)
        {
            return TaskList.Any(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public static bool TrySetReminder(string name, int days)
        {
            var task = TaskList.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (task != null)
            {
                task.DueDate = DateTime.Now.AddDays(days);
                return true;
            }
            return false;
        }


    }
}
