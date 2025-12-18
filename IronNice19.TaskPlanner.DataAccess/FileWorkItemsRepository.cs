using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using IroNice19.TaskPlanner.DataAccess.Abstractions;
using IroNice19.TaskPlanner.Domain.Models;

namespace IroNice19.TaskPlanner.DataAccess
{
    public class FileWorkItemsRepository : IWorkItemsRepository
    {
        private const string FileName = "work-items.json";
        private readonly Dictionary<Guid, WorkItem> _items = new();

        public FileWorkItemsRepository()
        {
            if (File.Exists(FileName))
            {
                var json = File.ReadAllText(FileName);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    var loadedItems = JsonConvert.DeserializeObject<WorkItem[]>(json);
                    if (loadedItems != null)
                    {
                        foreach (var item in loadedItems)
                        {
                            _items[item.Id] = item;
                        }
                    }
                }
            }
        }

        public Guid Add(WorkItem workItem)
        {
            var copy = workItem.Clone();
            copy.Id = Guid.NewGuid();
            _items[copy.Id] = copy;
            return copy.Id;
        }

        public WorkItem Get(Guid id)
        {
            _items.TryGetValue(id, out var item);
            return item;
        }

        public WorkItem[] GetAll()
        {
            return _items.Values.ToArray();
        }

        public bool Update(WorkItem workItem)
        {
            if (!_items.ContainsKey(workItem.Id))
                return false;

            _items[workItem.Id] = workItem.Clone();
            return true;
        }

        public bool Remove(Guid id)
        {
            return _items.Remove(id);
        }

        public void SaveChanges()
        {
            var json = JsonConvert.SerializeObject(_items.Values, Formatting.Indented);
            File.WriteAllText(FileName, json);
        }
    }
}