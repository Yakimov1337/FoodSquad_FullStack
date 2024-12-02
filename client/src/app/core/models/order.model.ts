export interface Order {
  id: string;
  userEmail: string;
  menuItemQuantities: { [menuItemId: number]: number }; // Object with menuItemId as key and quantity as value
  totalCost: number;
  createdOn: Date;
  paid: boolean;
  status: 'PENDING' | 'COMPLETED' | 'CANCELLED';
}

// New OrderSubmission model for API submission
export interface OrderSubmission {
  userEmail: string;
  menuItemQuantities: { [menuItemId: number]: number }; // Object with menuItemId as key and quantity as value
  createdOn: string;
  paid: boolean;
  status: 'PENDING' | 'COMPLETED' | 'CANCELLED';
}
