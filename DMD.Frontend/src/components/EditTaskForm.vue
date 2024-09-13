<template>
    <div class="edit-task-form">
      <h2>Редактировать задачу</h2>
      <input v-model="editedTask.taskName" placeholder="Название задачи" class="input-field" />
      <input v-model="editedTask.assignees" placeholder="Исполнитель" class="input-field" />
      <input v-model="editedTask.plannedEffort" placeholder="Планируемое усилие" class="input-field" />
      <input v-model="editedTask.actualEffort" placeholder="Фактическое усилие" class="input-field" />
      <textarea v-model="editedTask.description" placeholder="Описание задачи" class="textarea-field"></textarea>
      <div class="button-group">
        <button @click="saveTask" class="save-task-button">Сохранить изменения</button>
        <button @click="$emit('closeForm')" class="cancel-button">Отмена</button>
      </div>
    </div>
  </template>

<script lang="ts">
import { defineComponent } from 'vue';
import type { TodoTask } from '@/services/types/TodoTask';
import TaskService from '@/services/TaskService';

export default defineComponent({
    name: 'EditTaskForm',
    props: {
        task: {
            type: Object as () => TodoTask,
            required: true
        }
    },
    data() {
        return {
            editedTask: { ...this.task }
        };
    },
    methods: {
        async saveTask() {
            try {
                this.editedTask.status = this.task.status;
                await TaskService.updateTask(this.editedTask);
                this.$emit('taskUpdated', this.editedTask);
                this.$emit('closeForm');
            } catch (error) {
                console.error('Ошибка при обновлении задачи:', error);
            }
        }
    }
});
</script>

<style scoped lang="scss">
.edit-task-form {
  background: linear-gradient(135deg, #f9f9f9, #e0e0e0);
  color: #333;
  padding: 20px;
  border-radius: 12px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);

  h2 {
    margin-bottom: 20px;
    color: #333;
  }

  .input-field,
  .textarea-field {
    background-color: #fff;
    color: #333;
    border: 1px solid #ccc;
    border-radius: 6px;
    padding: 12px;
    margin-bottom: 16px;
    width: calc(100% - 24px);
    font-size: 16px;
  }

  .textarea-field {
    height: 120px;
    resize: vertical;
  }

  .button-group {
    display: flex;
    justify-content: flex-end;
    gap: 10px;

    .save-task-button,
    .cancel-button {
      border: none;
      color: #fff;
      padding: 10px 20px;
      border-radius: 6px;
      cursor: pointer;
      font-size: 16px;
      transition: background-color 0.3s;
    }

    .save-task-button {
      background-color: #4caf50;
      
      &:hover {
        background-color: #45a049;
      }
    }

    .cancel-button {
      background-color: #f44336;
      
      &:hover {
        background-color: #e53935;
      }
    }
  }
}

</style>