import { Component, inject, OnDestroy, OnInit, signal, ViewEncapsulation } from '@angular/core';
import { PanelModule } from 'primeng/panel';
import { TableModule } from 'primeng/table';
import { TaskService } from '../../services/task-service';
import { TaskListDetailModel, TaskListFilterModel } from '../../models/task-model';
import { ButtonModule } from 'primeng/button';
import { SelectModule } from 'primeng/select';
import { InputTextModule } from 'primeng/inputtext';
import { DatePickerModule } from 'primeng/datepicker';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { ExecutionResultEnum, TaskStatusEnum } from '../../../../core/constants';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { DropdownOptionModel } from '../../../../core/models/dropdown-option-model';
import { ApiResponseModel } from '../../../../core/models/api-response-model';
import { TaskCommunicateService } from '../../services/task-communicate-service';
import { Subscription } from 'rxjs';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-task-list-component',
  imports: [TableModule, PanelModule, ButtonModule, SelectModule, DatePipe,
    InputTextModule, DatePickerModule, ToastModule, ReactiveFormsModule
  ],
  templateUrl: './task-list-component.html',
  styleUrl: './task-list-component.css',
  providers:[MessageService],
})
export class TaskListComponent implements OnInit, OnDestroy {

  taskService = inject(TaskService);
  messageService = inject(MessageService);
  taskCommunicateService = inject(TaskCommunicateService);

  refreshListSubscription : any;

  formBuilder = inject(FormBuilder);

  filterForm:FormGroup;

  loading = signal(false);

  taskList : TaskListDetailModel[] = [];

  totalRecords = signal(0);

  start = signal(0);
  length = signal(10);

  taskStatusList : DropdownOptionModel[] = [];

  TaskStatusEnum = TaskStatusEnum;

  constructor(){
    this.filterForm = this.formBuilder.group({
      title: [null],
      description: [null],
      currentTaskStatusId: [null],
      fromCreatedDateTime: [null],
      toCreatedDateTime: [null],
      fromDueDateTime: [null],
      toDueDateTime: [null]
    });

    this.taskStatusList = this.taskService.getTaskStatusList();
  }

  ngOnInit(): void {
    this.refreshListSubscription = this.taskCommunicateService.refreshList$.subscribe(this.loadList);
    // this.loadList();
  }

  ngOnDestroy(): void {
    this.refreshListSubscription.unsubscribe();
  }

  onFilter():void {
    this.start.set(0);
    this.loadList();
  }

  onClearFilter():void {
    this.filterForm.reset();
    this.onFilter();
  }

  onViewTask(taskId:number):void{
    this.taskCommunicateService.selectTask(taskId);
  }

  onLazyLoad(event:any){
    this.start.set(event.first);
    this.length.set(event.rows);

    this.loadList();
  }

  loadList = () => {
    this.loading.set(true);
    
    const filterModel : TaskListFilterModel = {
      ...this.filterForm.value,
      Start : this.start(),
      Length : this.length(),
    } 

    this.taskService.getTaskList(filterModel).subscribe({
      next: (response:ApiResponseModel) => {
        if(response.executionResultId == ExecutionResultEnum.Success){
          this.totalRecords.set(response.data.totalRecords);
          this.taskList = response.data.taskList;
          
        }else{
          
          this.messageService.add({ severity: 'error', summary: 'Error', detail: response.responseText });

          this.taskList = [];
          this.totalRecords.set(0);

        }
        this.loading.set(false);
      },
      error: (error) => {
      
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'An error occured while fetching the task list.' });

        this.taskList = [];
        this.totalRecords.set(0);
        this.loading.set(false);
      }
    });

  }




  
}
