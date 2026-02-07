import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FeatureFlagForm } from './feature-flag-form';

describe('FeatureFlagForm', () => {
  let component: FeatureFlagForm;
  let fixture: ComponentFixture<FeatureFlagForm>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FeatureFlagForm]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FeatureFlagForm);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
