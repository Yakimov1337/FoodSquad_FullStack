import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AngularSvgIconModule } from 'angular-svg-icon';
import { Store, select } from '@ngrx/store';
import { User } from '../../../../../../core/models';
import { ButtonComponent } from '../../../../../../shared/components/button/button.component';
import { openDeleteUserModal, openUpdateUserModal } from '../../../../../../core/state/modal/user/modal.actions';
import { Observable } from 'rxjs';
import { selectCurrentUser } from '../../../../../../core/state/auth/auth.selectors';
@Component({
  selector: '[users-table-item]',
  templateUrl: './users-table-item.component.html',
  standalone: true,
  imports: [CommonModule, AngularSvgIconModule, ButtonComponent],
})
export class UsersTableItemComponent implements OnInit {
  @Input() user: User = <User>{};
  currentUser$: Observable<User | null>;

  constructor(private store: Store) {
    this.currentUser$ = this.store.pipe(select(selectCurrentUser));
  }

  ngOnInit(): void {}

  handleButtonClick(currentUser: User, user: User): void {
    // Admin can update and delete, Moderator cannot update delete anyone and cannot update Admins
    if (currentUser.role === 'Admin' || (currentUser.role === 'Moderator' && user.role !== 'Admin')) {
      this.openUpdateModal();
    }
  }
  openUpdateModal() {
    this.store.dispatch(openUpdateUserModal({ user: this.user }));
  }

  openDeleteModal() {
    this.store.dispatch(openDeleteUserModal({ userId: this.user.id }));
  }
}
