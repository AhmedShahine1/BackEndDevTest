import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class WorkOrderService {
  private hubConnection: HubConnection;
  private workOrderUpdatesSource = new BehaviorSubject<any>(null);
  workOrderUpdates$ = this.workOrderUpdatesSource.asObservable();

  private isConnected = false; // Track connection state

  constructor() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl('https://localhost:7097/workOrderHub') // Replace with your SignalR hub URL
      .build();

    this.setupListeners();
    this.startConnection();
  }

  // Establish connection with SignalR server
  private startConnection(): void {
    this.hubConnection
      .start()
      .then(() => {
        console.log('SignalR connection established');
        this.isConnected = true; // Set connection state to true
      })
      .catch(err => {
        console.error('Error starting SignalR connection: ', err);
        this.isConnected = false;
      });

    // Handle reconnection attempts
    this.hubConnection.onclose(() => {
      console.log('SignalR connection closed');
      this.isConnected = false;
      this.startConnection(); // Attempt reconnection
    });
  }

  // Setup listeners for incoming messages
  private setupListeners(): void {
    this.hubConnection.on('ReceiveWorkOrderUpdate', (workOrder: any) => {
      console.log(`Received update for WorkOrder:`, workOrder);
    });    
  }

  // Subscribe to work order updates by company and location
  subscribeToWorkOrderUpdates(companyId: number, locationId: number): void {
    if (this.isConnected) {
      this.hubConnection
        .invoke('SubscribeToWorkOrderUpdates', companyId, locationId)
        .catch(err => console.error('Error subscribing to work order updates: ', err));
    } else {
      console.log('Connection not established. Retrying...');
      setTimeout(() => this.subscribeToWorkOrderUpdates(companyId, locationId), 1000); // Retry after 1 second
    }
  }

  // Unsubscribe from work order updates by company and location
  unsubscribeFromWorkOrderUpdates(companyId: number, locationId: number): void {
    if (this.isConnected) {
      this.hubConnection
        .invoke('UnsubscribeFromWorkOrderUpdates', companyId, locationId)
        .catch(err => console.error('Error unsubscribing from work order updates: ', err));
    } else {
      console.log('Connection not established. Retrying...');
      setTimeout(() => this.unsubscribeFromWorkOrderUpdates(companyId, locationId), 1000); // Retry after 1 second
    }
  }
}
