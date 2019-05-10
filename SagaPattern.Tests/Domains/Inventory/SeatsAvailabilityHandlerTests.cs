using System;
using System.Threading.Tasks;
using FluentAssertions;
using SagaPattern.Domains.Inventory;
using SagaPattern.Infrastructure;
using Xunit;
using static SagaPattern.Domains.Inventory.InventoryMessages;

namespace SagaPattern.Tests.Domains.Inventory
{
    public class SeatsAvailabilityHandlerTests : IStore<SeatsAvailability>, IPublisher
    {
        [Fact]
        public void remove_seats_of_missing_seats_availability()
        {
            var handler = new SeatsAvailabilityHandler(this, this);
            var missingSeatsAvailabilityId = Guid.NewGuid();

            handler.Invoking(x => x.Handle(new RemoveSeats {SeatsAvailabilityId = missingSeatsAvailabilityId}).Wait())
                .Should().Throw<InvalidOperationException>()
                .WithMessage($"*{missingSeatsAvailabilityId}* does not exists");
        }

        SeatsAvailability IStore<SeatsAvailability>.Get(Guid id)
        {
            return null;
        }

        void IStore<SeatsAvailability>.Save(SeatsAvailability item)
        {
        }

        Task IPublisher.Publish<T>(T message)
        {
            return Task.CompletedTask;
        }
    }
}