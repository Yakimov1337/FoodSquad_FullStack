import { Store } from '@ngrx/store';
import { Component } from '@angular/core';
import { UsersTableComponent } from '../../components/users/users-table/users-table.component';
import { UserCreateModalComponent } from '../../components/users/user-create-modal/user-create-modal.component';
import { UserDeleteModalComponent } from '../../components/users/user-delete-modal/user-delete-modal.component';
import { UserUpdateModalComponent } from '../../components/users/user-update-modal/user-update-modal.component';
import {
  selectIsCreateUserModalOpen,
  selectIsDeleteUserModalOpen,
  selectIsUpdateUserModalOpen,
} from '../../../../../core/state/modal/user/modal.selectors';
import { CommonModule } from '@angular/common';
import { animate, state, style, transition, trigger } from '@angular/animations';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [
    CommonModule,
    UsersTableComponent,
    UserCreateModalComponent,
    UserDeleteModalComponent,
    UserUpdateModalComponent,
  ],
  templateUrl: './users.component.html',
  animations: [
    trigger('slideInRight', [
      transition(':enter', [
        style({ transform: 'translateX(100%)', opacity: 0 }),
        animate('1s ease-out', style({ transform: 'translateX(0)', opacity: 1 })),
      ]),
    ]),
    trigger('fadeInOut', [
      state('in', style({ opacity: 1 })),
      transition(':enter', [style({ opacity: 0 }), animate('0.5s ease-in', style({ opacity: 1 }))]),
      transition(':leave', [animate('0.5s ease-out', style({ opacity: 0 }))]),
    ]),
  ],
})
export class UsersComponent {
  showCreateUserModal$ = this.store.select(selectIsCreateUserModalOpen);
  showDeleteUserModal$ = this.store.select(selectIsDeleteUserModalOpen);
  showUpdateUserModal$ = this.store.select(selectIsUpdateUserModalOpen);

  constructor(private store: Store) {}
}
