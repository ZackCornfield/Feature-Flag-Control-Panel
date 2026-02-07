import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FeatureFlagEdit } from './feature-flag-edit';

describe('FeatureFlagEdit', () => {
  let component: FeatureFlagEdit;
  let fixture: ComponentFixture<FeatureFlagEdit>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FeatureFlagEdit]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FeatureFlagEdit);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
