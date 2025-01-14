using System;
using KiloTaxi.Common.Enums;

namespace KiloTaxi.Model.DTO.Response
{
    public class AdminInfoDTO
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Phone { get; set; }

        public string Email { get; set; }

        public DateTime? EmailVerifiedAt { get; set; }

        public DateTime? PhoneVerifiedAt { get; set; }

        public string Role { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }

        public GenderType Gender { get; set; }

        public string? Otp { get; set; }

        public string? Address { get; set; }

        public CustomerStatus Status { get; set; }
    }
}
