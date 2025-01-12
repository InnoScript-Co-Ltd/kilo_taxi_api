﻿using KiloTaxi.EntityFramework.EntityModel;
using KiloTaxi.Model.DTO;

namespace KiloTaxi.Realtime.HubInterfaces
{
    public interface IDriverHubHub
    {
        Task SendVehicleLocation(VehicleLocation vehicleLocation);
        Task SendSos(SosDTO sosDto);
        Task SendTripLocation(TripLocation tripLocation);
        Task SendDriverAvalilityStatus(string AvailityStatus);
        Task SendTripBegin(OrderDTO orderDto);
        Task AcceptOrder(OrderDTO orderDTO);
        Task ArrivedLocation(OrderDTO orderDTO);
    }
}
