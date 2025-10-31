import { Injectable } from '@angular/core';
import { BehaviorSubject, Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TaskCommunicateService {
  
  private refreshListSubject = new Subject<boolean>();
  refreshList$ = this.refreshListSubject.asObservable();

   private selectedTaskSubject = new Subject<number | null>();
   selectedTask$ = this.selectedTaskSubject.asObservable();

  triggerRefresh() {
    this.refreshListSubject.next(true);
  }

  selectTask(taskId: number) {
    this.selectedTaskSubject.next(taskId);
  }
}
