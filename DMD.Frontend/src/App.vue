<template>
  <div id="app">
    <header>
      <h1>Система управления задачами</h1>
    </header>
    <div class="content">

      <!-- Панель для отображения дерева подзадач -->
      <div class="task-tree">
        <button @click="showCreateTaskForm = true" class="create-task-button">Создать основную задачу</button>
        <TaskList :tasks="tasks" @selectTask="handleTaskSelection" @taskUpdated="handleTaskCreated" />
      </div>

      <!-- Панель для отображения детальной информации о выбранной задаче -->
      <div class="task-details" v-if="selectedTask">
        <TaskDetails :task="selectedTask" @taskDeleted="removeTaskFromList" @taskUpdated="UpdateTaskData" />
      </div>

      <!-- Модальное окно для создания подзадачи -->
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
      this.tasks = await TaskService.getAllTasks();
      this.selectedTask = null;
    },

    async UpdateTaskData(taskId: number) {
      try {
        this.selectedTask = await TaskService.getTask(taskId);
        this.tasks = await TaskService.getAllTasks();
      } catch (error) {
        console.error('Ошибка при получении задач:', error);
      }
    },

    async handleTaskCreated() {
      this.showCreateTaskForm = false;
      await this.loadTasks();
    }
  }
});
</script>

<style scoped lang="scss">
header {
  background: linear-gradient(90deg, #ff6f61, #ff9a62);
  color: #fff;
  padding: 16px;
  text-align: center;
  font-size: 24px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  border-radius: 8px;
}

.content {
  display: flex;
  padding: 20px;
  background-color: #f4f4f9;

  .task-tree {
    flex: 1;
    background: linear-gradient(120deg, #ffecd2, #fcb69f);
    border-radius: 12px;
    padding: 20px;
    margin-right: 20px;
    box-shadow: 0 6px 20px rgba(0, 0, 0, 0.1);
    transition: box-shadow 0.3s ease;

    &:hover {
      box-shadow: 0 10px 30px rgba(0, 0, 0, 0.2);
    }

    .create-task-button {
      background: linear-gradient(90deg, #43e97b, #38f9d7);
      border: none;
      color: #fff;
      padding: 12px 24px;
      border-radius: 6px;
      cursor: pointer;
      margin-bottom: 20px;
      font-size: 16px;
      transition: background 0.3s ease;

      &:hover {
        background: linear-gradient(90deg, #38f9d7, #43e97b);
      }
    }
  }

  .task-details {
    flex: 2;
    background: linear-gradient(120deg, #d4fc79, #96e6a1);
    border-radius: 12px;
    padding: 20px;
    box-shadow: 0 6px 20px rgba(0, 0, 0, 0.1);
    transition: box-shadow 0.3s ease;

    &:hover {
      box-shadow: 0 10px 30px rgba(0, 0, 0, 0.2);
    }
  }

  .create-task-form-container {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: rgba(0, 0, 0, 0.3);
    display: flex;
    justify-content: center;
    align-items: center;

    > div {
      background: #fff;
      padding: 24px;
      border-radius: 12px;
      box-shadow: 0 8px 30px rgba(0, 0, 0, 0.2);
      width: 500px;
      transition: transform 0.3s ease;

      &:hover {
        transform: scale(1.02);
      }
    }
  }
}
</style>
