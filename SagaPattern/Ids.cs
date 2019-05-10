using System;

namespace SagaPattern
{
    public static class Ids
    {
        public static readonly Guid LambdaWorld = Guid.Parse("{F364C8F8-15AF-4F5A-9616-AE0931F40FD2}");

        public static readonly Guid PayPal = Guid.Parse("{2CAA0415-1800-4A04-B365-212FCAA8F8EE}");
        public static readonly Guid Check = Guid.Parse("{16E9D044-32DB-46CB-8001-F7CC75E041B7}");
        public static readonly Guid Visa = Guid.Parse("{35B8D0AC-5B2C-42B0-9A89-A9A527E01BEA}");

        public static readonly Guid StarkIndustries = Guid.Parse("{A5451653-456A-4E9D-9980-D1AFF8F017EE}");
        public static readonly Guid JonSnow = Guid.Parse("{45FAE19D-5158-4F1A-9ABA-345A897656EC}");

        public static Guid New()
        {
            return Guid.NewGuid();
        }
    }
}