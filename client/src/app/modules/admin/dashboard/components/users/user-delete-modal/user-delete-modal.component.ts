import { Subscription } from 'rxjs';
import { Component } from '@angular/core';
import { Store } from '@ngrx/store';
import { CommonModule } from '@angular/common';
import { UserService } from '../../../../../../services/user.service';
import { selectDeleteUserId } from '../../../../../../core/state/modal/user/modal.selectors';
import { closeDeleteUserModal } from '../../../../../../core/state/modal/user/modal.actions';
import { ToastrService } from 'ngx-toastr';
@Component({
  selector: '[user-delete-modal]',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './user-delete-modal.component.html',
})
export class UserDeleteModalComponent {
  userIdToDelete: string | undefined | null = null;
  private subscription = new Subscription();

  constructor(private store: Store, private userService: UserService, private toastr: ToastrService) {
    this.subscription.add(
      this.store.select(selectDeleteUserId).subscribe((userId) => {
        this.userIdToDelete = userId;
      }),
    );
  }
  ngOnInit(): void {
    console.log(this.userIdToDelete);
  }

  deleteUser(): void {
    if (!this.userIdToDelete) {
      console.error('No user ID provided for deletion.');
      return;
    }
    this.userService.deleteUser(this.userIdToDelete).subscribe({
      next: () => {
        this.toastr.success(`User with ID ${this.userIdToDelete} deleted successfully.`);
        this.closeModal();
        this.userService.userDeleted(this.userIdToDelete);
      },
      error: (error) =>  this.toastr.error('Error ocurred while deleting this user!',error)
    });
  }

  closeModal(): void {
    this.store.dispatch(closeDeleteUserModal());
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
