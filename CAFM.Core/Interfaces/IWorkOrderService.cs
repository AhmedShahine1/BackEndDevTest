using CAFM.Core.DTO;
using CAFM.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAFM.Core.Interfaces
{
    public interface IWorkOrderService
    {
        Task<long> SaveWorkOrderAsync(WorkOrder workOrder);
        Task<long> GenerateInternalNumberAsync();
        Task<bool> UpdateWorkOrderStatusAsync(long id, int statusUpdate);
        Task<IEnumerable<WorkOrderDTO>> GetAllWorkOrderAsync();
        Task<WorkOrderDTO> GetWorkOrderByIdAsync(long workOrderId);
    }
}
