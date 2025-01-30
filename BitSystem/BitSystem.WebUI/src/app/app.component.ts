import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ThemeService } from '../services/theme.service';

@Component({
  selector: 'body',
  imports: [RouterOutlet, CommonModule],
  providers: [ThemeService],
  templateUrl: './app.component.html'
})
export class AppComponent {


  constructor (private themeService: ThemeService) {
    this.themeService.loadTheme('neptune');
  }


}
