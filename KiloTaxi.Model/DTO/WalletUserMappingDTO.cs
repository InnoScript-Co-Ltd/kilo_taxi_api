using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KiloTaxi.Common.Enums;

namespace KiloTaxi.Model.DTO
{
    public class WalletUserMappingDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Balance { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        [Required]
        public WalletType WalletType { get; set; }
        [Required]
        public WalletStatus Status { get; set; }
        [Required]
        public int WalletId { get; set; }
    }
}
