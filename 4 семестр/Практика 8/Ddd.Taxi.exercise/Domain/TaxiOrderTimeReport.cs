using System;
using Ddd.Infrastructure;

namespace Ddd.Taxi.Domain
{
    public class TaxiOrderTimeReport : ValueType<TaxiOrderTimeReport>
    {
        public DateTime CreationTime { get; }
        public DateTime DriverAssignmentTime { get; private set; }
        public DateTime CancelTime { get; private set; }
        public DateTime StartRideTime { get; private set; }
        public DateTime FinishRideTime { get; private set; }

        private readonly TaxiOrder order;

        public TaxiOrderTimeReport(DateTime creationTime, TaxiOrder taxiOrder)
        {
            CreationTime = creationTime;
            order = taxiOrder;
        }

        public void Report(DateTime time)
        {
            switch (order.Status)
            {
                case TaxiOrderStatus.WaitingCarArrival:
                    DriverAssignmentTime = time;
                    break;
                case TaxiOrderStatus.InProgress:
                    StartRideTime = time;
                    break;
                case TaxiOrderStatus.Finished:
                    FinishRideTime = time;
                    break;
                case TaxiOrderStatus.Canceled:
                    CancelTime = time;
                    break;
                default:
                    throw new NotSupportedException($"Информация о времени для статуса {order.Status} не сохраняется!");
            }
        }

        public DateTime GetLastProgressTime()
        {
            switch (order.Status)
            {
                case TaxiOrderStatus.WaitingForDriver:
                    return CreationTime;
                case TaxiOrderStatus.WaitingCarArrival:
                    return DriverAssignmentTime;
                case TaxiOrderStatus.InProgress:
                    return StartRideTime;
                case TaxiOrderStatus.Finished:
                    return FinishRideTime;
                case TaxiOrderStatus.Canceled:
                    return CancelTime;
                default:
                    throw new NotSupportedException(order.Status.ToString());
            }
        }
    }
}