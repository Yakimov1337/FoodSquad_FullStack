
import { MenuItem } from './food-menu-item.model';
import { Order } from './order.model';

export interface User {
  id: string;
  name: string;
  email: string;
  imageUrl: URL;
  phoneNumber: string;
  role: Role;
  ordersCount?: number;
}

export interface UserResponse extends User {

}

export interface UserClaims {
  id: string;
  name: string;
  email: string;
  imageUrl: URL;
  phoneNumber: string;
  role: Role;
}
export interface AuthState {
  isAuthenticated: boolean;
  user: User | null;
  error: string | null;
  isLoading: boolean;
  isUpdatingInfo: boolean;
}

export interface UserUpdate {
  name: string;
  imageUrl: URL;
  phoneNumber: string;
  role: string;
}


export enum Role {
  Normal = 'Normal',
  Moderator = 'Moderator',
  Admin = 'Admin',
  SuperAdmin = 'Super Admin',
}
