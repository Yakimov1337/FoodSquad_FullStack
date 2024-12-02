import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MenuItemSingleCardComponent } from './menuItem-single-card.component';

describe('MenuItemSingleCardComponent', () => {
  let component: MenuItemSingleCardComponent;
  let fixture: ComponentFixture<MenuItemSingleCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    imports: [MenuItemSingleCardComponent],
}).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MenuItemSingleCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
