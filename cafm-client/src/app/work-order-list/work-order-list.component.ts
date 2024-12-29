import { Component, OnDestroy, OnInit } from '@angular/core';
import { WorkOrderService } from '../work-order.service';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-work-order-list',
  imports: [CommonModule],
  templateUrl: './work-order-list.component.html',
  styleUrl: './work-order-list.component.css'
})
export class WorkOrderListComponent implements OnInit, OnDestroy {
  companyId: number | null = null;
  locationId: number | null = null;
  workOrderUpdates: Array<any> = []; // To store received work order updates

  constructor(private route: ActivatedRoute, private workOrderService: WorkOrderService) {}

  ngOnInit(): void {
    // Fetch parameters (these values are placeholders, update as needed)
    const companyIdParam = this.route.snapshot.paramMap.get('companyId');
    const locationIdParam = this.route.snapshot.paramMap.get('locationId');

    console.log('Route Params:', this.route.snapshot.paramMap);

    if (companyIdParam && locationIdParam) {
      this.companyId = +companyIdParam; // Convert to number
      this.locationId = +locationIdParam; // Convert to number

      // Subscribe to work order updates
      this.workOrderService.subscribeToWorkOrderUpdates(this.companyId, this.locationId);

      // Listen for work order updates from SignalR
      this.workOrderService.workOrderUpdates$.subscribe((update) => {
        if (update) {
          console.log('Work Order Update Received:', update);
          this.workOrderUpdates.push(update); // Add to updates list
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