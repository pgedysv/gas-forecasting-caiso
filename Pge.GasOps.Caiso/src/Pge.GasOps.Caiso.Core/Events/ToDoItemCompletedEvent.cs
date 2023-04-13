using Pge.GasOps.Caiso.Core.Entities;
using Pge.GasOps.Caiso.Core.SharedKernel;

namespace Pge.GasOps.Caiso.Core.Events
{
    public class ToDoItemCompletedEvent : BaseDomainEvent
    {
        public ToDoItem CompletedItem { get; set; }

        public ToDoItemCompletedEvent(ToDoItem completedItem)
        {
            CompletedItem = completedItem;
        }
    }
}