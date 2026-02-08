import { Component, OnInit, signal, computed } from '@angular/core';
import {
  FeatureFlagService,
  FeatureFlagOverrideDto,
  FeatureFlagDto,
  FeatureFlagToggleRequestDto,
} from '../../../../core/services/feature-flag';
import { Toggle } from '../../../../shared/components/toggle/toggle';
import { FormsModule } from '@angular/forms';

interface OverrideWithFlag {
  data: FeatureFlagOverrideDto;
  flag: FeatureFlagDto;
}

@Component({
  selector: 'app-feature-flag-overrides',
  imports: [Toggle, FormsModule],
  templateUrl: './feature-flag-overrides.html',
  styleUrl: './feature-flag-overrides.css',
})
export class FeatureFlagOverrides implements OnInit {
  constructor(private featureFlagService: FeatureFlagService) {}

  featureFlags = signal<FeatureFlagDto[]>([]);
  allOverrides = signal<FeatureFlagOverrideDto[]>([]);
  searchTerm = '';
  statusFilter = signal<'all' | 'enabled' | 'disabled'>('all');

  // Modal state
  showAddModal = signal(false);
  selectedFlagForOverride = signal<FeatureFlagDto | undefined>(undefined);
  flagSearchTerm = '';
  newUserId = '';
  newOverrideState = false;

  // Combine overrides with their flag details
  overridesWithFlags = computed(() => {
    const flags = this.featureFlags();
    const overrides = this.allOverrides();

    return overrides
      .map((override) => {
        const flag = flags.find((f) => f.id === override.featureFlagId);
        return {
          data: override,
          flag: flag!,
        } as OverrideWithFlag;
      })
      .filter((o) => o.flag); // Filter out any overrides without matching flags
  });

  // Filtered overrides based on search and status
  filteredOverrides = computed(() => {
    let data = this.overridesWithFlags();

    // Apply search filter
    if (this.searchTerm) {
      const search = this.searchTerm.toLowerCase();
      data = data.filter(
        (o) =>
          o.flag.key.toLowerCase().includes(search) ||
          o.flag.environment.toLowerCase().includes(search) ||
          o.data.userId.toLowerCase().includes(search),
      );
    }

    // Apply status filter
    const filter = this.statusFilter();
    if (filter === 'enabled') {
      data = data.filter((o) => o.data.isEnabled);
    } else if (filter === 'disabled') {
      data = data.filter((o) => !o.data.isEnabled);
    }

    return data;
  });

  // Filtered flags for the add modal dropdown
  filteredFlags = computed(() => {
    const flags = this.featureFlags();
    if (!this.flagSearchTerm) return [];

    const search = this.flagSearchTerm.toLowerCase();
    return flags.filter(
      (f) => f.key.toLowerCase().includes(search) || f.environment.toLowerCase().includes(search),
    );
  });

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.loadFeatureFlags();
    this.loadAllOverrides();
  }

  loadFeatureFlags(): void {
    this.featureFlagService.getAllFeatureFlags().subscribe({
      next: (flags) => this.featureFlags.set(flags),
      error: (err) => console.error('Error loading feature flags', err),
    });
  }

  loadAllOverrides(): void {
    this.featureFlagService.getFeatureFlagOverrides().subscribe({
      next: (overrides) => this.allOverrides.set(overrides),
      error: (err) => console.error('Error loading overrides', err),
    });
  }

  onSearchChange(): void {
    this.featureFlags.set([...this.featureFlags()]);
  }

  onFlagSearchChange(): void {
    this.featureFlags.set([...this.featureFlags()]);
  }

  openAddOverrideModal(): void {
    this.showAddModal.set(true);
    this.flagSearchTerm = '';
    this.newUserId = '';
    this.newOverrideState = false;
    this.selectedFlagForOverride.set(undefined);
  }

  closeAddModal(): void {
    this.showAddModal.set(false);
    this.selectedFlagForOverride.set(undefined);
    this.flagSearchTerm = '';
    this.newUserId = '';
    this.newOverrideState = false;
  }

  selectFlag(flag: FeatureFlagDto): void {
    this.selectedFlagForOverride.set(flag);
    this.flagSearchTerm = '';
  }

  onAddOverride(): void {
    const flag = this.selectedFlagForOverride();
    if (!flag || !this.newUserId.trim()) return;

    const request: FeatureFlagToggleRequestDto = {
      isEnabled: this.newOverrideState,
    };

    this.featureFlagService
      .addFeatureFlagOverrideForUser(flag.id, this.newUserId.trim(), request)
      .subscribe({
        next: () => {
          console.log(`Override added for ${flag.key} for user ${this.newUserId}`);
          this.loadAllOverrides();
          this.closeAddModal();
        },
        error: (err) => console.error('Error adding override', err),
      });
  }

  onToggleOverride(newState: boolean, override: OverrideWithFlag): void {
    const request: FeatureFlagToggleRequestDto = {
      isEnabled: newState,
    };

    this.featureFlagService
      .toggleFeatureFlagOverrideForUser(override.flag.id, override.data.userId, request)
      .subscribe({
        next: () => {
          console.log(`Override toggled for ${override.flag.key} to ${newState}`);
          this.loadAllOverrides();
        },
        error: (err) => console.error('Error toggling override', err),
      });
  }

  onDeleteOverride(override: OverrideWithFlag): void {
    if (!confirm(`Delete override for ${override.flag.key} (user: ${override.data.userId})?`)) {
      return;
    }

    this.featureFlagService
      .removeFeatureFlagOverrideForUser(override.flag.id, override.data.userId)
      .subscribe({
        next: () => {
          console.log(`Override removed for ${override.flag.key}`);
          this.loadAllOverrides();
        },
        error: (err) => console.error('Error removing override', err),
      });
  }
}
