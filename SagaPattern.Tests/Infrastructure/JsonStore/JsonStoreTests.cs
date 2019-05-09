using System;
using System.IO;
using FluentAssertions;
using SagaPattern.Infrastructure.JsonStore;
using Xunit;
namespace SagaPattern.Tests.Infrastructure.JsonStore
{
    public class JsonStoreTests
    {
        [Fact]
        public void ensure_id_property()
        {
            this.Invoking(_ =>
                {
                    var store = new JsonStore<EntityWithoutId>();
                })
                .Should().Throw<InvalidOperationException>()
                .WithMessage("* Id * EntityWithoutId*");
        }

        [Fact]
        public void ensure_version_property()
        {
            this.Invoking(_ =>
                {
                    var store = new JsonStore<EntityWithoutVersion>();
                })
                .Should().Throw<InvalidOperationException>()
                .WithMessage("* Version * EntityWithoutVersion*");
        }

        [Fact]
        public void get_missing_entity()
        {
            var store = new JsonStore<Entity>();

            store.Get(Guid.NewGuid()).Should().BeNull();
        }

        [Fact]
        public void get_existing_entity()
        {
            WithStoreInFolder<Entity>(nameof(get_existing_entity), (folder, store) =>
            {
                var existingEntityId = Guid.NewGuid();
                store.Save(new Entity(existingEntityId, "test value"));

                var entity = new JsonStore<Entity>(folder)
                    .Get(existingEntityId);

                entity.Id.Should().Be(existingEntityId);
                entity.Value.Should().Be("test value");
            });
        }

        [Fact]
        public void save_increment_version()
        {
            WithStoreInFolder<Entity>(nameof(save_increment_version), (_, store) =>
            {
                var existingEntityId = Guid.NewGuid();
                store.Save(new Entity(existingEntityId, "test value"));

                var entity = store.Get(existingEntityId);
                store.Save(entity);

                store.Get(existingEntityId).Version.Should().Be(2);
            });
        }

        [Fact]
        public void optimistic_lock()
        {
            WithStoreInFolder<Entity>(nameof(optimistic_lock), (_, store) =>
            {
                var existingEntityId = Guid.NewGuid();
                store.Save(new Entity(existingEntityId, "test value"));

                store.Invoking(x => x.Save(new Entity(existingEntityId, "test value")))
                    .Should().Throw<StoreConcurrencyException>();
            });
        }

        [Fact]
        public void save_does_not_mutate_original_entity()
        {
            WithStoreInFolder<Entity>(nameof(save_does_not_mutate_original_entity), (_, store) =>
            {
                var existingEntityId = Guid.NewGuid();
                var entity = new Entity(existingEntityId, "test value");

                store.Save(entity);

                entity.Version.Should().Be(0);
                entity.Should().Be(entity);
            });
        }

        private void WithStoreInFolder<T>(string folder, Action<string, JsonStore<T>> test) where T : class
        {
            var fullPath = Path.GetFullPath(folder);
            try
            {
                if (Directory.Exists(fullPath))
                {
                    Directory.Delete(fullPath, true);
                }
                Directory.CreateDirectory(fullPath);
                test(fullPath, new JsonStore<T>(fullPath));
            }
            finally
            {
                Directory.Delete(fullPath, true);
            }
        }

        private class EntityWithoutId
        {
            public int Version { get; set; }
        }

        private class EntityWithoutVersion
        {
            public Guid Id { get; set; }
        }

        private class Entity
        {
            public Entity(Guid id, string value)
            {
                Id = id;
                Value = value;
            }

            public Guid Id { get; private set; }
            public string Value { get; private set; }
            public int Version { get; private set; }
        }
    }
}