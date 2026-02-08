import { Component, OnInit, signal, computed } from '@angular/core';
import { FeatureFlagDto, FeatureFlagService } from '../../../../core/services/feature-flag';
import { Router } from '@angular/router';
import { Toggle } from '../../../../shared/components/toggle/toggle';
import { FormsModule } from '@angular/forms';

interface FlagGroup {
  name: string;
  flags: FeatureFlagDto[];
}

@Component({
  selector: 'app-feature-flag-list',
  imports: [Toggle, FormsModule],
  templateUrl: './feature-flag-list.html',
  styleUrl: './feature-flag-list.css',
})
export class FeatureFlagList implements OnInit {
  constructor(
    private featureFlagService: FeatureFlagService,
    private router: Router,
  ) {}

  featureFlags = signal<FeatureFlagDto[]>([]);
  searchTerm = '';
  groupBy = signal<'environment' | 'name'>('environment');

  // Filtered flags based on search (note to self: computed signals automatically recompute when dependencies (other signals) change)
  filteredFlags = computed(() => {
    const flags = this.featureFlags();
    if (!this.searchTerm) return flags;

    const search = this.searchTerm.toLowerCase();
    return flags.filter(
      (flag) =>
        flag.key.toLowerCase().includes(search) || flag.environment.toLowerCase().includes(search),
    );
  });

  // Grouped flags by environment or alphabetically
  groupedFlags = computed(() => {
    const flags = this.filteredFlags();
    const groups: Record<string, FeatureFlagDto[]> = {};

    if (this.groupBy() === 'environment') {
      flags.forEach((flag) => {
        if (!groups[flag.environment]) {
          groups[flag.environment] = [];
        }
        groups[flag.environment].push(flag);
      });
    } else {
      flags.forEach((flag) => {
        const firstLetter = flag.key.charAt(0).toUpperCase();
        if (!groups[firstLetter]) {
          groups[firstLetter] = [];
        }
        groups[firstLetter].push(flag);
      });
    }

    // Convert to array and sort
    return Object.entries(groups)
      .map(([name, flags]) => ({
        name,
        flags: flags.sort((a, b) => a.key.localeCompare(b.key)),
      }))
      .sort((a, b) => a.name.localeCompare(b.name));
  });

  ngOnInit(): void {
    this.loadFeatureFlags();
  }

  loadFeatureFlags(): void {
    this.featureFlagService.getAllFeatureFlags().subscribe({
      next: (flags) => this.featureFlags.set(flags),
      error: (err) => console.error('Error loading feature flags', err),
    });
  }

  onSearchChange(): void {
    // Trigger computed signal updates
    this.featureFlags.set([...this.featureFlags()]);
  }

  onFeatureFlagToggle(newState: boolean, flag: FeatureFlagDto): void {
    this.featureFlagService.toggleFeatureFlag(flag.id, { isEnabled: newState }).subscribe({
      next: (result) => {
        console.log(`Feature flag ${flag.key} toggled to ${newState}`);
        this.loadFeatureFlags();
      },
      error: (err) => console.error('Error toggling feature flag', err),
    });
  }
}
