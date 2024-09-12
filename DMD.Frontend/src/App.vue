<template>
  <div id="app">
    <header>
      <h1>Система управления задачами</h1>
    </header>
    <div class="content">
      <div class="task-tree">
        <button @click="showCreateTaskForm = true" class="create-task-button">Создать основную задачу</button>
        <TaskList :tasks="tasks" @selectTask="handleTaskSelection" @selectParentTask="handleParentTaskSelection" />
      </div>
      <div class="task-details" v-if="selectedTask">
        <TaskDetails :task="selectedTask" @taskDeleted="removeTaskFromList" />
      </div>
      <div v-if="showCreateTaskForm" class="create-task-form-container">
        <CreateTaskForm @taskCreated="handleTaskCreated" @closeForm="showCreateTaskForm = false" />
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import TaskList from './components/TaskList.vue';
import TaskDetails from './components/TaskDetails.vue';
import CreateTaskForm from './components/CreateTaskForm.vue';
import TaskService from './services/TaskService';
import type { TodoTask } from './services/types/TodoTask';

export default defineComponent({
  name: 'App',
  components: {
    TaskList,
    TaskDetails,
    CreateTaskForm
  },
  data() {
    return {
      tasks: [] as TodoTask[],
      selectedTask: null as TodoTask | null,
      showCreateTaskForm: false // Для отображения формы создания задачи
    };
  },
  async created() {
    await this.loadTasks();
  },
  methods: {
    async loadTasks() {
      try {
        this.tasks = await TaskService.getAllTasks();
      } catch (error) {
        console.error('Ошибка при получении задач:', error);
      }
    },
    handleTaskSelection(task: TodoTask) {
      this.selectedTask = task;
    },
    async removeTaskFromList(taskId: number) {
      this.tasks = this.tasks.filter(task => task.id !== taskId);
      this.selectedTask = null;
    },
    async handleTaskCreated() {
      this.showCreateTaskForm = false;
      await this.loadTasks();
    },
    handleParentTaskSelection(task: TodoTask) {
      this.selectedTask = task;
    }
  }
});
</script>

<style scoped lang="scss">
header {
  background-color: #1f1f1f;
  color: #e0e0e0;
  padding: 16px;
  text-align: center;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.3);
}

.content {
  display: flex;
  padding: 20px;

  .create-task-button {
    background-color: #4caf50;
    border: none;
    color: #fff;
    padding: 8px 16px;
    border-radius: 4px;
    cursor: pointer;
    margin-bottom: 20px;
    font-size: 16px;
  }

  .create-task-button:hover {
    background-color: #45a049;
  }

  .create-task-form-container {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: rgba(0, 0, 0, 0.5);
    display: flex;
    justify-content: center;
    align-items: center;
  }

  .create-task-form-container > div {
    background: #1e1e1e;
    padding: 20px;
    border-radius: 8px;
    width: 500px;
  }

  .task-tree {
    flex: 1;
    background-color: #1e1e1e;
    border-radius: 8px;
    padding: 20px;
    margin-right: 20px;
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.3);
  }

  .task-details {
    flex: 2;
    background-color: #1e1e1e;
    border-radius: 8px;
    padding: 20px;
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.3);
  }
}
</style>
