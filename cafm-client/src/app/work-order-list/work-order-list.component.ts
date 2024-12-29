import { Component, OnDestroy, OnInit } from '@angular/core';
import { WorkOrderService } from '../work-order.service';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-work-order-list',
  imports :[CommonModule],
  templateUrl: './work-order-list.component.html',
  styleUrls: ['./work-order-list.component.css']
})
export class WorkOrderListComponent implements OnInit, OnDestroy {
  companyId: number | null = null;
  locationId: number | null = null;
  workOrderId: number | null = null;
  message: string | null = null;
  statusHistory: Array<any> = [];

  constructor(private route: ActivatedRoute, private workOrderService: WorkOrderService) {}

  ngOnInit(): void {
    const companyIdParam = 1; // Replace with actual value if required
    const locationIdParam = 1; // Replace with actual value if required

    if (companyIdParam && locationIdParam) {
      this.companyId = +companyIdParam; // Convert to number
      this.locationId = +locationIdParam; // Convert to number

      // Subscribe to work order updates
      this.workOrderService.subscribeToWorkOrderUpdates(this.companyId, this.locationId);

      // Subscribe to the updates observable
      this.workOrderService.workOrderUpdates$.subscribe(update => {
        if (update) {
          this.workOrderId = update.workOrder?.id || null; // Assuming `id` exists in the work order object
          this.message = update.workOrder?.message || null; // Assuming `message` exists
          this.statusHistory = update.workOrder?.statusHistory || []; // Assuming `statusHistory` exists
        }
      });
    } else {
      console.error('Invalid or missing route parameters');
    }
  }

  ngOnDestroy(): void {
    // Unsubscribe when the component is destroyed
    if (this.companyId !== null && this.locationId !== null) {
      this.workOrderService.unsubscribeFromWorkOrderUpdates(this.companyId, this.locationId);
    }
  }
}
