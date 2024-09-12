<template>
  <div>
    <ul>
      <task-item
        v-for="task in tasks"
        :key="task.id"
        :task="task"
        @selectTask="selectTask"
        @selectParentTask="selectParentTask"
      />
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
}
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
}

.modal-content {
  background: #1e1e1e;
  padding: 20px;
  border-radius: 8px;
  max-width: 500px;
  width: 100%;
}
</style>
