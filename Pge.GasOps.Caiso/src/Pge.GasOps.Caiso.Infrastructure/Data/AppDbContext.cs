﻿//using System.Linq;
//using Microsoft.EntityFrameworkCore;
//using Pge.GasOps.Caiso.Core.Entities;
//using Pge.GasOps.Caiso.Core.Interfaces;
//using Pge.GasOps.Caiso.Core.SharedKernel;

//namespace Pge.GasOps.Caiso.Infrastructure.Data
//{
//    public class AppDbContext : DbContext
//    {
//        private readonly IDomainEventDispatcher _dispatcher;

//        public AppDbContext(DbContextOptions<AppDbContext> options, IDomainEventDispatcher dispatcher)
//            : base(options)
//        {
//            _dispatcher = dispatcher;
//        }

//        public DbSet<ToDoItem> ToDoItems { get; set; }

//        public override int SaveChanges()
//        {
//            int result = base.SaveChanges();

//            // dispatch events only if save was successful
//            var entitiesWithEvents = ChangeTracker.Entries<BaseEntity>()
//                .Select(e => e.Entity)
//                .Where(e => e.Events.Any())
//                .ToArray();

//            foreach (var entity in entitiesWithEvents)
//            {
//                var events = entity.Events.ToArray();
//                entity.Events.Clear();
//                foreach (var domainEvent in events)
//                {
//                    _dispatcher.Dispatch(domainEvent);
//                }
//            }

//            return result;
//        }
//    }
//}