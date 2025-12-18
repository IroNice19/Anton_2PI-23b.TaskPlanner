using IroNice19.TaskPlanner.DataAccess;
using IroNice19.TaskPlanner.DataAccess.Abstractions;
using IroNice19.TaskPlanner.Domain.Logic;
using IroNice19.TaskPlanner.Domain.Models;
using IroNice19.TaskPlanner.Domain.Models.Enums;
using System;

namespace IroNice19.TaskPlanner.ConsoleRunner
{
    internal static class Program
    {
        private static IWorkItemsRepository _repository = new FileWorkItemsRepository();
        private static SimpleTaskPlanner _planner = new SimpleTaskPlanner(_repository);

        public static void Main(string[] args)
        {
            Console.WriteLine("=== Task Planner ===");

            while (true)
            {
                Console.WriteLine("\nОберіть дію:");
                Console.WriteLine("[A]dd work item");
                Console.WriteLine("[B]uild a plan");
                Console.WriteLine("[M]ark work item as completed");
                Console.WriteLine("[R]emove a work item");
                Console.WriteLine("[Q]uit the app");

                var key = Console.ReadKey(true).KeyChar.ToString().ToUpper();

                switch (key)
                {
                    case "A":
                        AddWorkItem();
                        break;
                    case "B":
                        BuildPlan();
                        break;
                    case "M":
                        MarkAsCompleted();
                        break;
                    case "R":
                        RemoveWorkItem();
                        break;
                    case "Q":
                        _repository.SaveChanges();
                        Console.WriteLine("Дані збережено. До побачення!");
                        return;
                    default:
                        Console.WriteLine("Невірна команда.");
                        break;
                }
            }
        }

        private static void AddWorkItem()
        {
            Console.Write("Title: ");
            var title = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(title)) { Console.WriteLine("Title обов'язковий."); return; }

            Console.Write("Description (можна пропустити): ");
            var description = Console.ReadLine()?.Trim() ?? "";

            Console.Write("Due date (dd.MM.yyyy, або Enter для сьогодні): ");
            var dueInput = Console.ReadLine();
            DateTime dueDate = string.IsNullOrEmpty(dueInput)
                ? DateTime.Today
                : DateTime.ParseExact(dueInput, "dd.MM.yyyy", null);

            Console.Write("Priority (None, Low, Medium, High, Urgent): ");
            var priority = Enum.Parse<Priority>(Console.ReadLine() ?? "None", true);

            var item = new WorkItem
            {
                CreationDate = DateTime.Now,
                DueDate = dueDate,
                Priority = priority,
                Title = title,
                Description = description,
                IsCompleted = false
            };

            var id = _repository.Add(item);
            _repository.SaveChanges();
            Console.WriteLine($"Завдання додано з ID: {id}");
        }

        private static void BuildPlan()
        {
            var plan = _planner.CreatePlan();
            Console.WriteLine("\n--- План виконання ---");
            if (plan.Length == 0)
            {
                Console.WriteLine("Немає незавершених завдань.");
                return;
            }

            for (int i = 0; i < plan.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {plan[i]}");
            }
        }

        private static void MarkAsCompleted()
        {
            var allItems = _repository.GetAll();
            if (allItems.Length == 0)
            {
                Console.WriteLine("Немає завдань.");
                return;
            }

            Console.WriteLine("\nСписок завдань:");
            for (int i = 0; i < allItems.Length; i++)
            {
                Console.WriteLine($"{i + 1}. [{allItems[i].Id}] {allItems[i].Title} — due {allItems[i].DueDate:dd.MM.yyyy}, {allItems[i].Priority.ToString().ToLower()} priority");
                if (allItems[i].IsCompleted) Console.WriteLine("   (завершене)");
            }

            Console.Write("\nВведіть номер завдання або повний ID: ");
            var input = Console.ReadLine()?.Trim();

            WorkItem itemToMark = null;

            if (int.TryParse(input, out int number) && number >= 1 && number <= allItems.Length)
            {
                itemToMark = allItems[number - 1];
            }
            else if (Guid.TryParse(input, out Guid guid))
            {
                itemToMark = _repository.Get(guid);
            }

            if (itemToMark == null)
            {
                Console.WriteLine("Завдання не знайдено.");
                return;
            }

            if (itemToMark.IsCompleted)
            {
                Console.WriteLine("Це завдання вже завершене.");
                return;
            }

            itemToMark.IsCompleted = true;
            _repository.Update(itemToMark);
            _repository.SaveChanges();
            Console.WriteLine("Завдання позначено як завершене.");
        }

        private static void RemoveWorkItem()
        {
            var allItems = _repository.GetAll();
            if (allItems.Length == 0)
            {
                Console.WriteLine("Немає завдань.");
                return;
            }

            Console.WriteLine("\nСписок завдань:");
            for (int i = 0; i < allItems.Length; i++)
            {
                Console.WriteLine($"{i + 1}. [{allItems[i].Id}] {allItems[i].Title}");
            }

            Console.Write("\nВведіть номер завдання або повний ID для видалення: ");
            var input = Console.ReadLine()?.Trim();

            bool removed = false;

            if (int.TryParse(input, out int number) && number >= 1 && number <= allItems.Length)
            {
                removed = _repository.Remove(allItems[number - 1].Id);
            }
            else if (Guid.TryParse(input, out Guid guid))
            {
                removed = _repository.Remove(guid);
            }

            if (removed)
            {
                _repository.SaveChanges();
                Console.WriteLine("Завдання видалено.");
            }
            else
            {
                Console.WriteLine("Завдання не знайдено.");
            }
        }
    }
}