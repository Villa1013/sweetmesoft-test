import { Directive, ElementRef, OnChanges, OnDestroy, inject, input } from '@angular/core';
import gsap from 'gsap';

@Directive({
  selector: '[appCountUp]',
  standalone: true,
})
export class CountUpDirective implements OnChanges, OnDestroy {
  readonly appCountUp = input.required<number>();
  readonly countUpFormat = input<'number' | 'cop'>('number');

  private readonly el = inject<ElementRef<HTMLElement>>(ElementRef);
  private readonly copFormatter = new Intl.NumberFormat('es-CO', {
    style: 'currency',
    currency: 'COP',
    maximumFractionDigits: 0,
  });

  private readonly proxy = { value: 0 };
  private tween?: gsap.core.Tween;

  ngOnChanges(): void {
    const target = this.appCountUp();
    this.tween?.kill();
    this.proxy.value = 0;
    this.tween = gsap.to(this.proxy, {
      value: target,
      duration: 0.9,
      ease: 'power2.out',
      onUpdate: () => this.render(),
      onComplete: () => this.render(),
    });
  }

  ngOnDestroy(): void {
    this.tween?.kill();
  }

  private render(): void {
    const rounded = Math.round(this.proxy.value);
    this.el.nativeElement.textContent =
      this.countUpFormat() === 'cop'
        ? this.copFormatter.format(rounded)
        : new Intl.NumberFormat('es-CO').format(rounded);
  }
}
