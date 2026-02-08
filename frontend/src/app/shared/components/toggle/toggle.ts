import { Component, input, OnInit, output, signal } from '@angular/core';

@Component({
  selector: 'app-toggle',
  imports: [],
  templateUrl: './toggle.html',
  styleUrl: './toggle.css',
})
export class Toggle implements OnInit {
  initialState = input<boolean>();
  toggleState = signal<boolean>(false);
  toggleOutput = output<boolean>();

  ngOnInit(): void {
    this.toggleState.set(this.initialState() ?? false);
  }

  onToggle(): void {
    const newState = !this.toggleState();
    this.toggleOutput.emit(newState);
    this.toggleState.set(newState);
  }
}
