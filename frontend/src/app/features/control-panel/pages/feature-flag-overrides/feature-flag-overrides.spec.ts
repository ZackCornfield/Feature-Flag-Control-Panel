import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FeatureFlagOverrides } from './feature-flag-overrides';

describe('FeatureFlagOverrides', () => {
  let component: FeatureFlagOverrides;
  let fixture: ComponentFixture<FeatureFlagOverrides>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FeatureFlagOverrides]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FeatureFlagOverrides);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
