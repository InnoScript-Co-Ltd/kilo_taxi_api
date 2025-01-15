﻿using System.ComponentModel.DataAnnotations;
using KiloTaxi.Common.Enums;

namespace KiloTaxi.Model.DTO.Request;

public class ReasonFormDTO
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public GeneralStatus Status { get; set; }
}