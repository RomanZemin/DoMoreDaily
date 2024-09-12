<template>
    <div class="edit-task-form">
        <h2>Редактировать задачу</h2>
        <input v-model="editedTask.taskName" placeholder="Название задачи" class="input-field" />
        <input v-model="editedTask.assignees" placeholder="Исполнитель" class="input-field" />
        <input v-model="editedTask.plannedEffort" placeholder="Планируемое усилие" class="input-field" />
        <input v-model="editedTask.actualEffort" placeholder="Фактическое усилие" class="input-field" />
        <textarea v-model="editedTask.description" placeholder="Описание задачи" class="textarea-field"></textarea>
        <select v-model="editedTask.status" class="status-select">
            <option>Назначена</option>
            <option>Выполняется</option>
            <option>Приостановлена</option>
            <option>Завершена</option>
        </select>
        <p>
            <button @click="saveTask" class="save-task-button">Сохранить изменения</button>
            <button @click="$emit('closeForm')" class="cancel-button">Отмена</button>
        </p>
    </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import type { TodoTask } from '@/services/types/TodoTask';
import TaskService from '@/services/TaskService';

export default defineComponent({
    name: 'EditTaskForm',
    props: {
        task: {
            type: Object as () => TodoTask,
            required: true
        }
    },
    data() {
        return {
            editedTask: { ...this.task } // Создаем копию задачи для редактирования
        };
    },
    methods: {
        async saveTask() {
            try {
                await TaskService.updateTask(this.editedTask);
                this.$emit('taskUpdated', this.editedTask); // Оповещаем родителя о том, что задача обновлена
                this.$emit('closeForm'); // Закрываем форму
            } catch (error) {
                console.error('Ошибка при обновлении задачи:', error);
            }
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

.save-task-button,
.cancel-button {
    background-color: #4caf50;
    border: none;
    color: #fff;
    padding: 8px 16px;
    border-radius: 4px;
    cursor: pointer;
    margin-right: 10px;
}

.save-task-button:hover {
    background-color: #45a049;
}

.cancel-button {
    background-color: #f44336;
}

.cancel-button:hover {
    background-color: #e53935;
}
</style>