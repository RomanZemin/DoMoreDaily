# DoMoreDaily
<strong>DoMoreDaily</strong> - система управления задачами, позволяющая эффективно организовывать и отслеживать задачи и подзадачи. Проект включает фронтенд на Vue.js и бэкенд на C#, а также использует PostgreSQL в качестве базы данных.

<h2>Структура проекта</h2>

<h3>Backend</h3>
Проект на C# с использованием EntityFramework Core и Clean Architecture. Структура проекта:
<ul>
  <li>DMD.Backend/DMD.Core/DMD.Application - Приложение и бизнес-логика.</li>
  <li>DMD.Backend/DMD.Core/DMD.Domain - Доменная модель.</li>
  <li>DMD.Backend/DMD.Infrastructure/DMD.Persistence - Доступ к данным.</li>
  <li>DMD.Backend/DMD.Presentation/DMD.WebAPI - Web API.</li>
  <li>DMD.Backend/DMD.Tests/DMD.Presentation.Tests - Unit тесты.</li>
</ul>
<p align="center">
  <img width="638" height="657" src="https://i.imgur.com/5vMXImJ.png">
</p>

<h3>Frontend</h3>
Проект на Vue.js. Структура проекта:
<ul>
  <li>src/App.vue - Главный компонент приложения.</li>
  <li>src/components/CreateTaskForm.vue - Компонент для создания задач.</li>
  <li>src/components/EditTaskForm.vue - Компонент для редактирования задач.</li>
  <li>src/components/TaskDetails.vue - Компонент для отображения деталей задачи.</li>
  <li>src/components/TaskList.vue - Компонент для отображения списка задач.</li>
  <li>src/components/TaskItem.vue - Компонент для отображения одной задачи.</li>
  <li>src/services/TaskService.ts - Сервис для работы с API задач.</li>
  <li>src/services/types/TodoTask.ts - Типы для задач.</li>
</ul>
<p align="center">
  <img width="770" height="657" src="https://i.imgur.com/xUQFhGd.png">
</p>
<p align="center">
  <img width="770" height="657" src="https://i.imgur.com/PvgFKv5.png">
</p>

<h2>Запуск проекта</h2>
<h3>Frontend</h3>
<p>1. Убедитесь, что у вас установлен Node.js.</p>
<p>2. Перейдите в директорию фронтенда и выполните следующие команды:</p>

```bash
npm install
npm run dev
```
<h3>Backend</h3>
<p>1. Убедитесь, что у вас установлен SDK .NET 8.</p>
<p>2. Клонируйте или загрузите этот репозиторий на ваш локальный компьютер.</p>
<p>3. Откройте решение DMD.Backend\DMD.WebAPI\DMD.Backend.sln в Visual Studio или другой IDE, поддерживающей .NET.</p>
<p>4. Выполните команды для установки зависимостей и запуска:</p>

```bash
dotnet restore
dotnet build
dotnet run
```
<p>
5. Настройте необходимые параметры подключения к базе данных в файле appsettings.json проекта Infrastructure.</p>

<h3>Установка базы данных</h3>
Если у вас возникают проблемы с установкой базы данных, которая лежит в репозитории, используйте следующий SQL-код для создания базы данных и таблиц:

