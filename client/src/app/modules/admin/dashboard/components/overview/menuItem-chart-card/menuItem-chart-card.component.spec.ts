import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MenuItemChartCardComponent } from './menuItem-chart-card.component';

describe('MenuItemChartCardComponent', () => {
  let component: MenuItemChartCardComponent;
  let fixture: ComponentFixture<MenuItemChartCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    imports: [MenuItemChartCardComponent],
}).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MenuItemChartCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
