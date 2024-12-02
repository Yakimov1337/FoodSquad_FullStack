import { MenuItem } from "./food-menu-item.model";
import { User } from "./user.model";

export interface Review {
  id: number;
  userEmail: string;
  comment: string;
  rating: number;
  createdOn: Date;
  menuItemId: number;
  imageUrl: URL
}


// New ReviewSubmission model for API submission
export interface ReviewSubmission {
  comment: string;
  rating: number;
  menuItemId: number;
}
