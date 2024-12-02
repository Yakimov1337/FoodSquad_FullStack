import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UsersTableItemComponent } from './users-table-item.component';

describe('UsersTableItemComponent', () => {
  let component: UsersTableItemComponent;
  let fixture: ComponentFixture<UsersTableItemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    imports: [UsersTableItemComponent],
}).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UsersTableItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
