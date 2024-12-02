import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MenuItemHeaderComponent } from './menuItem-header.component';

describe('MenuItemHeaderComponent', () => {
  let component: MenuItemHeaderComponent;
  let fixture: ComponentFixture<MenuItemHeaderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    imports: [MenuItemHeaderComponent],
}).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MenuItemHeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
