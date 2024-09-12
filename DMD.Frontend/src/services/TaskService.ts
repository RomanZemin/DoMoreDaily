import axios from 'axios';
import type { TodoTask } from './types/TodoTask';

const API_URL = 'http://localhost:5108/api/Task';

export default {
  async getAllTasks(): Promise<TodoTask[]> {
    const response = await axios.get(API_URL);
    console.log(response.data)
    return response.data;
  },

  async getTask(id: number): Promise<TodoTask> {
    const response = await axios.get(`${API_URL}/${id}`);
    return response.data;
  },

  async createTask(task: TodoTask): Promise<void> {
    const response = await axios.post(API_URL, task);
    return response.data;
  },

  async createSubTask(subTask: Omit<TodoTask, 'id'>): Promise<void> {
    await axios.post(API_URL, subTask);
  },

  async updateTask(task: TodoTask): Promise<void> {
    const response = await axios.put(`${API_URL}/${task.id}`, task);
    return response.data;
  },

  async deleteTask(id: number): Promise<void> {
    const response = await axios.delete(`${API_URL}/${id}`);
    return response.data;
  },

  async changeTaskStatus(id: number, status: string): Promise<void> {
    const response = await axios.patch(`${API_URL}/${id}/status`, status, {
      headers: { 'Content-Type': 'application/json' }
    });
    return response.data;
  }
};
