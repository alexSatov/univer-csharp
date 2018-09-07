using System;
using System.Linq;
using System.Collections.Generic;

namespace Incapsulation
{
    public enum FailureType
    {
        Shutdown,
        NonResponding,
        HardwareFailure,
        ConnectionProblem
    }

    public class ReportMaker
    {
        /// <summary>
        /// </summary>
        /// <param name="day"></param>
        /// <param name="failureTypes">0 for unexpected shutdown, 1 for short non-responding, 2 for hardware failures, 3 for connection problems</param>
        /// <param name="deviceId"></param>
        /// <param name="times"></param>
        /// <param name="devices"></param>
        /// <returns></returns>
        public static List<string> FindDevicesFailedBeforeDateObsolete(
            int day,
            int month,
            int year,
            int[] failureTypes, 
            int[] deviceId, 
            object[][] times,
            List<Dictionary<string, object>> devices)
        {
            var obsoleteDate = new DateTime(year, month, day);
            var failedDevices = Enumerable.Range(0, deviceId.Length)
                .Select(i => new DeviceFailure(
                    devices[i]["Name"] as string,
                    (int) devices[i]["DeviceId"],
                    new DateTime((int) times[i][2], (int) times[i][1], (int) times[i][0]),
                    (FailureType) failureTypes[i]))
                .ToList();
            return FindDevicesFailedBeforeDateObsolete(obsoleteDate, failedDevices);
        }

        public static List<string> FindDevicesFailedBeforeDateObsolete(DateTime obsoleteDate, List<DeviceFailure> failures)
        {
            return failures
                .Where(f => f.IsSeriousFailure() && f.FailureDate < obsoleteDate)
                .Select(d => d.Name)
                .ToList();
        } 
    }

    public class DeviceFailure
    {
        public readonly string Name;
        public readonly int DeviceId;
        public readonly DateTime FailureDate;
        public readonly FailureType FailureType;

        public DeviceFailure(string name, int deviceId, DateTime failureDate, FailureType failureType)
        {
            Name = name;
            DeviceId = deviceId;
            FailureDate = failureDate;
            FailureType = failureType;
        }

        public bool IsSeriousFailure()
        {
            return FailureType == FailureType.HardwareFailure || FailureType == FailureType.Shutdown;
        }
    }
}
