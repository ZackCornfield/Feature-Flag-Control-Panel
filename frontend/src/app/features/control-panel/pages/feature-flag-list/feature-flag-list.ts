import { Component, OnInit, signal } from '@angular/core';
import { FeatureFlagDto, FeatureFlagService } from '../../../../core/services/feature-flag';
import { Router } from '@angular/router';
import { KeyValuePipe } from '@angular/common';
import { Toggle } from '../../../../shared/components/toggle/toggle';

@Component({
  selector: 'app-feature-flag-list',
  imports: [KeyValuePipe, Toggle],
  templateUrl: './feature-flag-list.html',
  styleUrl: './feature-flag-list.css',
})
export class FeatureFlagList implements OnInit {
  constructor(
    private featureFlagService: FeatureFlagService,
    private router: Router,
  ) {}

  featureFlags = signal<FeatureFlagDto[]>([]);
  //Dictionary key - value
  featureFlagDictionary = signal<Record<string, boolean>>({});

  ngOnInit(): void {
    this.loadFeatureFlags();
  }

  onToggle(newState: boolean): void {
    console.log('Toggle state changed to:', newState);
  }

  loadFeatureFlags(): void {
    this.featureFlagService.getAllFeatureFlags().subscribe({
      next: (flags) => this.featureFlags.set(flags),
      error: (err) => console.error('Error loading feature flags', err),
    });
  }

  evaluateFeatureFlagDictionary(): void {
    this.featureFlagService
      .evaluateAllFeatureFlagsForUser('EAE48F79-72D7-4C4A-8045-4C866F6B3A8B', 'production')
      .subscribe({
        next: (result) => {
          this.featureFlagDictionary.set(result);
        },
        error: (err) => console.error('Error evaluating feature flags', err),
      });
  }
}
