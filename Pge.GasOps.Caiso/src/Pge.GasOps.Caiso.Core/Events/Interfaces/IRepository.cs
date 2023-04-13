using System.Collections.Generic;
using Pge.GasOps.Caiso.Core.SharedKernel;

namespace Pge.GasOps.Caiso.Core.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        T GetById(int id);
        List<T> List();
        T Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}