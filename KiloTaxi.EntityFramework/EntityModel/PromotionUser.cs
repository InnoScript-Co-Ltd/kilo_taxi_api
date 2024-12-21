using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiloTaxi.EntityFramework.EntityModel;

public class PromotionUser
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [ForeignKey("Promotion")]
    public int PromotionId { get; set; }
    public virtual Promotion Promotion { get; set; }

    [ForeignKey("Customer")]
    public int CustomerId { get; set; }
    public virtual Customer Customer { get; set; }
}
