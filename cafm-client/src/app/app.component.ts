import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { WorkOrderListComponent } from "./work-order-list/work-order-list.component";

@Component({
  selector: 'app-root',
  imports: [WorkOrderListComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'cafm-client';
}
