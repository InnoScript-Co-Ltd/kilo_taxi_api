using System.ComponentModel.DataAnnotations;

namespace KiloTaxi.Model.DTO
{
    public class WalletUserMappingDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Balance { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        [Required]
        public string WalletType { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public int WalletId { get; set; }
    }
}
