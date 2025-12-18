using IroNice19.TaskPlanner.DataAccess.Abstractions;
using IroNice19.TaskPlanner.Domain.Logic;
using IroNice19.TaskPlanner.Domain.Models;
using IroNice19.TaskPlanner.Domain.Models.Enums;
using Moq;
using System;
using System.Linq;
using Xunit;

namespace IronNice19.TaskPlanner.Domain.Logic.Tests
{
    public class SimpleTaskPlannerTests
    {
        [Fact]
        public void CreatePlan_Sorts_ByPriorityDescending_ThenDueDateAscending_ThenTitle()
        {
            var mockRepo = new Mock<IWorkItemsRepository>();

            var items = new[]
            {
                new WorkItem { Id = Guid.NewGuid(), Title = "C", Priority = Priority.Low,    DueDate = new DateTime(2025, 12, 20), IsCompleted = false },
                new WorkItem { Id = Guid.NewGuid(), Title = "A", Priority = Priority.High,   DueDate = new DateTime(2025, 12, 22), IsCompleted = false },
                new WorkItem { Id = Guid.NewGuid(), Title = "B", Priority = Priority.Urgent, DueDate = new DateTime(2025, 12, 21), IsCompleted = false },
                new WorkItem { Id = Guid.NewGuid(), Title = "D", Priority = Priority.High,   DueDate = new DateTime(2025, 12, 19), IsCompleted = false },
            };

            mockRepo.Setup(r => r.GetAll()).Returns(items);

            var planner = new SimpleTaskPlanner(mockRepo.Object);

            var plan = planner.CreatePlan();

            Assert.Equal(4, plan.Length);

            Assert.Equal("B", plan[0].Title);
            Assert.Equal("D", plan[1].Title);
            Assert.Equal("A", plan[2].Title);
            Assert.Equal("C", plan[3].Title);
        }

        [Fact]
        public void CreatePlan_Excludes_CompletedTasks()
        {
            var mockRepo = new Mock<IWorkItemsRepository>();

            var items = new[]
            {
                new WorkItem { Id = Guid.NewGuid(), Title = "Active",   IsCompleted = false },
                new WorkItem { Id = Guid.NewGuid(), Title = "Completed", IsCompleted = true },
                new WorkItem { Id = Guid.NewGuid(), Title = "Another Active", IsCompleted = false },
            };

            mockRepo.Setup(r => r.GetAll()).Returns(items);

            var planner = new SimpleTaskPlanner(mockRepo.Object);

            var plan = planner.CreatePlan();

            Assert.Equal(2, plan.Length);
            Assert.DoesNotContain(plan, i => i.Title == "Completed");
        }

        [Fact]
        public void CreatePlan_Includes_OnlyIncompleteTasks_AndSortsCorrectly()
        {
            var mockRepo = new Mock<IWorkItemsRepository>();

            var items = new[]
            {
                new WorkItem { Id = Guid.NewGuid(), Title = "Low", Priority = Priority.Low, DueDate = new DateTime(2025, 12, 25), IsCompleted = false },
                new WorkItem { Id = Guid.NewGuid(), Title = "High Early", Priority = Priority.High, DueDate = new DateTime(2025, 12, 19), IsCompleted = false },
                new WorkItem { Id = Guid.NewGuid(), Title = "High Late", Priority = Priority.High, DueDate = new DateTime(2025, 12, 21), IsCompleted = false },
                new WorkItem { Id = Guid.NewGuid(), Title = "Completed", Priority = Priority.Urgent, IsCompleted = true },
            };

            mockRepo.Setup(r => r.GetAll()).Returns(items);

            var planner = new SimpleTaskPlanner(mockRepo.Object);

            // Act
            var plan = planner.CreatePlan();

            // Assert
            Assert.Equal(3, plan.Length);
            Assert.Equal("High Early", plan[0].Title);
            Assert.Equal("High Late", plan[1].Title);
            Assert.Equal("Low", plan[2].Title);
        }
    }
}