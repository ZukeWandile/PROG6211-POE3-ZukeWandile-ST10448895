using System;
using System.Collections.ObjectModel;
using demo2;


namespace demo2
{
    public class TaskItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }

        public string Status => IsCompleted ? "✅ Done" : "⏳ Pending";

        public override string ToString()
        {
            return $"{Name} - {Description} (Due: {DueDate:dd MMM yyyy}) [{Status}]";
        }
    }

    public static class TASKS
    {
        public static ObservableCollection<TaskItem> TaskList { get; } = new ObservableCollection<TaskItem>();

        public static void AddTask(string name, string description, int daysFromNow)
        {
            TaskList.Add(new TaskItem
            {
                Name = name,
                Description = description,
                DueDate = DateTime.Now.AddDays(daysFromNow),
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
    }
}
