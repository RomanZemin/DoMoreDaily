<template>
  <div>
    <ul>
      <!-- Рекурсивное построение дерева -->
      <task-item v-for="task in tasks" :key="task.id" :task="task" @selectTask="selectTask"
        @selectParentTask="selectParentTask" />
    </ul>

    <!-- Модальное окно для создания подзадачи -->
    <div v-if="showSubTaskForm" class="modal-overlay" @click.self="closeModal">
      <div class="modal-content">
        <CreateTaskForm :parentTask="selectedTask" @taskCreated="onTaskCreated" @closeForm="closeModal" />
      </div>
    </div>

  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import TaskItem from '@/components/TaskItem.vue';
import CreateTaskForm from '@/components/CreateTaskForm.vue';
import type { TodoTask } from '@/services/types/TodoTask';

export default defineComponent({
  name: 'TaskList',
  components: {
    TaskItem,
    CreateTaskForm
  },
  props: {
    tasks: {
      type: Array as () => TodoTask[],
      required: true
    }
  },
  data() {
    return {
      showSubTaskForm: false,
      selectedTask: null as TodoTask | null
    };
  },
  methods: {
    selectTask(task: TodoTask) {
      this.$emit('selectTask', task);
    },
    selectParentTask(task: TodoTask) {
      this.selectedTask = task;
      this.showSubTaskForm = true;
    },
    closeModal() {
      this.showSubTaskForm = false;
    },
    onTaskCreated() {
      this.showSubTaskForm = false;
      this.$emit('taskUpdated');
    }
  }
});
</script>

<style scoped lang="scss">
ul {
  list-style-type: none;
  padding: 0;
  margin: 0;

  li {
    margin-bottom: 10px;
    padding: 10px;
    background: linear-gradient(90deg, #ffecd2, #fcb69f);
    border-radius: 6px;
    transition: box-shadow 0.3s ease, transform 0.2s ease;

    &:hover {
      box-shadow: 0 6px 12px rgba(0, 0, 0, 0.1);
      transform: translateY(-2px);
    }
  }
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
    border-radius: 12px;
    max-width: 600px;
    width: 100%;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.2);
    transition: transform 0.3s ease;

    &:hover {
      transform: scale(1.02);
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