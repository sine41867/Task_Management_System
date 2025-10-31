export class TaskModel{
    taskId? : number;
    title? : string;
    description? : string;
    dueDateTime?: Date;
    currentTaskStatusId? : number;
    currentTaskStatus?: string;
    nextTaskStatus?: string;
    entryVersion?:number;
    entryIdentifier?:string;
    createdDateTime?: Date;
    updatedDateTime?: Date;
    
}

export class TaskListFilterModel {
    title?: string;
    description?: string;
    fromDueDateTime?: Date;
    toDueDateTime?: Date;
    fromCreatedDateTime?: Date;
    toCreatedDateTime?: Date;
    currentTaskStatusId?: number;
}

export class TaskListDetailModel {
    taskId? : number;
    title? : string;
    description? : string;
    dueDateTime? : string;
    currentTaskStatus? : string;
    currentTaskStatusId?:number;
    createdDateTime? : string
    updatedDateTime? : string;
}

export class TaskTimeLineEventModel{
    timeStamp?:Date;
    taskStatus?:string;
}