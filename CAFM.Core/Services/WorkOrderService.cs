using CAFM.Core.DTO;
using CAFM.Core.Hubs;
using CAFM.Core.Interfaces;
using CAFM.Database.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

public class WorkOrderService : IWorkOrderService
{
    private readonly IHubContext<WorkOrderHub> _hubContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<WorkOrderService> _logger;

    public WorkOrderService(IUnitOfWork unitOfWork, IHubContext<WorkOrderHub> hubContext, ILogger<WorkOrderService> logger)
    {
        _unitOfWork = unitOfWork;
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task<long> SaveWorkOrderAsync(WorkOrder workOrder)
    {
        try
        {
            using var transaction = await _unitOfWork.GetDbContext().Database.BeginTransactionAsync();

            // Validation
            if (string.IsNullOrEmpty(workOrder.TaskName))
                throw new ArgumentException("Task Name is required.");

            workOrder.InternalNumber = await GenerateInternalNumberAsync();

            _unitOfWork.WorkOrderRepository.Add(workOrder);
            await _unitOfWork.SaveChangesAsync();

            var groupName = $"Company_{workOrder.CompanyId}_Location_{workOrder.LocationId}";
            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveWorkOrderUpdate", workOrder);

            await transaction.CommitAsync();
            _unitOfWork.Dispose();
            return workOrder.Id;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Argument error while saving work order.");
            throw; // Rethrow to be caught by controller
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while saving the work order.");
            throw new Exception("An error occurred while processing your request.", ex);
        }
    }

    public async Task<long> GenerateInternalNumberAsync()
    {
        // Fetch the highest existing internal number
        var lastInternalNumber = await _unitOfWork.WorkOrderRepository
            .FindAllAsync(w => w.InternalNumber > 0)
            .ContinueWith(task => task.Result.Max(w => (long?)w.InternalNumber) ?? 0);

        return lastInternalNumber + 1;
    }

    public async Task<WorkOrderDTO> GetWorkOrderByIdAsync(long workOrderId)
    {
        try
        {
            var workOrder = await _unitOfWork.WorkOrderRepository.FindAsync(a => a.Id == workOrderId);
            if (workOrder == null)
                throw new KeyNotFoundException($"WorkOrder with ID {workOrderId} not found.");

            // Map the WorkOrder entity to WorkOrderDTO
            return new WorkOrderDTO
            {
                Id = workOrder.Id,
                CompanyId = workOrder.CompanyId,
                LocationId = workOrder.LocationId,
                InternalNumber = workOrder.InternalNumber,
                TaskName = workOrder.TaskName,
                TaskDescription = workOrder.TaskDescription,
                CreatedDate = workOrder.CreatedDate,
                ModifiedDate = workOrder.ModifiedDate,
                StartDate = workOrder.StartDate,
                DueDate = workOrder.DueDate,
                TaskAssignmentId = workOrder.TaskAssignmentId,
                EstimatedTime = workOrder.EstimatedTime,
                TaskTypeId = workOrder.TaskTypeId,
                CompletionDate = workOrder.CompletionDate,
                CompletionNote = workOrder.CompletionNote,
                CompletionRatio = workOrder.CompletionRatio,
                AssetDownTime = workOrder.AssetDownTime,
                IsDeleted = workOrder.IsDeleted,
                CreatedBy = workOrder.CreatedBy,
                Asset = workOrder.Asset,
                Priority = workOrder.Priority,
                TaskStatus = workOrder.TaskStatus,
                WorkOrderDetails = workOrder.WorkOrderDetails,
            };
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogError(ex, "WorkOrder not found.");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching the WorkOrder.");
            throw new Exception("An error occurred while processing your request.", ex);
        }
    }

    public async Task<IEnumerable<WorkOrderDTO>> GetAllWorkOrderAsync()
    {
        try
        {
            // Fetch all WorkOrder entities
            var workOrders = await _unitOfWork.WorkOrderRepository.GetAllAsync();

            // Map entities to DTOs
            var workOrderDTOs = workOrders.Select(wo => new WorkOrderDTO
            {
                Id = wo.Id,
                CompanyId = wo.CompanyId,
                LocationId = wo.LocationId,
                InternalNumber = wo.InternalNumber,
                TaskName = wo.TaskName,
                TaskDescription = wo.TaskDescription,
                CreatedDate = wo.CreatedDate,
                ModifiedDate = wo.ModifiedDate,
                StartDate = wo.StartDate,
                DueDate = wo.DueDate,
                TaskAssignmentId = wo.TaskAssignmentId,
                EstimatedTime = wo.EstimatedTime,
                TaskTypeId = wo.TaskTypeId,
                CompletionDate = wo.CompletionDate,
                CompletionNote = wo.CompletionNote,
                CompletionRatio = wo.CompletionRatio,
                AssetDownTime = wo.AssetDownTime,
                IsDeleted = wo.IsDeleted,
                CreatedBy = wo.CreatedBy,
                Asset = wo.Asset, // Map directly if no further transformation needed
                Priority = wo.Priority, // Map directly
                TaskStatus = wo.TaskStatus, // Map directly
                WorkOrderDetails = wo.WorkOrderDetails // Map directly if no transformation is required
            });

            return workOrderDTOs;
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogError(ex, "WorkOrder not found.");
            throw; // Rethrow to be caught by the controller
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching the WorkOrder.");
            throw new Exception("An error occurred while processing your request.", ex);
        }
    }


    public async Task<bool> UpdateWorkOrderStatusAsync(long id, int statusUpdate)
    {
        try
        {
            var workOrder = await _unitOfWork.WorkOrderRepository.FindAsync(w => w.Id == id);
            if (workOrder == null)
            {
                _logger.LogWarning($"WorkOrder with ID {id} not found.");
                return false;
            }

            var status = await _unitOfWork.TaskStatueRepository.FindAsync(s => s.Id == statusUpdate);
            if (status == null)
            {
                _logger.LogWarning($"Status {statusUpdate} not found.");
                return false;
            }

            workOrder.TaskStatusId = status.Id;
            if (status.IsStart)
                workOrder.StartDate = workOrder.StartDate ?? DateTime.UtcNow;

            if (status.IsCompleted)
            {
                workOrder.CompletionDate = DateTime.UtcNow;
            }

            _unitOfWork.WorkOrderRepository.Update(workOrder);
            await _unitOfWork.SaveChangesAsync();

            var groupName = $"Company_{workOrder.CompanyId}_Location_{workOrder.LocationId}";
            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveWorkOrderUpdate", workOrder);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating WorkOrder status.");
            throw new Exception("An error occurred while processing your request.", ex);
        }
    }
}
