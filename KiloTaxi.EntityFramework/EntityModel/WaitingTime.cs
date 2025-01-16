using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiloTaxi.EntityFramework.EntityModel;

public class WaitingTime
{
    [Key]
    [DatabaseGenerated((DatabaseGeneratedOption.Identity))]
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Location{get;set;}
    public string  Lat{get;set;}
    public string Long{ get; set; }
    public int OrderId { get; set; }
    public virtual Order Order { get; set; }

}