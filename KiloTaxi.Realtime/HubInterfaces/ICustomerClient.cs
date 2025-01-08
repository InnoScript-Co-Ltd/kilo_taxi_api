﻿using KiloTaxi.Model.DTO;

namespace KiloTaxi.Realtime.HubInterfaces;

public interface ICustomerClient
{
    Task ReceiveTripComplete(
        string pickupLocation,
        string destinationLocation,
        decimal actualTotalPrice,
        decimal promoCodeDiscount,
        List<ExtraDemandDTO> extraDemands
    );
}
