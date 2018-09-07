using System;
using System.Linq;

namespace Incapsulation.EnterpriseTask
{
    public class Enterprise
    {
        public readonly Guid Guid;
        public string Name { get; set; }
        public DateTime EstablishDate { get; set; }
        public TimeSpan ActiveTimeSpan => DateTime.Now - EstablishDate;

        public string Inn
        {
            get { return inn; }
            set
            {
                if (value.Length != 10 || !value.All(char.IsDigit))
                    throw new ArgumentException();
                inn = value;
            }
        }

        private string inn;

        public Enterprise(Guid guid)
        {
            Guid = guid;
        }

        public double GetTotalTransactionsAmount()
        {
            DataBase.OpenConnection();
            return DataBase.Transactions().Where(z => z.EnterpriseGuid == Guid).Sum(t => t.Amount);
        }
    }
}
