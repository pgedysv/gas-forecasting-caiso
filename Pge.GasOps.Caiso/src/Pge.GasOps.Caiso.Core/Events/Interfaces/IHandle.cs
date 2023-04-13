using Pge.GasOps.Caiso.Core.SharedKernel;

namespace Pge.GasOps.Caiso.Core.Interfaces
{
    public interface IHandle<T> where T : BaseDomainEvent
    {
        void Handle(T domainEvent);
    }
}