import { Component, computed, inject, signal } from '@angular/core';
import { NgClass } from '@angular/common';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { TooltipModule } from 'primeng/tooltip';
import { ThemeService } from '../../core/services/theme.service';
import { Footer } from '../../shared/ui';

interface NavItem {
  label: string;
  icon: string;
  route: string;
}

@Component({
  selector: 'app-shell',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, ButtonModule, TooltipModule, NgClass, Footer],
  templateUrl: './shell.html',
  styleUrl: './shell.scss',
})
export class Shell {
  protected readonly temas = inject(ThemeService);
  protected readonly collapsed = signal(false);
  protected readonly mobileOpen = signal(false);
  protected readonly mostrarTexto = computed(() => this.mobileOpen() || !this.collapsed());

  protected readonly nav: NavItem[] = [
    { label: 'Panel', icon: 'pi-th-large', route: '/panel' },
    { label: 'Solicitudes', icon: 'pi-inbox', route: '/solicitudes' },
    { label: 'Cotizaciones', icon: 'pi-file-edit', route: '/cotizaciones' },
    { label: 'Tarifas', icon: 'pi-tags', route: '/tarifas' },
  ];

  toggle(): void {
    this.collapsed.update((v) => !v);
  }

  toggleMobile(): void {
    this.mobileOpen.update((v) => !v);
  }

  cerrarMobile(): void {
    this.mobileOpen.set(false);
  }
}
