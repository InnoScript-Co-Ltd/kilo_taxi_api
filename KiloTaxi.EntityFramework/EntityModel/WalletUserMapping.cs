using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiloTaxi.EntityFramework.EntityModel
{
    public class WalletUserMapping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Balance { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        [Required]
        public string WalletType { get; set; }
        [Required]
        public string Status { get; set; }

        [ForeignKey("Wallet")]
        public int WalletId { get; set; }
        public virtual Wallet Wallet { get; set; }
       
    }
}
