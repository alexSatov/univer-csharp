using Ddd.Infrastructure;

namespace Ddd.Taxi.Domain
{
    public class Driver : Entity<int>
    {
        public int DriverId { get; }
        public PersonName Name { get; }

        public Driver(int driverId, PersonName name):base(driverId)
        {
            Name = name;
            DriverId = driverId;
        }
    }
}