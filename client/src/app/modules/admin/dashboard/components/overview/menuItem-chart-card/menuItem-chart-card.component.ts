import { Component, OnDestroy, OnInit, effect } from '@angular/core';
import { Subscription } from 'rxjs';
import { ThemeService } from 'src/app/services/theme.service';
import { ChartOptions } from '../../../../../../shared/models/chart-options';
import { NgApexchartsModule } from 'ng-apexcharts';
import { AngularSvgIconModule } from 'angular-svg-icon';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { OrdersService } from '../../../../../../services/orders.service';

@Component({
  selector: '[menuItem-chart-card]',
  templateUrl: './menuItem-chart-card.component.html',
  standalone: true,
  imports: [AngularSvgIconModule, NgApexchartsModule, RouterLink, CommonModule],
})
export class MenuItemChartCardComponent implements OnInit, OnDestroy {
  public chartOptions: Partial<ChartOptions>;
  private subscriptions: Subscription = new Subscription();
  public totalProfit: number = 0;
  public profitRate: number = 0;
  public profitData: { time: string, profit: number }[] = [];

  constructor(
    private themeService: ThemeService,
    private ordersService: OrdersService,
  ) {
    let baseColor = '#FFFFFF';

    this.chartOptions = {
      series: [
        {
          name: 'Profit rate',
          data: [100,200,300],
        },
      ],
      chart: {
        fontFamily: 'inherit',
        type: 'area',
        height: 150,
        toolbar: {
          show: false,
        },
        sparkline: {
          enabled: true,
        },
      },
      dataLabels: {
        enabled: false,
      },
      fill: {
        type: 'gradient',
        gradient: {
          shadeIntensity: 1,
          opacityFrom: 0.4,
          opacityTo: 0.2,
          stops: [15, 120, 100],
        },
      },
      stroke: {
        curve: 'smooth',
        show: true,
        width: 3,
        colors: [baseColor], // line color
      },
      xaxis: {
        categories: [],
        labels: {
          show: true,
          style: {
            colors: '#9aa0ac', // x-axis label color
          },
        },
        crosshairs: {
          position: 'front',
          stroke: {
            color: baseColor,
            width: 1,
            dashArray: 4,
          },
        },
        tooltip: {
          enabled: true,
        },
      },
      yaxis: {
        labels: {
          show: true,
          style: {
            colors: '#9aa0ac', // y-axis label color
          },
          formatter: function (val) {
            return val.toFixed(0); // Format y-axis values to remove decimals
          },
        },
      },
      tooltip: {
        theme: 'light',
        y: {
          formatter: function (val) {
            return val + '$';
          },
        },
      },
      colors: [baseColor], //line colors
    };

    effect(() => {
      /** change chart theme */
      let primaryColor = getComputedStyle(document.documentElement).getPropertyValue('--primary');
      primaryColor = this.HSLToHex(primaryColor);
      this.chartOptions.tooltip = {
        theme: this.themeService.theme().mode,
      };
      this.chartOptions.colors = [primaryColor];
      this.chartOptions.stroke!.colors = [primaryColor];
      this.chartOptions.xaxis!.crosshairs!.stroke!.color = primaryColor;
    });
  }

  ngOnInit(): void {
    this.fetchData();
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  private fetchData(): void {
    this.subscriptions.add(
      this.ordersService.getAllOrders().subscribe((orders) => {
        this.totalProfit = orders.reduce((sum, order) => sum + order.totalCost, 0);
        this.profitData = orders.map(order => ({
          time: new Date(order.createdOn).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }),
          profit: order.totalCost
        }));

        const categories = this.profitData.map((item) => item.time);
        const data = this.profitData.map((item) => item.profit);

        this.updateChartOptions(categories, data);
      })
    );
  }

  private updateChartOptions(categories: string[], data: number[]): void {
    this.chartOptions = {
      ...this.chartOptions,
      series: [
        {
          name: 'Profit rate',
          data: data,
        },
      ],
      xaxis: {
        ...this.chartOptions.xaxis,
        categories: categories,
        labels: {
          show: true,
          style: {
            colors: '#9aa0ac', // x-axis label color
          },
        },
      },
      yaxis: {
        ...this.chartOptions.yaxis,
        labels: {
          show: true,
          style: {
            colors: '#9aa0ac', // y-axis label color
          },
          formatter: function (val) {
            return val.toFixed(0); // Format y-axis values to remove decimals
          },
        },
      },
    };
  }

  private HSLToHex(color: string): string {
    const colorArray = color.split('%').join('').split(' ');
    const colorHSL = colorArray.map(Number);
    const hsl = {
      h: colorHSL[0],
      s: colorHSL[1],
      l: colorHSL[2],
    };

    const { h, s, l } = hsl;

    const hDecimal = l / 100;
    const a = (s * Math.min(hDecimal, 1 - hDecimal)) / 100;
    const f = (n: number) => {
      const k = (n + h / 30) % 12;
      const color = hDecimal - a * Math.max(Math.min(k - 3, 9 - k, 1), -1);

      // Convert to Hex and prefix with "0" if required
      return Math.round(255 * color)
        .toString(16)
        .padStart(2, '0');
    };
    return `#${f(0)}${f(8)}${f(4)}`;
  }
}
