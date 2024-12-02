import { MenuItem } from '../models/nav-menu-item.model';

export class Menu {
  public static pages: MenuItem[] = [
    {
      group: 'Dashboard',
      separator: true,
      items: [
        {
          icon: 'assets/icons/heroicons/outline/chart-pie.svg',
          label: 'Dashboard',
          route: '/admin/dashboard',
          children: [
            { label: 'Overview', route: '/admin/dashboard' },
          ],
        },
      ],
    },
    {
      group: 'Entities',
      separator: true,
      items: [
        {
          icon: 'assets/icons/heroicons/outline/gift.svg',
          label: 'Menu Items',
          route: '/admin/items',
        },
        {
          icon: 'assets/icons/heroicons/outline/download.svg',
          label: 'Orders',
          route: '/admin/orders',
        },
        {
          icon: 'assets/icons/heroicons/outline/users.svg',
          label: 'Users',
          route: '/admin/users',
        },
        {
          icon: 'assets/icons/heroicons/outline/star.svg',
          label: 'Reviews',
          route: '/admin/reviews',
        },
      ],
    },   {
      group: 'Pages',
      separator: false,
      items: [
        {
          icon: 'assets/icons/heroicons/outline/bell.svg',
          label: 'Home',
          route: '/',
        },
        {
          icon: 'assets/icons/heroicons/outline/cog.svg',
          label: 'Account Settings',
          route: '/profile',
        },
      ],
    },
  ];
}
