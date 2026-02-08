import { Component, OnInit, Input, Output, EventEmitter, input, output } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { FeatureFlagDto, FeatureFlagRequestDto } from '../../../../core/services/feature-flag';

@Component({
  selector: 'app-feature-flag-modal',
  imports: [ReactiveFormsModule],
  templateUrl: './feature-flag-modal.html',
  styleUrl: './feature-flag-modal.css',
})
export class FeatureFlagModal implements OnInit {
  flag = input<FeatureFlagDto | null>(null);
  save = output<FeatureFlagRequestDto>();
  close = output<void>();

  flagForm!: FormGroup;
  isEditMode = false;

  constructor(private fb: FormBuilder) {}

  ngOnInit(): void {
    const flagValue = this.flag();
    this.isEditMode = !!flagValue;

    this.flagForm = this.fb.group({
      key: [flagValue?.key || '', [Validators.required, Validators.minLength(2)]],
      environment: [flagValue?.environment || '', Validators.required],
      isEnabled: [flagValue?.isEnabled ?? false],
    });
  }

  onSubmit(): void {
    if (this.flagForm.valid) {
      const formValue = this.flagForm.getRawValue(); // getRawValue includes disabled fields
      const request: FeatureFlagRequestDto = {
        key: formValue.key,
        environment: formValue.environment,
        isEnabled: formValue.isEnabled,
      };
      this.save.emit(request);
    }
  }

  onClose(): void {
    this.close.emit();
  }
}
