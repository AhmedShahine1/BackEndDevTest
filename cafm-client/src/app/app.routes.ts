import { Routes } from '@angular/router';
import { WorkOrderListComponent } from './work-order-list/work-order-list.component';

export const routes: Routes = [
    {
        path: 'workorder/:Companyid/:LocationId',
        component: WorkOrderListComponent
      },
];
