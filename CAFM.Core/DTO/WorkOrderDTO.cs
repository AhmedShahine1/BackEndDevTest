using CAFM.Database.Models;

namespace CAFM.Core.DTO
{
    public class WorkOrderDTO
    {
        public long Id { get; set; }

        public int CompanyId { get; set; }

        public long LocationId { get; set; }

        public long InternalNumber { get; set; }

        public string TaskName { get; set; } = null!;

        public string? TaskDescription { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime DueDate { get; set; }

        public long? TaskAssignmentId { get; set; }

        public int EstimatedTime { get; set; }

        public short TaskTypeId { get; set; }

        public DateTime? CompletionDate { get; set; }

        public string? CompletionNote { get; set; }

        public int CompletionRatio { get; set; }

        public int AssetDownTime { get; set; }

        public bool IsDeleted { get; set; }

        public long CreatedBy { get; set; }

        public virtual Asset? Asset { get; set; }

        public virtual TaskPriority Priority { get; set; } = null!;

        public virtual TaskStatue TaskStatus { get; set; } = null!;

        public virtual ICollection<WorkOrderDetail> WorkOrderDetails { get; set; } = new List<WorkOrderDetail>();

    }
}
