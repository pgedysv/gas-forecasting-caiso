using Pge.GasOps.Caiso.Core.Events;
using Pge.GasOps.Caiso.Core.SharedKernel;

namespace Pge.GasOps.Caiso.Core.Entities
{
    public class ToDoItem : BaseEntity
    {
        public string Title { get; set; } 
        public string Description { get; set; }
        public bool IsDone { get; private set; } = false;

        public void MarkComplete()
        {
            IsDone = true;
            Events.Add(new ToDoItemCompletedEvent(this));
        }
    }
}