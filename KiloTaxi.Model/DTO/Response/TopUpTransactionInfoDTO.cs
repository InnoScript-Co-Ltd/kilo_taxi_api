using KiloTaxi.Common.Enums;

namespace KiloTaxi.Model.DTO
{
    public class TopUpTransactionInfoDTO
    {
        public int Id { get; set; }

        public decimal Amount { get; set; }

        public string? TransactionScreenShoot { get; set; }

        public TopUpTransactionStatus Status { get; set; }

        public string? PhoneNumber { get; set; }

        public string? DigitalPaymentFromPhoneNumber { get; set; }

        public string? DigitalPaymentToPhoneNumber { get; set; }

        public int PaymentChannelId { get; set; }

        public string PaymentChannelName { get; set; }

        public string? File_TransactionScreenShoot { get; set; }

        public int UseId { get; set; }

        public DateTime TransactionDate { get; set; }
    }
}
