export interface TodoTask {
  id: number;
  parentTaskID: number;
  taskName: string;
  description: string;
  assignees: string;
  registrationDate: string;
  status: string;
  plannedEffort: number;
  actualEffort: number;
  subTasks?: TodoTask[]
}

