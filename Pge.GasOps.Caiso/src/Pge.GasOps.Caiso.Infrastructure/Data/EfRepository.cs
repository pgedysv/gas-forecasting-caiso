//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.EntityFrameworkCore;
//using Pge.GasOps.Caiso.Core.Entities;
//using Pge.GasOps.Caiso.Core.Interfaces;
//using Pge.GasOps.Caiso.Core.SharedKernel;

//namespace Pge.GasOps.Caiso.Infrastructure.Data
//{
//    public class EfRepository<T> : IRepository<T> where T : BaseEntity
//    {
//        private readonly AppDbContext _dbContext;

//        public EfRepository(AppDbContext dbContext)
//        {
//            _dbContext = dbContext;
//        }

//        public T GetById(int id)
//        {
//            return _dbContext.Set<T>().SingleOrDefault(e => e.Id == id);
//        }

//        public List<T> List()
//        {
//            return _dbContext.Set<T>().ToList();
//        }

//        public T Add(T entity)
//        {
//            _dbContext.Set<T>().Add(entity);
//            _dbContext.SaveChanges();

//            return entity;
//        }

//        public void Delete(T entity)
//        {
//            _dbContext.Set<T>().Remove(entity);
//            _dbContext.SaveChanges();
//        }

//        public void Update(T entity)
//        {
//            _dbContext.Entry(entity).State = EntityState.Modified;
//            _dbContext.SaveChanges();
//        }
//    }
//}