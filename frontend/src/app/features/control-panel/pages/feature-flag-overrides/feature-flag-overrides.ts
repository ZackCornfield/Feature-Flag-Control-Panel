import { Component, OnInit, signal, computed } from '@angular/core';
import {
  FeatureFlagService,
  FeatureFlagOverrideDto,
  FeatureFlagDto,
  FeatureFlagToggleRequestDto,
} from '../../../../core/services/feature-flag';
import { AuthService } from '../../../../core/services/auth';
import { Toggle } from '../../../../shared/components/toggle/toggle';
import { FormsModule } from '@angular/forms';

interface FlagWithOverride {
  flag: FeatureFlagDto;
  override: FeatureFlagOverrideDto | null;
  hasOverride: boolean;
}

@Component({
  selector: 'app-feature-flag-overrides',
  imports: [Toggle, FormsModule],
  templateUrl: './feature-flag-overrides.html',
  styleUrl: './feature-flag-overrides.css',
})
export class FeatureFlagOverrides implements OnInit {
  constructor(
    private featureFlagService: FeatureFlagService,
    private authService: AuthService,
  ) {}

  featureFlags = signal<FeatureFlagDto[]>([]);
  featureFlagOverrides = signal<FeatureFlagOverrideDto[]>([]);
  searchTerm = '';
  showFilter = signal<'all' | 'overridden' | 'available'>('all');

  // Combine flags with their overrides
  flagData = computed(() => {
    const flags = this.featureFlags();
    const overrides = this.featureFlagOverrides();

    return flags.map((flag) => {
      const override = overrides.find((o) => o.featureFlagId === flag.id);
      return {
        flag,
        override: override || null,
        hasOverride: !!override,
      } as FlagWithOverride;
    });
  });

  // Filtered flag data based on search and filter
  filteredFlagData = computed(() => {
    let data = this.flagData();

    // Apply search filter
    if (this.searchTerm) {
      const search = this.searchTerm.toLowerCase();
      data = data.filter(
        (d) =>
          d.flag.key.toLowerCase().includes(search) ||
          d.flag.environment.toLowerCase().includes(search),
      );
    }

    // Apply show filter
    const filter = this.showFilter();
    if (filter === 'overridden') {
      data = data.filter((d) => d.hasOverride);
    } else if (filter === 'available') {
      data = data.filter((d) => !d.hasOverride);
    }

    return data;
  });

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.loadFeatureFlags();
    this.loadOverrides();
  }

  loadFeatureFlags(): void {
    this.featureFlagService.getAllFeatureFlags().subscribe({
      next: (flags) => this.featureFlags.set(flags),
      error: (err) => console.error('Error loading feature flags', err),
    });
  }

  loadOverrides(): void {
    const currentUser = this.authService.currentUser();
    if (!currentUser) {
      console.error('Current user is not available');
      return;
    }
    this.featureFlagService.getFeatureFlagOverrides(currentUser.id).subscribe({
      next: (overrides) => {
        this.featureFlagOverrides.set(overrides);
      },
      error: (err) => {
        console.error('Error fetching feature flag overrides:', err);
      },
    });
  }

  onSearchChange(): void {
    // Trigger recomputation
    this.featureFlags.set([...this.featureFlags()]);
  }

  onAddOverride(flag: FeatureFlagDto): void {
    const currentUser = this.authService.currentUser();
    if (!currentUser) {
      console.error('Current user is not available');
      return;
    }

    // Create override with opposite of global state by default
    const request: FeatureFlagToggleRequestDto = {
      isEnabled: !flag.isEnabled,
    };

    this.featureFlagService
      .addFeatureFlagOverrideForUser(flag.id, currentUser.id, request)
      .subscribe({
        next: () => {
          console.log(`Override added for ${flag.key}`);
          this.loadOverrides();
        },
        error: (err) => console.error('Error adding override', err),
      });
  }

  onToggleOverride(
    newState: boolean,
    flag: FeatureFlagDto,
    override: FeatureFlagOverrideDto,
  ): void {
    const currentUser = this.authService.currentUser();
    if (!currentUser) {
      console.error('Current user is not available');
      return;
    }

    const request: FeatureFlagToggleRequestDto = {
      isEnabled: newState,
    };

    this.featureFlagService
      .toggleFeatureFlagOverrideForUser(flag.id, currentUser.id, request)
      .subscribe({
        next: () => {
          console.log(`Override toggled for ${flag.key} to ${newState}`);
          this.loadOverrides();
        },
        error: (err) => console.error('Error toggling override', err),
      });
  }

  onRemoveOverride(flag: FeatureFlagDto, override: FeatureFlagOverrideDto): void {
    const currentUser = this.authService.currentUser();
    if (!currentUser) {
      console.error('Current user is not available');
      return;
    }

    this.featureFlagService.removeFeatureFlagOverrideForUser(flag.id, currentUser.id).subscribe({
      next: () => {
        console.log(`Override removed for ${flag.key}`);
        this.loadOverrides();
      },
      error: (err) => console.error('Error removing override', err),
    });
  }
}
