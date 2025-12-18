using IroNice19.TaskPlanner.Domain.Models;
using IroNice19.TaskPlanner.DataAccess.Abstractions;

using System.Linq;

namespace IroNice19.TaskPlanner.Domain.Logic
{
    public class SimpleTaskPlanner
    {
        private readonly IWorkItemsRepository _repository;

        public SimpleTaskPlanner(IWorkItemsRepository repository)
        {
            _repository = repository;
        }

        public WorkItem[] CreatePlan()
        {
            var items = _repository.GetAll()
                .Where(item => !item.IsCompleted)  // ігноруємо завершені (Завдання 5)
                .ToList();

            items.Sort(CompareWorkItems);

            return items.ToArray();
        }

        private static int CompareWorkItems(WorkItem first, WorkItem second)
        {
            int priorityComparison = second.Priority.CompareTo(first.Priority);
            if (priorityComparison != 0) return priorityComparison;

            int dueDateComparison = first.DueDate.CompareTo(second.DueDate);
            if (dueDateComparison != 0) return dueDateComparison;

            return string.Compare(first.Title, second.Title, StringComparison.OrdinalIgnoreCase);
        }
    }
}