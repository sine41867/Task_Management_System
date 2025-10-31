export enum ExecutionResultEnum {
    Success = 1,
    DuplicateResource = 2,
    ResourceNotFound = 3,
    InternalServerError = 4,
    ValidationError = 5,
    Exception = 6,
    InsufficientPermision = 7,
    RecordExist = 8,
    VersionMismatch = 9,
}

export enum TaskStatusEnum{
    Pending = 1,
    Started = 2,
    Completed = 3,
}