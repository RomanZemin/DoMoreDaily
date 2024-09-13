<template>
    <div v-if="task" class="task-details-container">
      <!-- Информация о задаче -->
      <p v-if="task.parentTaskID"><strong>Главная задача: </strong> {{ ParentTaskName }}</p>
      <h2>Название: {{ task.taskName }}</h2>
      <p><strong>Описание: </strong>{{ task.description }}</p>
      <strong>Статус: </strong>
      <select class="status-select" v-model="task.status" @change="onStatusChange">
        <option>Назначена</option>
        <option>Выполняется</option>
        <option>Приостановлена</option>
        <option>Завершена</option>
      </select>
      <p><strong>Исполнители:</strong> {{ task.assignees }}</p>
      <p><strong>Планируемое усилие:</strong> {{ task.plannedEffort }} часов</p>
      <p><strong>Фактическое усилие:</strong> {{ task.actualEffort }} часов</p>
      <p><strong>Дата назначения задачи:</strong> {{ task.registrationDate }}</p>
      <button @click="showEditTaskForm = true" class="edit-button">Редактировать</button>
      <button @click="deleteTask" class="delete-button">Удалить</button>
      <p v-if="errorMessage" class="error-message">{{ errorMessage }}</p>
  
      <!-- Модальное окно для редактирования задачи -->
      <div v-if="showEditTaskForm" class="modal-overlay" @click.self="closeModal">
        <div class="modal-content">
          <EditTaskForm :task="task" @closeForm="closeModal" @taskUpdated="UpdateTask" />
        </div>
      </div>
    </div>
  </template>
  
  <script lang="ts">
  import { defineComponent } from 'vue';
  import TaskService from '@/services/TaskService';
  import type { TodoTask } from '@/services/types/TodoTask';
  import EditTaskForm from '@/components/EditTaskForm.vue';
  
  export default defineComponent({
    name: 'TaskDetails',
    props: {
      task: {
        type: Object as () => TodoTask,
        required: true
      }
    },
    components: {
      EditTaskForm
    },
    data() {
      return {
        showEditTaskForm: false,
        ParentTaskName: 'Undefined',
        errorMessage: '',
      }
    },
    async created() {
      await this.GetNameParentTask();
    },
    watch: {
      task: {
        handler: async function () {
          if (this.task.parentTaskID)
            await this.GetNameParentTask();
        },
        immediate: true
      }
    },
    methods: {
      closeModal() {
        this.showEditTaskForm = false;
      },
      UpdateTask() {
        this.$emit('taskUpdated', this.task.id);
      },
      async GetNameParentTask() {
        try {
          const response = await TaskService.getTask(this.task.parentTaskID);
          this.ParentTaskName = response.taskName;
        } catch (error) {
          console.error('Ошибка при поиске названия главной задачи:', error);
        }
      },
      async deleteTask() {
        try {
          await TaskService.deleteTask(this.task.id);
          this.$emit('taskDeleted', this.task.id);
        } catch (error) {
          console.error('Ошибка при удалении задачи:', error);
        }
      },
      async onStatusChange(event: Event) {
        this.errorMessage = '';
        try {
          await TaskService.changeTaskStatus(this.task.id, this.task.status);
        } catch (error: any) {
          this.task.status = await (await TaskService.getTask(this.task.id)).status
  
          if (error.response && error.response.status === 400) {
            this.errorMessage = error.response.data;
          } else {
            console.error('Ошибка при изменении статуса задачи:', error);
          }
          return;
        }
        this.$emit('taskUpdated', this.task.id);
      }
    }
  });
  </script>
  
  <style scoped lang="scss">
  .task-details-container {
    background: linear-gradient(180deg, #ffffff, #f0f0f0);
    padding: 20px;
    border-radius: 8px;
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
    color: #333;
    font-family: Arial, sans-serif;
    max-width: 600px;
    margin: auto;
  
    h2 {
      margin-top: 0;
      color: #333;
    }
  
    .status-select {
      background: linear-gradient(90deg, #ffffff, #e0e0e0);
      color: #333;
      border: 1px solid #ccc;
      border-radius: 4px;
      padding: 8px;
      margin-bottom: 12px;
      transition: background 0.3s ease;
      
      &:hover {
        background: linear-gradient(90deg, #f5f5f5, #e0e0e0);
      }
    }
  
    .edit-button,
    .delete-button {
      background: linear-gradient(90deg, #4caf50, #81c784);
      border: none;
      color: #fff;
      padding: 10px 20px;
      border-radius: 4px;
      cursor: pointer;
      font-size: 14px;
      margin-right: 10px;
      transition: background 0.3s ease, transform 0.2s ease;
  
      &:hover {
        background: linear-gradient(90deg, #81c784, #4caf50);
        transform: scale(1.02);
      }
    }
  
    .delete-button {
      background: linear-gradient(90deg, #f44336, #e53935);
      
      &:hover {
        background: linear-gradient(90deg, #e53935, #f44336);
      }
    }
  
    .error-message {
      color: #d76666;
      font-weight: bold;
    }
  
    .modal-overlay {
      position: fixed;
      top: 0;
      left: 0;
      width: 100%;
      height: 100%;
      background-color: rgba(0, 0, 0, 0.3);
      display: flex;
      align-items: center;
      justify-content: center;
      animation: fadeIn 0.3s ease;
  
      .modal-content {
        padding: 1px;
        border-radius: 8px;
        max-width: 600px;
        width: 100%;
        box-shadow: 0 8px 16px rgba(0, 0, 0, 0.2);
        transition: transform 0.3s ease;
        
        &:hover {
          transform: scale(1.02);
        }
      }
    }
  }
  
  @keyframes fadeIn {
    from {
      opacity: 0;
    }
    to {
      opacity: 1;
    }
  }
  </style>
  