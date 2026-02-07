import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FeatureFlagList } from './feature-flag-list';

describe('FeatureFlagList', () => {
  let component: FeatureFlagList;
  let fixture: ComponentFixture<FeatureFlagList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FeatureFlagList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FeatureFlagList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
