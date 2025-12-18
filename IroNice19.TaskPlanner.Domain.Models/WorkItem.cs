using IroNice19.TaskPlanner.Domain.Models.Enums;

namespace IroNice19.TaskPlanner.Domain.Models;

public class WorkItem
{
    public DateTime CreationDate { get; set; }
    public Guid Id { get; set; }
    public DateTime DueDate { get; set; }
    public Priority Priority { get; set; }
    public Complexity Complexity { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsCompleted { get; set; }

    public WorkItem Clone()
    {
        return new WorkItem
        {
            Id = this.Id,
            CreationDate = this.CreationDate,
            DueDate = this.DueDate,
            Priority = this.Priority,
            Complexity = this.Complexity,
            Title = this.Title,
            Description = this.Description,
            IsCompleted = this.IsCompleted
        };
    }

    public override string ToString()
    {
        string priorityText = Priority.ToString().ToLower();
        if (Priority == Priority.None)
            priorityText = "no priority";
        else if (Priority == Priority.Urgent)
            priorityText = "urgent priority";
        else
            priorityText += " priority";

        return $"{Title}: due {DueDate:dd.MM.yyyy}, {priorityText}";
    }
}