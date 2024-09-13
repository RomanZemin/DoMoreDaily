<template>
    <li :style="{ marginLeft: `${indentLevel * 1}px` }" class="task-item">
        <span @click="selectTask(task)">
            {{ task.taskName }} ({{ task.status }})
            <button @click="selectParentTask(task)" class="add-subtask-button">+</button>
        </span>

        <!-- Отображение подзадач, если они существуют -->
        <ul v-if="task.subTasks && task.subTasks.length > 0">
            <task-item v-for="subtask in task.subTasks" :key="subtask.id" :task="subtask"
                :indent-level="indentLevel + 1" @selectTask="selectTask" @selectParentTask="selectParentTask" />
        </ul>
    </li>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import type { TodoTask } from '@/services/types/TodoTask';

export default defineComponent({
    name: 'TaskItem',
    props: {
        task: {
            type: Object as () => TodoTask,
            required: true
        },
        indentLevel: {
            type: Number,
            default: 0
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
.task-item {
  padding: 12px;
  margin-bottom: 8px;
  border-radius: 8px;
  background: linear-gradient(90deg, #ffffff, #f0f0f0);
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  color: #333;
  transition: background 0.3s ease, box-shadow 0.3s ease;

  &:last-child {
    margin-bottom: 0;
  }

  &:hover {
    background: linear-gradient(90deg, #f0f0f0, #e0e0e0);
    box-shadow: 0 6px 12px rgba(0, 0, 0, 0.15);
  }

  span {
    cursor: pointer;
    display: flex;
    align-items: center;
    font-size: 16px;
    color: #333;
    transition: color 0.3s ease;

    &:hover {
      color: #007bff;
    }
  }

  .add-subtask-button {
    background: linear-gradient(90deg, #4caf50, #81c784);
    border: none;
    color: #fff;
    padding: 6px 12px;
    border-radius: 4px;
    cursor: pointer;
    font-size: 14px;
    margin-left: 12px;
    transition: background 0.3s ease;

    &:hover {
      background: linear-gradient(90deg, #81c784, #4caf50);
    }
  }
}

ul {
  list-style-type: none;
  padding-left: 20px;
  margin: 0;
}
</style>
