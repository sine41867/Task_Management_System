import { Component, inject, OnDestroy, OnInit, signal, ViewEncapsulation } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { FluidModule } from 'primeng/fluid';
import { InputTextModule } from 'primeng/inputtext';
import { PanelModule } from 'primeng/panel';
import { TextareaModule } from 'primeng/textarea';
import { TimelineModule } from 'primeng/timeline';
import { DividerModule } from 'primeng/divider';
import { TaskService } from '../../services/task-service';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DatePickerModule } from 'primeng/datepicker';
import { TaskModel, TaskTimeLineEventModel } from '../../models/task-model';
import { ExecutionResultEnum, TaskStatusEnum } from '../../../../core/constants';
import { ApiResponseModel } from '../../../../core/models/api-response-model';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { v4 as uuid } from 'uuid';
import { TaskCommunicateService } from '../../services/task-communicate-service';
import { DatePipe } from '@angular/common';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { EventType } from '@angular/router';

@Component({
  selector: 'app-task-form-component',
  imports: [
    InputTextModule, TextareaModule, PanelModule, DatePickerModule, ToastModule,
    FluidModule, ButtonModule, TimelineModule, DividerModule, ReactiveFormsModule,
    DatePipe, ProgressSpinnerModule, ConfirmDialogModule
  ],
  templateUrl: './task-form-component.html',
  styleUrl: './task-form-component.css',
  providers: [MessageService, ConfirmationService],
})
export class TaskFormComponent implements OnInit, OnDestroy {

  taskService = inject(TaskService);
  messageService = inject(MessageService);
  confirmationService = inject(ConfirmationService);
  taskComminicateService = inject(TaskCommunicateService);

  selectedTaskSubscription: any;

  formBuilder = inject(FormBuilder);

  taskForm: FormGroup;

  taskId = signal(null);
  isEditMode = signal(false);

  isLoading = signal(false);
  isDeleting = signal(false);
  isSaving = signal(false);

  taskViewModel: TaskModel = new TaskModel();

  timelineEvents: TaskTimeLineEventModel[] = [];

  
  TaskStatusEnum = TaskStatusEnum;

  constructor() {
    this.taskForm = this.formBuilder.group({
      title: [null, Validators.required],
      description: [null],
      dueDateTime: [null, Validators.required],
      entryIdentifier: [uuid()],
      entryVersion: [null]
    });

  }

  ngOnInit(): void {
    this.selectedTaskSubscription = this.taskComminicateService.selectedTask$.subscribe((taskId) => this.loadTask(taskId))
  }

  ngOnDestroy(): void {
    this.selectedTaskSubscription.unsubscribe();
  }

  onEditTask(): void {
    this.isEditMode.set(true);
  }

  onCancelEdit(): void {
    this.isEditMode.set(false);
    this.taskForm.patchValue({
      title: this.taskViewModel.title,
      description: this.taskViewModel.description,
      dueDateTime: new Date(this.taskViewModel.dueDateTime as Date),
    })
  }

  onNewTask(): void {
    this.isEditMode.set(true);
    this.taskId.set(null);
    this.taskForm.reset({ entryIdentifier: uuid() });
    this.taskViewModel = new TaskModel();
  }

  onSubmitTask(): void {
    if (this.taskForm.valid) {
      this.isSaving.set(true);

      const taskModel: TaskModel = {
        ...this.taskForm.value,
        taskId: this.taskId()
      }
      console.log(taskModel);
      this.taskService.saveTask(taskModel).subscribe({
        next: (response: ApiResponseModel) => {
          this.taskComminicateService.triggerRefresh();
          this.isSaving.set(false);
          this.messageService.add({ severity: 'success', summary: 'Success', detail: response.responseText });

          if (this.taskId() == null) {
            this.taskForm.reset({ entryIdentifier: uuid() });
          } else {
            this.loadTask(this.taskId());
          }
        },
        error: (err) => {
          this.isSaving.set(false);
          this.messageService.add({ severity: 'error', summary: 'Error', detail: err.error?.responseText ?? 'An error occurred while saving the task.' });
        }
      })
    } else {
      this.taskForm.markAllAsTouched();
      this.taskForm.markAllAsDirty();
    }
  }

  onDeleteTask(event: Event): void {

    this.confirmationService.confirm({
      target: event.target as EventTarget,
      message: 'Are you sure to delete this task?',
      header: 'Confirm',
      icon: 'pi pi-info-circle',
      rejectLabel: 'No',
      rejectButtonProps: {
        label: 'No',
        severity: 'secondary',
        outlined: true,
      },
      acceptButtonProps: {
        label: 'Delete',
        severity: 'danger',
      },

      accept: () => {
        this.isDeleting.set(true);

        const obj: TaskModel = {
          taskId: this.taskId() as unknown as number,
          entryVersion: this.taskForm.get("entryVersion")?.value,
        }
        this.taskService.deleteTask(obj).subscribe({
          next: (response: ApiResponseModel) => {
            this.taskComminicateService.triggerRefresh();
            this.onNewTask();
            this.messageService.add({ severity: 'success', summary: 'Success', detail: response.responseText });
            this.isDeleting.set(false);
          },
          error: (err) => {
            this.isDeleting.set(false);
            this.messageService.add({ severity: 'error', summary: 'Error', detail:  err.error?.responseText ?? "An error occured while deleting the task. Please try again." });
          }
        })
      },

    });


  }

  onChangeTaskStatus(event: Event): void {

    this.confirmationService.confirm({
      target: event.target as EventTarget,
      message: `Are you sure to mark this task as ${this.taskViewModel.nextTaskStatus}?`,
      header: 'Confirm',
      icon: 'pi pi-info-circle',
      closeOnEscape:true,
      closable:true,
      rejectLabel: 'No',
      rejectButtonProps: {
        label: 'No',
        severity: 'secondary',
        outlined: true,
      },
      acceptButtonProps: {
        label: 'Yes',
        severity: 'primary',
      },

      accept: () => {
        this.isLoading.set(true);

        const obj: TaskModel = {
          taskId: this.taskId() as unknown as number,
          entryVersion: this.taskForm.get("entryVersion")?.value,
        }
        this.taskService.changeTaskStatus(obj).subscribe({
          next: (response: ApiResponseModel) => {
            this.taskComminicateService.triggerRefresh();
            this.loadTask(this.taskId());
            this.messageService.add({ severity: 'success', summary: 'Success', detail: response.responseText });
            this.isLoading.set(false);
          },
          error: (err) => {
            this.isLoading.set(false);
            this.messageService.add({ severity: 'error', summary: 'Error', detail: err.error?.responseText ?? `An error occured while mark the task as ${this.taskViewModel.nextTaskStatus}. Please try again.` });
          }
        })
      },

    });


  }

  loadTask(taskId: number | null): void {
    if (taskId != null) {

      this.isLoading.set(true);

      this.taskService.getTaskById(taskId).subscribe({
        next: (response: ApiResponseModel) => {
          this.isEditMode.set(false);
          this.taskId.set(response.data.taskId);

          this.taskForm.patchValue({
            title: response.data.title,
            description: response.data.description,
            dueDateTime: new Date(response.data.dueDateTime),
            entryIdentifier: response.data.entryIdentifier,
            entryVersion: response.data.entryVersion
          })

          this.taskViewModel = response.data;
          this.timelineEvents = response.data.taskStatusChangeList;
          this.isLoading.set(false);
        },
        error: (err) => {
          this.messageService.add({ severity: 'error', summary: 'Error', detail: err.error?.responseText ?? 'An error occurred while loading the task.' });
        }
      })
    }


  }

}
