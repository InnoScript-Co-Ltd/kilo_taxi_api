using System.ComponentModel.DataAnnotations;
using KiloTaxi.Common.Enums;

namespace KiloTaxi.Model.DTO.Response;

public class ReasonInfoDTO
{
    public int Id { get; set; }

    public string Name { get; set; }

    public GeneralStatus Status { get; set; }
}
