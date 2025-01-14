using System.ComponentModel.DataAnnotations;
using KiloTaxi.Common.Enums;
using Microsoft.AspNetCore.Http;

namespace KiloTaxi.Model.DTO.Request
{
    public class PaymentChannelFormDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Channel Name is required.")]
        [StringLength(50, ErrorMessage = "Channel Name cannot exceed 50 characters.")]
        public string ChannelName { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Payment Type is required.")]
        public PaymentType PaymentType { get; set; }

        public string? Icon { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string? Phone { get; set; }

        [StringLength(50, ErrorMessage = "User Name cannot exceed 50 characters.")]
        public string? UserName { get; set; }

        public IFormFile? File_Icon { get; set; }
    }
}
