<template>
    <div v-if="task" class="task-details-container">
        <h2>{{ task.taskName }}</h2>
        <p>{{ task.description }}</p>
        <p><strong>Статус:</strong> {{ task.status }}</p>
        <p><strong>Исполнители:</strong> {{ task.assignees }}</p>
        <p><strong>Планируемое усилие:</strong> {{ task.plannedEffort }}</p>
        <p><strong>Фактическое усилие:</strong> {{ task.actualEffort }}</p>
        <button @click="showEditTaskForm = true" class="edit-button">Редактировать</button>
        <button @click="deleteTask" class="delete-button">Удалить</button>

        <!-- Модальное окно для редактирования задачи -->
        <div v-if="showEditTaskForm" class="modal-overlay" @click.self="closeModal">
            <div class="modal-content">
                <EditTaskForm :task="task" @closeForm="closeModal" @taskUpdated="UpdateTask"/>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import TaskService from '@/services/TaskService';
import type { TodoTask } from '@/services/types/TodoTask';
import EditTaskForm from '@/components/EditTaskForm.vue';

export default defineComponent({
    name: 'TaskDetails',
    props: {
        task: {
            type: Object as () => TodoTask,
            required: true
        }
    },
    components: {
        EditTaskForm
    },
    data() {
        return {
            showEditTaskForm: false, // Переменная для отображения модального окна
        };
    },
    methods: {
        closeModal() {
            this.showEditTaskForm = false; // Закрытие модального окна
        },
        UpdateTask(){
            this.$emit('taskUpdated', this.task.id);
        },

        async deleteTask() {
            try {
                await TaskService.deleteTask(this.task.id);
                //alert('Задача удалена');
                this.$emit('taskDeleted', this.task.id);
            } catch (error) {
                console.error('Ошибка при удалении задачи:', error);
            }
        }
    }
});
</script>

<style scoped>
.edit-button,
.delete-button {
    background-color: #3760e7f3;
    border: none;
    color: #fff;
    padding: 8px 16px;
    border-radius: 4px;
    cursor: pointer;
    margin-right: 10px;
}

.delete-task-button:hover {
    background-color: #45a049;
}

.delete-button {
    background-color: #f44336;
}

.delete-button:hover {
    background-color: #e53935;
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
