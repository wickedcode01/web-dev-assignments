document.addEventListener('DOMContentLoaded', () => {
    const taskManager = new TaskManager();
    const taskForm = document.getElementById('taskForm');
    const taskList = document.getElementById('taskList');
    const searchInput = document.getElementById('searchInput');
    const categoryFilter = document.getElementById('categoryFilter');
    const prioritySort = document.getElementById('prioritySort');
    const themeToggle = document.getElementById('themeToggle');
    themeToggle.addEventListener('click', toggleTheme);
    // Add this at the top of the file (after DOMContentLoaded event)
    let isEditing = false;

    // Form submission
    taskForm.addEventListener('submit', (e) => {
        e.preventDefault();
        if (isEditing) return;  // Prevent duplicate submissions

        const title = document.getElementById('taskTitle').value;
        const description = document.getElementById('taskDescription').value;
        const priority = document.getElementById('taskPriority').value;
        const category = document.getElementById('taskCategory').value;

        taskManager.addTask(title, description, priority, category);
        taskManager.showNotification('Task added successfully!');
        renderTasks();
        taskForm.reset();
    });

    // Filter and search handling
    const handleFilters = () => {
        const searchTerm = searchInput.value;
        const category = categoryFilter.value;
        const priority = prioritySort.value;
        renderTasks(category, searchTerm, priority);
    };

    searchInput.addEventListener('input', handleFilters);
    categoryFilter.addEventListener('change', handleFilters);
    prioritySort.addEventListener('change', handleFilters);

    // Render tasks
    function renderTasks(category = 'all', searchTerm = '', priority = 'all') {
        const filteredTasks = taskManager.filterTasks(category, searchTerm, priority);
        taskList.innerHTML = '';

        filteredTasks.forEach(task => {
            const taskElement = document.createElement('div');
            taskElement.className = `task-item ${task.priority} ${task.completed ? 'completed' : ''}`;
            taskElement.innerHTML = `
                <h3>${task.title}</h3>
                <p>${task.description}</p>
                <div class="task-meta">
                    <span class="category">${task.category}</span>
                    <span class="priority">${task.priority}</span>
                </div>
                <div class="task-actions">
                    <button onclick="editTask('${task.id}')">Edit</button>
                    <button onclick="deleteTask('${task.id}')">Delete</button>
                    <button onclick="toggleComplete('${task.id}')">
                        ${task.completed ? 'Uncomplete' : 'Complete'}
                    </button>
                </div>
            `;
            taskList.appendChild(taskElement);
        });
    }

    // Global functions for task actions
    window.editTask = (taskId) => {
        isEditing = true;
        const task = taskManager.getTask(taskId);
        if (task) {
            // change button text
            const submitButton = document.getElementById('submitButton');
            submitButton.textContent = 'Update Task';

            document.getElementById('taskTitle').value = task.title;
            document.getElementById('taskDescription').value = task.description;
            document.getElementById('taskPriority').value = task.priority;
            document.getElementById('taskCategory').value = task.category;

            // Save the original handler
            const originalHandler = taskForm.onsubmit;

            // Change form submit behavior temporarily
            taskForm.onsubmit = (e) => {
                e.preventDefault();
                taskManager.updateTask(
                    taskId,
                    document.getElementById('taskTitle').value,
                    document.getElementById('taskDescription').value,
                    document.getElementById('taskPriority').value,
                    document.getElementById('taskCategory').value
                );
                renderTasks();
                taskForm.reset();
                submitButton.textContent = 'Add Task';
                // Restore original handler
                taskForm.onsubmit = originalHandler;
                isEditing = false;  // Clear editing flag
            };
        }
    };

    window.deleteTask = (taskId) => {
        taskManager.deleteTask(taskId);
        renderTasks();
    };

    window.toggleComplete = (taskId) => {
        const task = taskManager.getTask(taskId);
        if (task) {
            taskManager.toggleComplete(taskId);
            renderTasks();
        }
    };

    // Initial render
    renderTasks();
});

// Theme toggle
const toggleTheme = () => {
    document.body.classList.toggle('dark-theme');
    console.log('switch theme')
};