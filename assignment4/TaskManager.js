class TaskManager {
    constructor() {
        this.tasks = this.loadTasks();
    }

    saveTasks() {
        localStorage.setItem('tasks', JSON.stringify(this.tasks));
    }

    loadTasks() {
        const tasksData = localStorage.getItem('tasks');
        if (!tasksData) return [];

        return JSON.parse(tasksData).map(taskData => {
            const task = new Task(
                taskData.title,
                taskData.description,
                taskData.priority,
                taskData.category
            );
            task.id = taskData.id;
            task.completed = taskData.completed;
            task.createdAt = new Date(taskData.createdAt);
            return task;
        });
    }

    addTask(title, description, priority, category) {
        const task = new Task(title, description, priority, category);
        this.tasks.push(task);
        this.saveTasks();
        if (priority === 'high') {
            this.showNotification(`High priority task added: ${title}`);
        }
        return task;
    }

    deleteTask(taskId) {
        this.tasks = this.tasks.filter(task => task.id !== taskId);
        this.saveTasks();
    }

    updateTask(taskId, title, description, priority, category) {
        const task = this.getTask(taskId);
        if (task) {
            const oldPriority = task.priority;
            task.update(title, description, priority, category);
            this.saveTasks();
            if (priority === 'high' && oldPriority !== 'high') {
                this.showNotification(`Task updated to high priority: ${title}`);
            }
        }
    }

    getTask(taskId) {
        return this.tasks.find(task => task.id === taskId);
    }

    filterTasks(category, searchTerm, priorityFilter) {
        return this.tasks.filter(task => {
            const categoryMatch = category === 'all' || task.category === category;
            const searchMatch = !searchTerm ||
                task.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
                task.description.toLowerCase().includes(searchTerm.toLowerCase());
            const priorityMatch = priorityFilter === 'all' || task.priority === priorityFilter;

            return categoryMatch && searchMatch && priorityMatch;
        });
    }

    showNotification(message) {
        const notification = document.getElementById('notification');
        notification.textContent = message;
        notification.classList.add('show');

        setTimeout(() => {
            notification.classList.remove('show');
        }, 4000);
    }

    toggleComplete(taskId) {
        const task = this.getTask(taskId);
        if (task) {
            task.toggleComplete();
            this.saveTasks();
        }
    }
}