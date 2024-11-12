using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiloTaxi.Common.Enums
{
    public enum GenderType
    {
        Male,
        Female,
        Undefined
    }

    public enum Status
    {
        Pending,
        Success,
        Expired
    }

    public enum DriverStatus
    {
        Pending,
        Active,
        Deactivate,
        Suspend
    }

    public enum KycStatus
    {
        Pending,
        FullKyc,
        Reject
    }

    public enum VehicleStatus
    {
        Active,
        Suspend
    }
}