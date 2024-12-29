using CAFM.Core.DTO;
using CAFM.Core.Interfaces;
using CAFM.Database.Models;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class WorkOrderController : ControllerBase
{
    private readonly IWorkOrderService _workOrderService;
    private readonly ILogger<WorkOrderController> _logger;

    public WorkOrderController(IWorkOrderService workOrderService, ILogger<WorkOrderController> logger)
    {
        _workOrderService = workOrderService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateWorkOrder([FromBody] WorkOrderPostDto workOrderDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var workOrder = new WorkOrder
            {
                CompanyId = workOrderDto.CompanyId,
                LocationId = workOrderDto.LocationId,
                TaskName = workOrderDto.TaskName,
                TaskDescription = workOrderDto.TaskDescription,
                DueDate = workOrderDto.DueDate,
                TaskAssignmentId = workOrderDto.TaskAssignmentId,
                EstimatedTime = workOrderDto.EstimatedTime,
                TaskTypeId = workOrderDto.TaskTypeId,
                AssetId = workOrderDto.AssetId,
                PriorityId = workOrderDto.PriorityId,
                TaskStatusId = workOrderDto.TaskStatusId,
                AssetDownTime = workOrderDto.AssetDownTime,
                CreatedBy = workOrderDto.CreatedBy,
                CreatedDate = DateTime.UtcNow
            };

            var workOrderId = await _workOrderService.SaveWorkOrderAsync(workOrder);
            return CreatedAtAction(nameof(GetWorkOrderById), new { id = workOrderId }, workOrder);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid data while creating work order.");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating the work order.");
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again.");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetWorkOrderById(long id)
    {
        try
        {
            var workOrder = await _workOrderService.GetWorkOrderByIdAsync(id);
            if (workOrder == null)
                return NotFound();

            return Ok(workOrder);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogError(ex, "WorkOrder not found.");
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching the work order.");
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetWorkOrder()
    {
        try
        {
            var workOrder = await _workOrderService.GetAllWorkOrderAsync();
            if (workOrder == null)
                return NotFound();

            return Ok(workOrder);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogError(ex, "WorkOrder not found.");
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching the work order.");
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again.");
        }
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateWorkOrderStatus(long id, [FromBody] int statusUpdate)
    {
        try
        {
            var result = await _workOrderService.UpdateWorkOrderStatusAsync(id, statusUpdate);
            if (result)
                return NoContent();

            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating work order status.");
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again.");
        }
    }

}
