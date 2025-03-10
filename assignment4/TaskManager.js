class TaskManager {
    constructor() {
        this.tasks = [];
    }

    addTask(title, description, priority, category) {
        const task = new Task(title, description, priority, category);
        this.tasks.push(task);
        if (priority === 'high') {
            this.showNotification(`High priority task added: ${title}`);
        }
        return task;
    }

    deleteTask(taskId) {
        this.tasks = this.tasks.filter(task => task.id !== taskId);
    }

    updateTask(taskId, title, description, priority, category) {
        const task = this.getTask(taskId);
        if (task) {
            const oldPriority = task.priority;
            task.update(title, description, priority, category);
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
}