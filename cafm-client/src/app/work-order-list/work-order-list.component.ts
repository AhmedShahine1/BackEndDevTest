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
  workOrderId: number | null = null;
  message: string | null = null;
  statusHistory: Array<any> = [];

  constructor(private route: ActivatedRoute, private workOrderService: WorkOrderService) {}

  ngOnInit(): void {
    const companyIdParam = 1;
    const locationIdParam = 1;
    console.log('Parm:', this.route.snapshot.paramMap);
    console.log('Company ID:', companyIdParam);
    console.log('Location ID:', locationIdParam);
  
    if (companyIdParam && locationIdParam) {
      this.companyId = +companyIdParam;  // Convert to number
      this.locationId = +locationIdParam;  // Convert to number
  
      // Subscribe to work order updates
      this.workOrderService.subscribeToWorkOrderUpdates(this.companyId, this.locationId);
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