namespace CAFM.Database.Models;

public partial class TaskStatue
{
    public int Id { get; set; }

    public int CompanyId { get; set; }

    public string StatusName { get; set; } = null!;

    public string? StatusNameEn { get; set; }

    public bool IsStart { get; set; }

    public bool IsCompleted { get; set; }

    public bool IsCheckInstructions { get; set; }

    public bool IsMandatory { get; set; }

    public byte StatusOrder { get; set; }

    public bool IsRejected { get; set; }

    public virtual ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
}
