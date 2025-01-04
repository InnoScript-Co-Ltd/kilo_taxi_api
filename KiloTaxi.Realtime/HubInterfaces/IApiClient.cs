﻿using KiloTaxi.Model.DTO;

namespace KiloTaxi.Realtime.HubInterfaces;

public interface IApiClient
{
    Task ReceiveAvailityStatus(string availabilityStatus, int key);
    Task ReceiveTripLocation(TripLocation tripLocation);
    Task ReceiveTripComplete(OrderDTO order, List<ExtraDemandDTO> extraDemands);
}
