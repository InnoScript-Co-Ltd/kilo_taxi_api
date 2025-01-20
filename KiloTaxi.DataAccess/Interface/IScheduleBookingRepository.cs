using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.DataAccess.Interface;

public interface IScheduleBookingRepository
{
    ResponseDTO<ScheduleBookingPagingDTO> GetAllScheduleBooking(PageSortParam pageSortParam);
    ScheduleBookingInfoDTO AddScheduleBooking(ScheduleBookingFormDTO scheduleBookingFormDTO);
    bool UpdateScheduleBooking(ScheduleBookingFormDTO scheduleBookingFormDTO);
    ScheduleBookingInfoDTO getScheduleBookingById(int id);
    bool DeleteScheduleBooking(int id);
}
