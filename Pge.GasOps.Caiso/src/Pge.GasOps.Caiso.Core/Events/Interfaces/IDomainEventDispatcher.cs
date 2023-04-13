using Pge.GasOps.Caiso.Core.SharedKernel;

namespace Pge.GasOps.Caiso.Core.Interfaces
{
    public interface IDomainEventDispatcher
    {
        void Dispatch(BaseDomainEvent domainEvent);
    }
}