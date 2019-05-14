using System;
using SagaPattern.Infrastructure;

namespace SagaPattern.Domains.Process
{
    public static class ProcessMessages
    {
        public class ReservationExpired : EventBase
        {
            public Guid ReservationId { get; set; }
            public string Reason { get; set; }
        }
    }
}