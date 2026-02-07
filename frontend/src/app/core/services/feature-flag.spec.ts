import { TestBed } from '@angular/core/testing';

import { FeatureFlag } from './feature-flag';

describe('FeatureFlag', () => {
  let service: FeatureFlag;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(FeatureFlag);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
