using System;
using System.ComponentModel.DataAnnotations;
using KiloTaxi.Common.Enums;
using Microsoft.AspNetCore.Http;

namespace KiloTaxi.Model.DTO.Request
{
    public class AdminFormDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(
            100,
            MinimumLength = 3,
            ErrorMessage = "Name must be between 3 and 100 characters."
        )]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(
            @"^9\d{7,9}$",
            ErrorMessage = "Phone number must start with 9 and be between 8 to 10 digits."
        )]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        public DateTime? EmailVerifiedAt { get; set; }

        public DateTime? PhoneVerifiedAt { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(
            26,
            MinimumLength = 6,
            ErrorMessage = "Password must be between 6 and 26 characters."
        )]
        public string Password { get; set; }

        public string? Role { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }

        public GenderType? Gender { get; set; }

        public string? Otp { get; set; }

        public string? Address { get; set; }

        public CustomerStatus Status { get; set; }
    }
}
