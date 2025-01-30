import { Injectable } from '@angular/core';
import { Renderer2, Inject } from '@angular/core';
import { DOCUMENT } from '@angular/common';

@Injectable({
  providedIn: 'root',
})
export class ThemeService {
  private linkElement?: HTMLLinkElement;

  constructor(@Inject(DOCUMENT) private document: Document, private renderer: Renderer2) {}

  loadTheme(themeName: string): void {
    if (this.linkElement) {
      this.renderer.removeChild(this.document.head, this.linkElement);
    }

    this.linkElement = this.renderer.createElement('link');
    this.renderer.setAttribute(this.linkElement, 'rel', 'stylesheet');
    this.renderer.setAttribute(this.linkElement, 'href', `/themes/${themeName}-theme.css`);

    this.renderer.appendChild(this.document.head, this.linkElement);
  }
}
