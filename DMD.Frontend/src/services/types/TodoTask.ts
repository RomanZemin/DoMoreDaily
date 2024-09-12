export interface TodoTask {
  id: number;
  parentTaskID?: number; // Необязательно для подзадач
  parentTask?: string;
  taskName: string;
  description: string;
  assignees: string;
  registrationDate: string;
  status: string;
  plannedEffort: number;
  actualEffort: number;
  completionDate?: string; // Необязательно
  subTasks?: TodoTask[]
}

