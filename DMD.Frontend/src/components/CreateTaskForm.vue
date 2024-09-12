<template>
    <div class="create-task-form">
        <h2 v-if="parentTask">Создать подзадачу</h2>
        <h2 v-else>Создать основную задачу</h2>
        <h4 v-if="parentTask">К задаче:</h4>
        <input v-if="parentTask" v-model="parentTask.taskName" readonly class="input-field" />
        <input v-if="parentTask" v-model="newTask.taskName" placeholder="Название подзадачи" class="input-field" />
        <input v-else v-model="newTask.taskName" placeholder="Название задачи" class="input-field" />
        <input v-model="newTask.assignees" placeholder="Исполнитель" class="input-field" />
        <input v-model="newTask.plannedEffort" placeholder="Планируемое усилие" class="input-field" />
        <textarea v-model="newTask.description" placeholder="Описание задачи" class="textarea-field"></textarea>
        <select v-model="newTask.status" class="status-select">
            <option>Назначена</option>
            <option>Выполняется</option>
            <option>Приостановлена</option>
            <option>Завершена</option>
        </select>
        <p>
            <button @click="createTask" class="create-task-button">Создать задачу</button>
            <button @click="closeForm" class="cancel-button">Отмена</button>
        </p>
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
                parentTaskID: this.parentTask ? this.parentTask.id : null, // Привязываем ID родительской задачи
                taskName: '',
                description: '',
                assignees: '',
                registrationDate: new Date().toISOString(),
                status: 'Назначена',
                plannedEffort: 0,
                actualEffort: 0,
                completionDate: new Date().toISOString()
            } as Omit<TodoTask, 'id'>
        };
    },
    methods: {
        async createTask() {
            if (this.newTask.taskName.trim() === '') {
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
    background-color: #1e1e1e;
    color: #e0e0e0;
    padding: 16px;
    border-radius: 8px;
}

.input-field,
.textarea-field {
    background-color: #333;
    color: #e0e0e0;
    border: 1px solid #444;
    border-radius: 4px;
    padding: 8px;
    margin-bottom: 16px;
    width: 97%;
}

.textarea-field {
    height: 100px;
    resize: vertical;
}

.status-select {
    background-color: #333;
    color: #e0e0e0;
    border: 1px solid #444;
    border-radius: 4px;
    padding: 8px;
    margin-bottom: 2px;
}

.create-task-button,
.cancel-button {
    background-color: #4caf50;
    border: none;
    color: #fff;
    padding: 8px 16px;
    border-radius: 4px;
    cursor: pointer;
    margin-right: 10px;
}

.create-task-button:hover {
    background-color: #45a049;
}

.cancel-button {
    background-color: #f44336;
}

.cancel-button:hover {
    background-color: #e53935;
}
</style>