<template>
    <li :style="{ marginLeft: `${indentLevel * 20}px` }" class="task-item">
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
    padding: 8px;
    border-bottom: 1px solid #333;
    color: #e0e0e0;

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