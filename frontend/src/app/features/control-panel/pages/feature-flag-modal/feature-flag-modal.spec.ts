import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FeatureFlagModal } from './feature-flag-modal';

describe('FeatureFlagModal', () => {
  let component: FeatureFlagModal;
  let fixture: ComponentFixture<FeatureFlagModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FeatureFlagModal],
    }).compileComponents();

    fixture = TestBed.createComponent(FeatureFlagModal);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
