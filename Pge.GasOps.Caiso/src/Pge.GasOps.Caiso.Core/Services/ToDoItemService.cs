using Ardalis.GuardClauses;
using Pge.GasOps.Caiso.Core.Events;
using Pge.GasOps.Caiso.Core.Interfaces;

namespace Pge.GasOps.Caiso.Core.Services
{
    public class ToDoItemService : IHandle<ToDoItemCompletedEvent>
    {
        public void Handle(ToDoItemCompletedEvent domainEvent)
        {
            Guard.Against.Null(domainEvent, nameof(domainEvent));

            // Do Nothing
        }
    }
}