```sql
-- Table: public.Tasks

CREATE TABLE IF NOT EXISTS public."Tasks"
(
    "Id" integer NOT NULL DEFAULT nextval('tasks_taskid_seq'::regclass),
    "ParentTaskID" integer,
    "TaskName" character varying(255) COLLATE pg_catalog."default" NOT NULL,
    "Description" text COLLATE pg_catalog."default",
    "Assignees" text COLLATE pg_catalog."default",
    "RegistrationDate" timestamp without time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "Status" character varying(50) COLLATE pg_catalog."default" NOT NULL,
    "PlannedEffort" numeric(10,2) DEFAULT 0,
    "ActualEffort" numeric(10,2) DEFAULT 0,
    "CompletionDate" timestamp without time zone,
    CONSTRAINT tasks_pkey PRIMARY KEY ("Id"),
    CONSTRAINT fk_tasks_parenttask FOREIGN KEY ("ParentTaskID")
        REFERENCES public."Tasks" ("Id") MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE,
    CONSTRAINT tasks_status_check CHECK ("Status"::text = ANY (ARRAY['Назначена'::character varying, 'Выполняется'::character varying, 'Приостановлена'::character varying, 'Завершена'::character varying]::text[]))
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public."Tasks"
    OWNER to postgres;

-- Triggers

CREATE OR REPLACE TRIGGER trg_taskstatusupdate
    AFTER UPDATE 
    ON public."Tasks"
    FOR EACH ROW
    EXECUTE FUNCTION public.update_subtasks_status();

CREATE OR REPLACE TRIGGER trg_update_parent_effort_after_delete
    AFTER DELETE
    ON public."Tasks"
    FOR EACH ROW
    WHEN (old."ParentTaskID" IS NOT NULL)
    EXECUTE FUNCTION public.update_parent_task_effort();

CREATE OR REPLACE TRIGGER trg_update_parent_effort_after_insert
    AFTER INSERT
    ON public."Tasks"
    FOR EACH ROW
    WHEN (new."ParentTaskID" IS NOT NULL)
    EXECUTE FUNCTION public.update_parent_task_effort();

CREATE OR REPLACE TRIGGER trg_update_parent_effort_after_update
    AFTER UPDATE 
    ON public."Tasks"
    FOR EACH ROW
    WHEN (new."ParentTaskID" IS NOT NULL)
    EXECUTE FUNCTION public.update_parent_task_effort();
```

<h2>Особенности проекта</h2>

<h3>Шаблон проекта включает следующие функции:</h3>
<ul>
<li>Чистая архитектура: Проект структурирован в соответствии с принципами чистой архитектуры, что способствует разделению ответственности и ясной организации компонентов.</li>
<li>Принципы SOLID: Код следует принципам SOLID (Единая ответственность, Открытость/Закрытость, Подстановка Лисков, Разделение интерфейсов и Инверсия зависимостей), что упрощает поддержку и расширение.</li>
<li>Паттерн Repository: Паттерн Repository абстрагирует уровень доступа к данным и предоставляет единый интерфейс для работы с данными.</li>
<li>Паттерн Unit of Work: Паттерн Unit of Work помогает управлять транзакциями и обеспечивает согласованность при работе с несколькими репозиториями.</li>
<li>Entity Framework Core: Проект использует Entity Framework Core в качестве инструмента ORM (Object-Relational Mapping) для доступа к данным.</li>
<li>ASP.NET Core API: В проект включен проект ASP.NET Core API, который служит слоем API, обрабатывающим HTTP запросы и ответы.</li>
<li>CRUD операции: Шаблон проекта предоставляет основу для реализации полных операций CRUD (Создание, Чтение, Обновление, Удаление) для сущностей с использованием Entity Framework Core.</li>
<li>Внедрение зависимостей: Проект использует встроенный контейнер внедрения зависимостей в ASP.NET Core, что упрощает управление и внедрение зависимостей по всему приложению.</li>
<li>Модульное тестирование: В решении включены отдельные проекты для модульного тестирования уровней ядра, инфраструктуры и API.</li>
</ul>

<h2>Функциональный требования</h2>
<ul>
  <li>FR001: Создание задач и подзадач.</li>
  <li>FR002: Просмотр и редактирование всех полей задачи (кроме вычисляемых).</li>
  <li>FR003: Удаление задач и подзадач.</li>
  <li>FR004: Просмотр списка задач.</li>
  <li>FR005: Управление статусами задач с учетом их подзадач.</li>
</ul>

<h2>Требования к интерфейсу пользователя</h2>
<ul>
  <li>UI001: Иерархия задач представлена в виде дерева.</li>
  <li>UI002: Список подзадач выбранной задачи в виде плоского списка.</li>
  <li>UI003: Интерфейс организован по типу "Проводника Windows".</li>
  <li>UI004: Переход между задачами без перезагрузки страницы (AJAX).</li>
</ul>
