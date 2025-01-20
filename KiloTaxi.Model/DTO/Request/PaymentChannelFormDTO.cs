﻿using System.ComponentModel.DataAnnotations;
using KiloTaxi.Common.Enums;
using Microsoft.AspNetCore.Http;

namespace KiloTaxi.Model.DTO.Request;

public class PaymentChannelFormDTO
{
    public int Id { get; set; }
    
    [Required]
    public string ChannelName { get; set; }

    [Required]
    public string Description { get; set; }

    public PaymentType PaymentType { get; set; }

    public string? Icon { get; set; }

    public string? Phone { get; set; }

    public string? UserName { get; set; }
    
    public IFormFile? File_Icon { get; set; }
}
