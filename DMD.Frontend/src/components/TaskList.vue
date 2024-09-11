<template>
  <div>
    <ul>
      <li v-for="task in tasks" :key="task.id" class="task-item">
        <span @click="selectTask(task)">
          {{ task.taskName }} ({{ task.status }})
          <button v-if="task.parentTaskID == null" @click="selectParentTask(task)" class="add-subtask-button">+</button>
        </span>
        <TaskList v-if="task.subTasks && task.subTasks.length" :tasks="task.subTasks" @selectTask="selectTask" @selectParentTask="selectParentTask" />
      </li>
    </ul>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import type { TodoTask } from '@/services/types/TodoTask';

export default defineComponent({
  name: 'TaskList',
  props: {
    tasks: {
      type: Array as () => TodoTask[],
      required: true
    }
  },
  methods: {
    selectTask(task: TodoTask) {
      this.$emit('selectTask', task);
    },
    selectParentTask(task: TodoTask) {
      this.$emit('selectParentTask', task);
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

.task-item {
  padding: 8px;
  border-bottom: 1px solid #333;
  color: #e0e0e0;
  display: flex;
  justify-content: space-between;
  align-items: center;

  &:last-child {
    border-bottom: none;
  }

  span {
    cursor: pointer;
    display: flex;
    align-items: center;
  }

  .add-subtask-button {
    background-color: #ff5722;
    border: none;
    color: #fff;
    padding: 4px 8px;
    border-radius: 4px;
    cursor: pointer;
    font-size: 14px;
    margin-left: 8px;

    &:hover {
      background-color: #e64a19;
    }
  }
}
</style>
