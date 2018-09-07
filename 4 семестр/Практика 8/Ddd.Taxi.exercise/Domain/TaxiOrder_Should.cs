using System;
using System.Linq;
using System.Reflection;
using Ddd.Infrastructure;
using NUnit.Framework;

namespace Ddd.Taxi.Domain
{
	[TestFixture]
	public class TaxiOrder_Should
	{
		private readonly Type taxiOrderType = typeof(TaxiOrder);

		private ITaxiApi<TaxiOrder> CreateApi()
		{
			return new TaxiApi(() => time);
		}

		private DateTime time = DateTime.MinValue;

		private void AssertHasPropertyOrField(Type type, string propName, string propTypeName)
		{
			var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase;
			var field = type.GetField(propName, bindingFlags);
			if (field != null)
				Assert.AreEqual(propTypeName, field.FieldType.Name, type + " has wrong type of field " + propTypeName);
			else
			{
				var prop = type.GetProperty(propName, bindingFlags);
				Assert.IsNotNull(prop, type + " should have field or property " + propName + " with type " + propTypeName);
				Assert.AreEqual(propTypeName, prop.PropertyType.Name, type + " has wrong type of property " + propTypeName);
			}
		}

		private void AssertHasMethod(Type type, string methodName)
		{
			var method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
			Assert.IsNotNull(method, type.Name + " should have public method " + methodName);
		}

		private static void AssertNotAnemic(Type type)
		{
			var publicProps = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			var publicFields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
			Assert.IsEmpty(publicProps.Where(p => !(p.SetMethod?.IsPrivate ?? true)),
				type.Name + " should not have writable properties");
			Assert.IsEmpty(publicFields, type.Name + " should not have any public fields");
		}

		[Test]
		public void AssignDriver()
		{
			var taxiApi = CreateApi();
			time = new DateTime(2017, 1, 1);
			var order = taxiApi.CreateOrderWithoutDestination("John", "Doe", "Street1", "building1");
			time = new DateTime(2017, 1, 2);
			taxiApi.AssignDriver(order, 15);
			Assert.AreEqual(
				"OrderId: 0 Status: WaitingCarArrival ClientName: John Doe Driver: Drive Driverson From: Street1 building1 To:  LastProgressTime: 2017-01-02 00:00:00",
				taxiApi.GetShortOrderInfo(order));
			Assert.AreEqual(
				"Id: 15 DriverName: Drive Driverson Color: Baklazhan CarModel: Lada sedan PlateNumber: A123BT 66",
				taxiApi.GetDriverFullInfo(order));
		}

		[Test]
		public void BeInitialized_AfterCreation()
		{
			var taxiApi = CreateApi();
			time = new DateTime(2017, 1, 1);
			var order = taxiApi.CreateOrderWithoutDestination("John", "Doe", "Street1", "building1");
			Assert.AreEqual(
				"OrderId: 0 Status: WaitingForDriver ClientName: John Doe Driver:  From: Street1 building1 To:  LastProgressTime: 2017-01-01 00:00:00",
				taxiApi.GetShortOrderInfo(order));
		}

		[Test]
		public void BeRichModel()
		{
			AssertHasMethod(taxiOrderType, "AssignDriver");
			AssertHasMethod(taxiOrderType, "Cancel");
			AssertHasMethod(taxiOrderType, "UpdateDestination");
			AssertHasMethod(taxiOrderType, "StartRide");
			AssertHasMethod(taxiOrderType, "FinishRide");
		}

		[Test]
		public void CancelRide()
		{
			var taxiApi = CreateApi();
			var order = taxiApi.CreateOrderWithoutDestination("John", "Doe", "Street1", "building1");
			time = new DateTime(2000, 01, 30);
			taxiApi.Cancel(order);
			Assert.AreEqual(
				"OrderId: 0 Status: Canceled ClientName: John Doe Driver:  From: Street1 building1 To:  LastProgressTime: 2000-01-30 00:00:00",
				taxiApi.GetShortOrderInfo(order));
		}

		[Test]
		public void FinishRide()
		{
			var taxiApi = CreateApi();
			var order = taxiApi.CreateOrderWithoutDestination("John", "Doe", "Street1", "building1");
			taxiApi.UpdateDestination(order, "far far away", "42");
			taxiApi.AssignDriver(order, 15);
			taxiApi.StartRide(order);
			time = new DateTime(2017, 01, 20);
			taxiApi.FinishRide(order);
			Assert.AreEqual(
				"OrderId: 0 Status: Finished ClientName: John Doe Driver: Drive Driverson From: Street1 building1 To: far far away 42 LastProgressTime: 2017-01-20 00:00:00",
				taxiApi.GetShortOrderInfo(order));
		}

		[Test]
		public void NotBeAnemicEntity()
		{
			AssertNotAnemic(taxiOrderType);
			Assert.AreEqual(
				typeof(Entity<int>), taxiOrderType.BaseType,
				taxiOrderType.Name + " should inherit Entity<int>");
		}

		[Test]
		public void StartRide()
		{
			var taxiApi = CreateApi();
			time = new DateTime(2017, 1, 1);
			var order = taxiApi.CreateOrderWithoutDestination("John", "Doe", "Street1", "building1");
			taxiApi.AssignDriver(order, 15);
			taxiApi.UpdateDestination(order, "far", "away");
			time = new DateTime(2017, 1, 2);
			taxiApi.StartRide(order);
			Assert.AreEqual(
				"OrderId: 0 Status: InProgress ClientName: John Doe Driver: Drive Driverson From: Street1 building1 To: far away LastProgressTime: 2017-01-02 00:00:00",
				taxiApi.GetShortOrderInfo(order));
		}

		[Test]
		public void TaxiOrderConsistsOfValueObjects()
		{
			AssertHasPropertyOrField(taxiOrderType, "ClientName", nameof(PersonName));
			AssertHasPropertyOrField(taxiOrderType, "Start", nameof(Address));
			AssertHasPropertyOrField(taxiOrderType, "Destination", nameof(Address));
			AssertHasPropertyOrField(taxiOrderType, "Driver", "Driver");
			Assert.AreEqual(
				typeof(Entity<int>), taxiOrderType.BaseType,
				taxiOrderType.Name + " should inherit Entity<int>");
		}

		[Test]
		public void UnassignDriver()
		{
			var taxiApi = CreateApi();
			time = new DateTime(2000, 01, 01);
			var order = taxiApi.CreateOrderWithoutDestination("John", "Doe", "Street1", "building1");
			time = new DateTime(2000, 01, 30);
			taxiApi.AssignDriver(order, 15);
			taxiApi.UnassignDriver(order);
			Assert.AreEqual(
				"OrderId: 0 Status: WaitingForDriver ClientName: John Doe Driver:  From: Street1 building1 To:  LastProgressTime: 2000-01-01 00:00:00",
				taxiApi.GetShortOrderInfo(order));
		}

		[Test]
		public void UpdateDestination()
		{
			var taxiApi = CreateApi();
			time = new DateTime(2017, 1, 1);
			var order = taxiApi.CreateOrderWithoutDestination("John", "Doe", "Street1", "building1");
			taxiApi.UpdateDestination(order, "far", "away");
			Assert.AreEqual(
				"OrderId: 0 Status: WaitingForDriver ClientName: John Doe Driver:  From: Street1 building1 To: far away LastProgressTime: 2017-01-01 00:00:00",
				taxiApi.GetShortOrderInfo(order));
		}
	}
}