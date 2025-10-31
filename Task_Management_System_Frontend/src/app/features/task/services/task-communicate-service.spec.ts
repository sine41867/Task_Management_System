import { TestBed } from '@angular/core/testing';

import { TaskCommunicateService } from './task-communicate-service';

describe('TaskCommunicateService', () => {
  let service: TaskCommunicateService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TaskCommunicateService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
