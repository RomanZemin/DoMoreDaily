<template>
    <div class="create-task-form">
        <h2 v-if="parentTask">Создать подзадачу</h2>
        <h2 v-else>Создать основную задачу</h2>
        <h4 v-if="parentTask">К задаче:</h4>
        <input v-if="parentTask" v-model="parentTask.taskName" readonly class="input-field" />
        <input v-model="newTask.taskName" :placeholder="parentTask ? 'Название подзадачи' : 'Название задачи'"
            class="input-field" />
        <input v-model="newTask.assignees" placeholder="Исполнитель" class="input-field" />
        <input v-model="newTask.plannedEffort" placeholder="Планируемое усилие" class="input-field" />
        <textarea v-model="newTask.description" placeholder="Описание задачи" class="textarea-field"></textarea>
        <select v-model="newTask.status" class="status-select">
            <option>Назначена</option>
            <option>Выполняется</option>
            <option>Приостановлена</option>
            <option>Завершена</option>
        </select>
        <div class="button-group">
            <button @click="createTask" class="create-task-button">Создать задачу</button>
            <button @click="closeForm" class="cancel-button">Отмена</button>
        </div>
    </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import TaskService from '@/services/TaskService';
import type { TodoTask } from '@/services/types/TodoTask';

export default defineComponent({
    name: 'CreateTaskForm',
    props: {
        parentTask: {
            type: Object as () => TodoTask | null,
            default: null
        }
    },
    data() {
        return {
            newTask: {
                parentTaskID: this.parentTask ? this.parentTask.id : null,
                taskName: '',
                description: '',
                assignees: '',
                registrationDate: new Date().toISOString(),
                plannedEffort: 0,
                actualEffort: 0,
                status: 'Назначена'
            } as TodoTask
        };
    },
    methods: {
        async createTask() {
            if (!this.newTask.taskName.trim()) {
                alert('Название задачи не может быть пустым');
                return;
            }

            try {
                await TaskService.createTask(this.newTask);
                this.$emit('taskCreated');
            } catch (error) {
                console.error('Ошибка при создании задачи:', error);
            }
        },
        closeForm() {
            this.$emit('closeForm');
        }
    }
});
</script>


<style scoped lang="scss">
.create-task-form {
    background: linear-gradient(135deg, #f5f5f5, #e0e0e0);
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

        .create-task-button,
        .cancel-button {
            border: none;
            color: #fff;
            padding: 10px 20px;
            border-radius: 6px;
            cursor: pointer;
            font-size: 16px;
            transition: background-color 0.3s;
        }

        .create-task-button {
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

    .status-select {
        background: #fff;
        color: #333;
        border: 1px solid #ccc;
        border-radius: 4px;
        padding: 8px;
        margin-bottom: 12px;
        transition: background 0.3s ease;

        &:hover {
            background: #f5f5f5;
        }
    }
}

</style>