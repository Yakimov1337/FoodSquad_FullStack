import { Component, OnInit, OnDestroy } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable, Subscription, interval, startWith, switchMap } from 'rxjs';
import { UserService } from '../../../../../../services/user.service';
import { User } from '../../../../../../core/models';
import { openCreateUserModal } from '../../../../../../core/state/modal/user/modal.actions';
import { CommonModule, NgFor } from '@angular/common';
import { AngularSvgIconModule } from 'angular-svg-icon';
import { UsersTableItemComponent } from '../users-table-item/users-table-item.component';
import { LoaderComponent } from '../../../../../../shared/components/loader/loader.component';
import { ButtonComponent } from '../../../../../../shared/components/button/button.component';
import { PaginationComponent } from '../../../../../../shared/components/pagination/pagination.component';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: '[users-table]',
  templateUrl: './users-table.component.html',
  standalone: true,
  imports: [
    CommonModule,
    NgFor,
    AngularSvgIconModule,
    ButtonComponent,
    UsersTableItemComponent,
    LoaderComponent,
    PaginationComponent,
  ],
})
export class UsersTableComponent implements OnInit, OnDestroy {
  public users: User[] = [];
  public isLoading = true;
  public currentPage = 1;
  public totalPages!: number;
  private lastUpdated = new Date();
  public timeSinceLastUpdate$!: Observable<number>;

  private subscriptions = new Subscription();

  constructor(private store: Store, private userService: UserService,private toastr: ToastrService) {}

  ngOnInit(): void {
    this.loadUsers(this.currentPage);
    this.initializeSubscriptions();
    this.initializeTimeSinceLastUpdate();
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  loadUsers(page: number, limit: number = 10): void {
    this.isLoading = true;
    this.userService.getAllUsers(page, limit).subscribe({
      next: (users) => {
        this.users = users;
        this.totalPages = Math.ceil(50 / limit); //change later
        this.isLoading = false;
      },
      error: (error) => {
        this.toastr.error('Error fetching Users:', error);
        this.isLoading = false;
      },
    });
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.loadUsers(this.currentPage);
  }

  openCreateModal(): void {
    console.log('Dispatching openCreateUserModal action');
    this.store.dispatch(openCreateUserModal());
  }

  private initializeTimeSinceLastUpdate(): void {
    this.timeSinceLastUpdate$ = interval(60000).pipe(
      startWith(0),
      switchMap(() => {
        const now = new Date();
        const difference = now.getTime() - this.lastUpdated.getTime();
        return [Math.floor(difference / 60000)]; // Convert to minutes
      }),
    );
  }

  private initializeSubscriptions(): void {
    const userCreatedSub = this.userService.userCreated$.subscribe((user) => {
      if (user) {
        this.loadUsers(this.currentPage); // Refetch users after a new user is created
      }
    });

    const userUpdatedSub = this.userService.userUpdated$.subscribe((user) => {
      if (user) {
        this.loadUsers(this.currentPage); // Refetch users after a user update
      }
    });

    const userDeletedSub = this.userService.userDeleted$.subscribe((userId) => {
      if (userId) {
        this.users = this.users.filter((user) => user.id !== userId);
      }
    });

    this.subscriptions.add(userCreatedSub);
    this.subscriptions.add(userUpdatedSub);
    this.subscriptions.add(userDeletedSub);
  }
}
