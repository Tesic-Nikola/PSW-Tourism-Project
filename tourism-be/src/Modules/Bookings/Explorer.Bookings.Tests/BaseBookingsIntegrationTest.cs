using Explorer.BuildingBlocks.Tests;

namespace Explorer.Bookings.Tests;

public class BaseBookingsIntegrationTest : BaseWebIntegrationTest<BookingsTestFactory>
{
    public BaseBookingsIntegrationTest(BookingsTestFactory factory) : base(factory) { }
}