import { Component, OnInit, signal } from '@angular/core';
import { FeatureFlagService } from '../../../../core/services/feature-flag';
import { AuthService } from '../../../../core/services/auth';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';

interface FlagStatus {
  key: string;
  isEnabled: boolean;
}

@Component({
  selector: 'app-dashboard',
  imports: [RouterLink],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard implements OnInit {
  constructor(
    private featureFlagService: FeatureFlagService,
    private authService: AuthService,
  ) {}

  // Individual feature flags
  newDashboardUI = signal(false);
  advancedAnalytics = signal(false);
  betaFeatures = signal(false);
  darkMode = signal(false);

  // Overall status for display
  featureFlagStatus = signal<FlagStatus[]>([]);

  ngOnInit(): void {
    this.loadFeatureFlags();
  }

  loadFeatureFlags(): void {
    const currentUser = this.authService.currentUser();
    const userId = currentUser?.id || 'EAE48F79-72D7-4C4A-8045-4C866F6B3A8B';
    const environment = 'production';

    // Define the flags we want to check
    const flagsToCheck = ['NEW_DASHBOARD_UI', 'ADVANCED_ANALYTICS', 'BETA_FEATURES', 'DARK_MODE'];

    // Create observables for each flag
    const flagObservables = flagsToCheck.map((key) =>
      this.featureFlagService.evaluateFeatureFlag(key, userId, environment),
    );

    // Execute all flag checks in parallel
    forkJoin(flagObservables).subscribe({
      next: (results) => {
        // Update individual signals
        this.newDashboardUI.set(results[0]);
        this.advancedAnalytics.set(results[1]);
        this.betaFeatures.set(results[2]);
        this.darkMode.set(results[3]);

        // Update status array for display
        const statuses: FlagStatus[] = flagsToCheck.map((key, index) => ({
          key,
          isEnabled: results[index],
        }));
        this.featureFlagStatus.set(statuses);
      },
      error: (err) => {
        console.error('Error loading feature flags:', err);
        // Set all to false on error
        this.featureFlagStatus.set(flagsToCheck.map((key) => ({ key, isEnabled: false })));
      },
    });
  }
}
