using KiloTaxi.Model.DTO;

namespace KiloTaxi.DataAccess.Interface;

public interface ISmsRepository
{
    SmsPagingDTO GetAllSms(PageSortParam pageSortParam);
    SmsDTO CreateSms(SmsDTO smsDTO);
    bool UpdateSms(SmsDTO smsDTO);
    SmsDTO GetSmsById(int id);
    bool DeleteSms(int id);
}
