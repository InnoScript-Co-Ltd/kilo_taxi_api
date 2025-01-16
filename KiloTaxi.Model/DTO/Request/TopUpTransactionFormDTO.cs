using System.ComponentModel.DataAnnotations;
using KiloTaxi.Common.Enums;
using Microsoft.AspNetCore.Http;

namespace KiloTaxi.Model.DTO.Request
{
    public class TopUpTransactionFormDTO
    {
        public int Id { get; set; }

        [Required]
        [Range(0.01, 10000.00, ErrorMessage = "Amount must be between 0.01 and 10000.")]
        public decimal Amount { get; set; }

        public string? TransactionScreenShoot { get; set; }

        [Required]
        public TopUpTransactionStatus Status { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string? PhoneNumber { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string? DigitalPaymentFromPhoneNumber { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string? DigitalPaymentToPhoneNumber { get; set; }

        [Required]
        public int PaymentChannelId { get; set; }

        public IFormFile? File_TransactionScreenShoot { get; set; }

        [Required]
        public int UseId { get; set; }
    }
}
