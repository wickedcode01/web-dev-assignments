class Task {
    constructor(title, description, priority, category) {
        this.id = Date.now().toString();
        this.title = title;
        this.description = description;
        this.priority = priority;
        this.category = category;
        this.completed = false;
        this.createdAt = new Date();
    }

    toggleComplete() {
        this.completed = !this.completed;
    }

    update(title, description, priority, category) {
        this.title = title;
        this.description = description;
        this.priority = priority;
        this.category = category;
    }
}