using IroNice19.TaskPlanner.Domain.Models;
using System;
using System.Collections.Generic;

namespace IroNice19.TaskPlanner.Domain.Logic
{
    public class SimpleTaskPlanner
    {
        public WorkItem[] CreatePlan(WorkItem[] workItems)
        {
            // Конвертація масиву в List
            List<WorkItem> workList = workItems.ToList();

            // Сортування за допомогою Comparison<T>
            workList.Sort((x, y) =>
            {
                // i. Priority - за спаданням
                int priorityComparison = y.Priority.CompareTo(x.Priority);
                if (priorityComparison != 0) return priorityComparison;

                // ii. DueDate - за зростанням
                int dueDateComparison = x.DueDate.CompareTo(y.DueDate);
                if (dueDateComparison != 0) return dueDateComparison;

                // iii. Title - в алфавітному порядку
                return string.Compare(x.Title, y.Title, StringComparison.Ordinal);
            });

            // Конвертація посортованого List у масив
            return workList.ToArray();
        }
    }
}