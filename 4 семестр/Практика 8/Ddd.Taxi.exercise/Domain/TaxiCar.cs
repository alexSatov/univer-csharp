using Ddd.Infrastructure;

namespace Ddd.Taxi.Domain
{
    public class TaxiCar : ValueType<TaxiCar>
    {
        public string CarModel { get; }
        public string CarColor { get; }
        public string CarPlateNumber { get; }

        public TaxiCar(string carModel, string carColor, string carPlateNumber)
        {
            CarModel = carModel;
            CarColor = carColor;
            CarPlateNumber = carPlateNumber;
        }
    }
}