using IroNice19.TaskPlanner.Domain.Logic;
using IroNice19.TaskPlanner.Domain.Models.Enums;
using IroNice19.TaskPlanner.Domain.Models;

internal static class Program
{
    public static void Main(string[] args)
    {
        List<WorkItem> workItems = new List<WorkItem>();

        Console.WriteLine("Enter tasks (type 'done' to finish). Enter one field at a time when prompted:");
        while (true)
        {
            Console.WriteLine("\nEnter Title (or 'done' to finish):");
            string title = Console.ReadLine()?.Trim();
            if (title?.ToLower() == "done") break;

            Console.WriteLine("Enter DueDate (dd.MM.yyyy, e.g., 21.10.2025):");
            string dueDateInput = Console.ReadLine()?.Trim();
            if (!DateTime.TryParseExact(dueDateInput, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dueDate))
            {
                Console.WriteLine("Invalid date format. Use dd.MM.yyyy (e.g., 21.10.2025). Try again.");
                continue;
            }

            Console.WriteLine("Enter Priority (None/Low/Medium/High/Urgent):");
            string priorityInput = Console.ReadLine()?.Trim();
            if (!Enum.TryParse<Priority>(priorityInput, true, out Priority priority))
            {
                Console.WriteLine("Invalid Priority. Use None/Low/Medium/High/Urgent. Try again.");
                continue;
            }

            Console.WriteLine("Enter Complexity (None/Minutes/Hours/Days/Weeks):");
            string complexityInput = Console.ReadLine()?.Trim();
            if (!Enum.TryParse<Complexity>(complexityInput, true, out Complexity complexity))
            {
                Console.WriteLine("Invalid Complexity. Use None/Minutes/Hours/Days/Weeks. Try again.");
                continue;
            }

            Console.WriteLine("Enter Description:");
            string description = Console.ReadLine()?.Trim();

            Console.WriteLine("Enter IsCompleted (true/false):");
            if (!bool.TryParse(Console.ReadLine()?.Trim(), out bool isCompleted))
            {
                Console.WriteLine("Invalid boolean value. Use true/false. Try again.");
                continue;
            }

            workItems.Add(new WorkItem
            {
                Title = title,
                DueDate = dueDate,
                Priority = priority,
                Complexity = complexity,
                Description = description,
                IsCompleted = isCompleted,
                CreationDate = DateTime.Now 
            });
        }

       
        SimpleTaskPlanner planner = new SimpleTaskPlanner();
        WorkItem[] sortedItems = planner.CreatePlan(workItems.ToArray());

        Console.WriteLine("\nSorted Tasks:");
        int taskNumber = 1;
        foreach (var item in sortedItems)
        {
            Console.WriteLine($"Task {taskNumber}: {item.ToString()}");
            Console.WriteLine($"Complexity: {item.Complexity}, Description: {item.Description}, IsCompleted: {item.IsCompleted}");
            Console.WriteLine($"Creation Date: {item.CreationDate:dd.MM.yyyy HH:mm}");
            Console.WriteLine();
            taskNumber++;
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}