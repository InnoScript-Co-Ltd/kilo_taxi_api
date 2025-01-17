using KiloTaxi.Model.DTO;
using KiloTaxi.Model.DTO.Request;
using KiloTaxi.Model.DTO.Response;

namespace KiloTaxi.DataAccess.Interface;

public interface ISmsRepository
{
    ResponseDTO<SmsPagingDTO> GetAllSms(PageSortParam pageSortParam);
    SmsInfoDTO CreateSms(SmsFormDTO smsFormDTO);
    bool UpdateSms(SmsFormDTO smsFormDTO);
    SmsInfoDTO GetSmsById(int id);
    bool DeleteSms(int id);
}
