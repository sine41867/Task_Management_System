import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../environments/environments';
import { HttpClient } from '@angular/common/http';
import { ApiResponseModel } from '../../../core/models/api-response-model';
import { Observable } from 'rxjs';
import { TaskListFilterModel, TaskModel } from '../models/task-model';
import { DropdownOptionModel } from '../../../core/models/dropdown-option-model';
import { TaskStatusEnum } from '../../../core/constants';

@Injectable({
  providedIn: 'root'
})
export class TaskService {

  private baseUrl = `${environment.apiUrl}/Task`;

  http = inject(HttpClient);

  constructor() { }

  getTaskList(filterModel: TaskListFilterModel): Observable<ApiResponseModel> {
    return this.http.post<ApiResponseModel>(`${this.baseUrl}/GetTaskList`, filterModel);
  }

  getTaskStatusList(): DropdownOptionModel[] {
    const list: DropdownOptionModel[] = [
      new DropdownOptionModel({ Id: TaskStatusEnum.Started, Text: 'Started' }),
      new DropdownOptionModel({ Id: TaskStatusEnum.Pending, Text: 'Pending' }),
      new DropdownOptionModel({ Id: TaskStatusEnum.Completed, Text: 'Completed' })
    ];

    return list;
  }

  saveTask(taskModel: TaskModel): Observable<ApiResponseModel> {
    return this.http.post(`${this.baseUrl}/CreateTask`, taskModel);
  }

  getTaskById(taskId:number):Observable<ApiResponseModel>{
    return this.http.get<ApiResponseModel>(`${this.baseUrl}/GetTaskById?taskId=${taskId}`);
  }

  deleteTask(taskModel:TaskModel):Observable<ApiResponseModel> {
    return this.http.post(`${this.baseUrl}/DeleteTask`, taskModel);
  }

  changeTaskStatus(taskModel:TaskModel):Observable<ApiResponseModel> {
    return this.http.post(`${this.baseUrl}/ChangeTaskStatus`, taskModel);
  }
}
