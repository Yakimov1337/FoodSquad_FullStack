import { Observable, Subscription, interval, startWith, switchMap } from 'rxjs';
import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { MenuItemAuctionsTableItemComponent } from '../menuItem-auctions-table-item/menuItem-auctions-table-item.component';
import { CommonModule, NgFor } from '@angular/common';
import { MenuItemsService } from '../../../../../../services/menuItems.service';
import { MenuItem, PaginatedResponseDTO } from '../../../../../../core/models';
import { LoaderComponent } from '../../../../../../shared/components/loader/loader.component';
import { AngularSvgIconModule } from 'angular-svg-icon';
import { ButtonComponent } from '../../../../../../shared/components/button/button.component';
import { openCreateMenuItemModal } from '../../../../../../core/state/modal/menuItem/modal.actions';
import { Store } from '@ngrx/store';
import { PaginationComponent } from '../../../../../../shared/components/pagination/pagination.component';
import { ToastrService } from 'ngx-toastr';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: '[menuItem-auctions-table]',
  templateUrl: './menuItem-auctions-table.component.html',
  standalone: true,
  imports: [
    NgFor,
    AngularSvgIconModule,
    ButtonComponent,
    MenuItemAuctionsTableItemComponent,
    CommonModule,
    LoaderComponent,
    PaginationComponent,
    FormsModule,
  ],
})
export class MenuItemAuctionsTableComponent implements OnInit {
  public menuItems: MenuItem[] = [];
  public isLoading: boolean = true;
  public currentPage = 1;
  public totalPages!: number;
  public timeSinceLastUpdate$!: Observable<number>;
  public lastUpdated: Date = new Date();
  public categoryFilter: string = '';
  public priceSortDirection: string = '';
  public defaultFilter: string = 'all';
  public originalMenuItems: MenuItem[] = [];
  selectedItemIds: Set<number> = new Set<number>();

  private subscriptions: Subscription = new Subscription();

  constructor(
    private menuItemsService: MenuItemsService,
    private store: Store,
    private toastr: ToastrService,
    private route: ActivatedRoute,
    private router: Router,
    private cdRef: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe((params) => {
      this.currentPage = +params['page'] || 1;
      this.categoryFilter = params['category'] || '';
      this.priceSortDirection = params['sort'] || '';
      this.defaultFilter = params['default'] || 'all';

      this.loadMenuItems(this.currentPage);
    });

    this.initializeSubscriptions();
    this.initializeTimeSinceLastUpdate();
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  loadMenuItems(
    page: number,
    limit: number = 10,
    categoryFilter: string = this.categoryFilter,
    isDefault: string = this.defaultFilter,
    priceSortDirection: string = this.priceSortDirection,
  ): void {
    this.isLoading = true;

    this.menuItemsService.getAllMenuItems(page, limit, categoryFilter, isDefault, priceSortDirection).subscribe({
      next: (response: PaginatedResponseDTO<MenuItem>) => {
        this.originalMenuItems = response.items;
        this.menuItems = [...this.originalMenuItems]; // Copy for display purposes
        this.totalPages = Math.ceil(response.totalCount / limit);
        this.isLoading = false;
      },
      error: (error) => {
        this.toastr.error('Error fetching items:', error);
        this.isLoading = false;
      },
    });

    // Update URL query parameters
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { page, category: categoryFilter, sort: priceSortDirection, default: isDefault },
      queryParamsHandling: 'merge',
    });
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.loadMenuItems(this.currentPage);
  }

  openCreateModal() {
    console.log('Dispatching openCreateOrderModal action');
    this.store.dispatch(openCreateMenuItemModal());
  }

  private initializeTimeSinceLastUpdate(): void {
    this.timeSinceLastUpdate$ = interval(60000) // Emit value every 60 seconds
      .pipe(
        startWith(0), // Start immediately upon subscription
        switchMap(() => {
          const now = new Date();
          const difference = now.getTime() - this.lastUpdated.getTime();
          return [Math.floor(difference / 60000)]; // Convert to minutes
        }),
      );
  }

  private initializeSubscriptions(): void {
    const menuItemsCreatedSub = this.menuItemsService.menuItemCreated$.subscribe((menuItem) => {
      if (menuItem) {
        this.loadMenuItems(this.currentPage);
        // this.menuItems.unshift(menuItem);
      }
    });

    const menuItemsUpdatedSub = this.menuItemsService.menuItemUpdated$.subscribe((updatedMenuItem) => {
      if (updatedMenuItem) {
        this.loadMenuItems(this.currentPage);
        // const index = this.menuItems.findIndex(menuItem => menuItem.id === updatedMenuItem.id);
        // if (index !== -1) {
        //   this.menuItems[index] = updatedMenuItem; // Update the menu item in the list
        // }
      }
    });

    const menuItemsDeletedSub = this.menuItemsService.menuItemDeleted$.subscribe((deletedMenuItemId) => {
      this.menuItems = this.menuItems.filter((item) => item.id !== deletedMenuItemId);
    });

    this.subscriptions.add(menuItemsCreatedSub);
    this.subscriptions.add(menuItemsUpdatedSub);
    this.subscriptions.add(menuItemsDeletedSub);
  }

  applyFiltersAndSorting(): void {
    this.loadMenuItems(this.currentPage, 10, this.categoryFilter, this.defaultFilter, this.priceSortDirection);
  }

  isSelected(id: number): boolean {
    return this.selectedItemIds.has(id);
  }

  onToggleSelection(id: number): void {
    if (this.selectedItemIds.has(id)) {
      this.selectedItemIds.delete(id);
    } else {
      this.selectedItemIds.add(id);
    }
  }

  toggleAllSelection(): void {
    const allSelected = this.isAllSelected();
    console.log('Current selection state:', allSelected ? 'All Selected' : 'Not All Selected');

    if (allSelected) {
      console.log('Deselecting all items');
      this.selectedItemIds.clear();
    } else {
      console.log('Selecting all items');
      this.menuItems.forEach(item => this.selectedItemIds.add(item.id));
    }

    console.log('Selected item IDs:', Array.from(this.selectedItemIds));
     this.cdRef.detectChanges();
  }

  isAllSelected(): boolean {
    return this.menuItems.length > 0 && this.menuItems.every((item) => this.selectedItemIds.has(item.id));
  }

  trackById(index: number, item: MenuItem): number {
    return item.id;
  }

  getSelectedItemIds(): number[] {
    return Array.from(this.selectedItemIds);  // Converts Set<number> to number[]
  }

  // Function to delete all selected items
  deleteAllItems(): void {
    const idsToDelete = Array.from(this.selectedItemIds);
    if (idsToDelete.length === 0) {
      // No items selected, show error
      this.toastr.error('Please select at least one item to delete.');
      return;
    }

    // Items are selected, proceed with deletion
    this.menuItemsService.deleteMenuItemsByIds(idsToDelete).subscribe({
      next: () => {
        this.toastr.success('All selected items deleted successfully');
        this.selectedItemIds.clear();
        this.loadMenuItems(this.currentPage);
      },
      error: (error) => {
        this.toastr.error('Error deleting items:', error);
      }
    });
  }


}
