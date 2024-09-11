<template>
    <div v-if="task" class="task-details-container">
        <h2>{{ task.taskName }}</h2>
        <p>{{ task.description }}</p>
        <p><strong>Статус:</strong> {{ task.status }}</p>
        <p><strong>Исполнители:</strong> {{ task.assignees }}</p>
        <p><strong>Планируемое усилие:</strong> {{ task.plannedEffort }}</p>
        <p><strong>Фактическое усилие:</strong> {{ task.actualEffort }}</p>
        <button @click="editTask" class="edit-button">Редактировать</button>
        <button @click="deleteTask" class="delete-button">Удалить</button>

        <!-- Форма для создания подзадачи -->
        <div v-if="showSubTaskForm" class="subtask-form"> <!-- скрыть в случае попытки создания подзадачи у подзадач, подзадачу может создать  только у основной задачи-->
            <h3>Создать подзадачу для "{{ task.taskName }}"</h3>
            <select v-model="newSubTask.status" class="status-select">
                <option>Назначена</option>
                <option>Выполняется</option>
                <option>Приостановлена</option>
                <option>Завершена</option>
            </select>
            <input v-model="newSubTask.taskName" placeholder="Название подзадачи" class="input-field" />
            <textarea v-model="newSubTask.description" placeholder="Описание подзадачи" class="textarea-field"></textarea>
            <button @click="createSubTask" class="create-subtask-button">Создать подзадачу</button>
        </div>
        <button v-else @click="showSubTaskForm = true" class="select-parent-button">Создать подзадачу</button>
    </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import TaskService from '@/services/TaskService';
import type { TodoTask } from '@/services/types/TodoTask';

export default defineComponent({
    name: 'TaskDetails',
    props: {
        task: {
            type: Object as () => TodoTask,
            required: true
        }
    },
    data() {
        return {
            showSubTaskForm: false, // Переменная для отображения формы
            newSubTask: {
                taskName: '',
                description: '',
                assignees: '',
                registrationDate: new Date().toISOString(),
                status: 'Назначена',
                plannedEffort: 0,
                actualEffort: 0,
                parentTaskID: this.task.id // Заполняем parentTaskID при создании подзадачи
            } as Omit<TodoTask, 'id'>
        };
    },
    methods: {
    async editTask() {
        const updatedTask = { ...this.task, taskName: 'Новое имя' }; // Пример изменений
        try {
            await TaskService.updateTask(updatedTask);
            alert('Задача обновлена');
        } catch (error) {
            console.error('Ошибка при обновлении задачи:', error);
        }
    },
    async deleteTask() {
        try {
            await TaskService.deleteTask(this.task.id);
            alert('Задача удалена');
            this.$emit('taskDeleted', this.task.id);
        } catch (error) {
            console.error('Ошибка при удалении задачи:', error);
        }
    },
    async createSubTask() {
        if (this.newSubTask.taskName.trim() === '') {
            alert('Название подзадачи не может быть пустым');
            return;
        }

        try {
            // Убираем поле id из подзадачи, так как оно автоматически генерируется сервером
            const subTask = {
                ...this.newSubTask,
                parentTaskID: this.task.id
            };

            await TaskService.createSubTask(subTask);
            alert('Подзадача создана');
            this.showSubTaskForm = false;
            this.newSubTask = {
                taskName: '',
                description: '',
                assignees: '',
                registrationDate: new Date().toISOString(),
                status: 'Назначена',
                plannedEffort: 0,
                actualEffort: 0,
                completionDate: new Date().toISOString() // Добавляем поле completionDate
            };
            this.$emit('taskUpdated');
        } catch (error) {
            console.error('Ошибка при создании подзадачи:', error);
        }
    }
}
});
</script>