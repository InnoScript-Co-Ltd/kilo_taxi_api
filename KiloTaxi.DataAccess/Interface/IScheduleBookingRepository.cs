using KiloTaxi.Model.DTO;

namespace KiloTaxi.DataAccess.Interface;

public interface IScheduleBookingRepository
{
    ScheduleBookingPagingDTO GetAllScheduleBooking(PageSortParam pageSortParam);
    ScheduleBookingDTO AddScheduleBooking(ScheduleBookingDTO scheduleBookingDTO);
    bool UpdateScheduleBooking(ScheduleBookingDTO scheduleBookingDTO);
    ScheduleBookingDTO getScheduleBookingById(int id);
    bool DeleteScheduleBooking(int id);
}
