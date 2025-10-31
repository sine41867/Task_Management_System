import { Component } from '@angular/core';
import { TaskListComponent } from "../../components/task-list-component/task-list-component";
import { TaskFormComponent } from "../../components/task-form-component/task-form-component";
import { TopNavBarComponent } from '../../../../shared/components/top-nav-bar-component/top-nav-bar-component';

@Component({
  selector: 'app-task-page-component',
  imports: [TaskListComponent, TaskFormComponent, TopNavBarComponent],
  templateUrl: './task-page-component.html',
  styleUrl: './task-page-component.css'
})
export class TaskPageComponent {

}
