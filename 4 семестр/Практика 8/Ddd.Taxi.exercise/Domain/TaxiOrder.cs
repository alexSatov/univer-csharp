using System;
using Ddd.Infrastructure;
using System.Globalization;

namespace Ddd.Taxi.Domain
{
	public class TaxiApi : ITaxiApi<TaxiOrder>
	{
		private int idCounter;
		private readonly Func<DateTime> currentTime;

		public TaxiApi(Func<DateTime> currentTime)
		{
			this.currentTime = currentTime;
		}

		public TaxiOrder CreateOrderWithoutDestination(string firstName, string lastName, string street, string building)
		{
		    return TaxiOrder.CreateOrderWithoutDestination(
		        idCounter++,
		        new PersonName(firstName, lastName),
		        new Address(street, building),
		        currentTime());
		}

		public void UpdateDestination(TaxiOrder order, string street, string building)
		{
			order.UpdateDestination(new Address(street, building));
		}

		public void AssignDriver(TaxiOrder order, int driverId)
		{
			order.AssignDriver(driverId, currentTime());
		}

		public void UnassignDriver(TaxiOrder order)
		{
			order.UnassignDriver();
		}

		public string GetDriverFullInfo(TaxiOrder order)
		{
		    return order.GetDriverFullInfo();
		}

		public string GetShortOrderInfo(TaxiOrder order)
		{
		    return order.GetShortOrderInfo();
		}

		public void Cancel(TaxiOrder order)
		{
			order.Cancel(currentTime());
		}

		public void StartRide(TaxiOrder order)
		{
            order.StartRide(currentTime());
        }

		public void FinishRide(TaxiOrder order)
		{
            order.FinishRide(currentTime());
        }
	}

	public class TaxiOrder : Entity<int>
    {
        public PersonName ClientName { get; }
        public Address Start { get; }
        public Address Destination { get; private set; }
        public TaxiCar Car { get; private set; }
        public Driver Driver { get; private set; }
        public TaxiOrderTimeReport TimeReport { get; }
        public TaxiOrderStatus Status { get; private set; }

        public TaxiOrder(int id, PersonName clientName, Address start, DateTime orderTime) : base(id)
        {
            ClientName = clientName;
            Start = start;
            TimeReport = new TaxiOrderTimeReport(orderTime, this);
	    }

	    public static TaxiOrder CreateOrderWithoutDestination(int id, PersonName client, Address start, DateTime orderTime)
	    {
            return new TaxiOrder(id, client, start, orderTime);
        }

        public void UpdateDestination(Address destination)
        {
            Destination = destination;
        }

        public void AssignDriver(int driverId, DateTime driverAssignmentTime)
        {
            if (Status != TaxiOrderStatus.WaitingForDriver)
                throw new InvalidOperationException($"Недопустимая операция для статуса {Status}");

            if (driverId == 15)
            {
                Driver = new Driver(driverId, new PersonName("Drive", "Driverson"));
                Car = new TaxiCar("Lada sedan", "Baklazhan", "A123BT 66");
            }
            else
                throw new Exception("Unknown driver id " + driverId);

            Status = TaxiOrderStatus.WaitingCarArrival;
            TimeReport.Report(driverAssignmentTime);
        }

        public void UnassignDriver()
        {
            if (Status != TaxiOrderStatus.WaitingCarArrival)
                throw new InvalidOperationException($"Недопустимая операция для статуса {Status}");

            Driver = null;
            Status = TaxiOrderStatus.WaitingForDriver;
        }

        public void StartRide(DateTime startRideTime)
        {
            if (Status != TaxiOrderStatus.WaitingCarArrival)
                throw new InvalidOperationException($"Недопустимая операция для статуса {Status}");

            Status = TaxiOrderStatus.InProgress;
            TimeReport.Report(startRideTime);
        }

        public void FinishRide(DateTime finishRideTime)
        {
            if (Status != TaxiOrderStatus.InProgress)
                throw new InvalidOperationException($"Недопустимая операция для статуса {Status}");

            Status = TaxiOrderStatus.Finished;
            TimeReport.Report(finishRideTime);
        }

        public void Cancel(DateTime cancelTime)
        {
            switch (Status)
            {
                case TaxiOrderStatus.InProgress:
                case TaxiOrderStatus.Finished:
                    throw new InvalidOperationException($"Недопустимая операция для статуса {Status}");
                case TaxiOrderStatus.Canceled:
                    throw new InvalidOperationException("Заказ уже отменен");
            }

            Status = TaxiOrderStatus.Canceled;
            TimeReport.Report(cancelTime);
        }

        public string GetDriverFullInfo()
        {
            if (Status == TaxiOrderStatus.WaitingForDriver) return null;
            return string.Join(" ",
                "Id: " + Driver.DriverId,
                "DriverName: " + FormatName(Driver.Name),
                "Color: " + Car.CarColor,
                "CarModel: " + Car.CarModel,
                "PlateNumber: " + Car.CarPlateNumber);
        }

        public string GetShortOrderInfo()
        {
            return string.Join(" ",
                "OrderId: " + Id,
                "Status: " + Status,
                "ClientName: " + FormatName(ClientName),
                "Driver: " + FormatName(Driver?.Name),
                "From: " + FormatAddress(Start),
                "To: " + FormatAddress(Destination),
                "LastProgressTime: " + TimeReport.GetLastProgressTime().ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
        }

        private static string FormatName(PersonName name)
        {
            return Equals(name, null) ? "" : $"{name.FirstName} {name.LastName}";
        }

        private static string FormatAddress(Address address)
        {
            return Equals(address, null) ? "" : $"{address.Street} {address.Building}";
        }
    }
}