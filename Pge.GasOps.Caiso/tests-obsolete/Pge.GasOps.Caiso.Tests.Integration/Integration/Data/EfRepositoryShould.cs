﻿using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Pge.GasOps.Caiso.Core.Entities;
using Pge.GasOps.Caiso.Core.Interfaces;
using Pge.GasOps.Caiso.Infrastructure.NetStandard.Data;
using Xunit;

namespace Pge.GasOps.Caiso.Tests.Integration.Integration.Data
{
    public class EfRepositoryAddShould
    {
        private AppDbContext _dbContext;

        private static DbContextOptions<AppDbContext> CreateNewContextOptions()
        {
            // Create a fresh service provider, and therefore a fresh 
            // InMemory database instance.
            var serviceProvider = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(new ServiceCollection()
                                      .AddEntityFrameworkInMemoryDatabase());

            // Create a new options instance telling the context to use an
            // InMemory database and the new service provider.
            var builder = new DbContextOptionsBuilder<AppDbContext>();
            builder.UseInMemoryDatabase("cleanarchitecture")
                   .UseInternalServiceProvider(serviceProvider);

            return builder.Options;
        }

        [Fact]
        public void AddItemAndSetId()
        {
            var repository = GetRepository();
            var item = new ToDoItem();

            repository.Add(item);

            var newItem = repository.List().FirstOrDefault();

            Assert.Equal(item, newItem);
            Assert.True(newItem.Id > 0);

        }

        [Fact]
        public void UpdateItemAfterAddingIt()
        {
            // add an item
            var repository = GetRepository();
            var initialTitle = Guid.NewGuid().ToString();
            var item = new ToDoItem()
            {
                Title = initialTitle
            };
            repository.Add(item);
            
            // detach the item so we get a different instance
            _dbContext.Entry(item).State = EntityState.Detached;

            // fetch the item and update its title
            var newItem = repository.List()
                .FirstOrDefault(i => i.Title == initialTitle);
            Assert.NotSame(item, newItem);
            var newTitle = Guid.NewGuid().ToString();
            newItem.Title = newTitle;

            // Update the item
            repository.Update(newItem);        
            var updatedItem = repository.List()
                .FirstOrDefault(i => i.Title == newTitle);

            Assert.NotEqual(item.Title, updatedItem.Title);
            Assert.Equal(newItem.Id, updatedItem.Id);
        }

        [Fact]
        public void DeleteItemAfterAddingIt()
        {
            // add an item
            var repository = GetRepository();
            var initialTitle = Guid.NewGuid().ToString();
            var item = new ToDoItem()
            {
                Title = initialTitle
            };
            repository.Add(item);

            // delete the item
            repository.Delete(item);

            // verify it's no longer there
            Assert.DoesNotContain(repository.List(), 
                i => i.Title == initialTitle);
        }



        private EfRepository<ToDoItem> GetRepository()
        {
            var options = CreateNewContextOptions();
            var mockDispatcher = new Mock<IDomainEventDispatcher>();

            _dbContext = new AppDbContext(options, mockDispatcher.Object);
            return new EfRepository<ToDoItem>(_dbContext);
        }
    }
}