import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BetaFeature } from './beta-feature';

describe('BetaFeature', () => {
  let component: BetaFeature;
  let fixture: ComponentFixture<BetaFeature>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BetaFeature]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BetaFeature);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
