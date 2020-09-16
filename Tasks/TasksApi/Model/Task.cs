using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TasksApi.Model
{
    public class Task
    {
        public string Status { get; set; }
        public int Priority { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Notes { get; set; }

        public Task()
        {
        }

        public override bool Equals(object obj)
        {
            return obj is Task task &&
                   Status == task.Status &&
                   Priority == task.Priority &&
                   StartDate == task.StartDate &&
                   EndDate == task.EndDate &&
                   Notes == task.Notes;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Status, Priority, StartDate, EndDate, Notes);
        }
    }
}
