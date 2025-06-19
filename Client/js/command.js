class ListEditCommand {
    execute() {
        throw new Error('Execute method must be implemented');
    }

    undo() {
        throw new Error('Undo method must be implemented');
    }
}

class CommandManager {
    constructor() {
        this.history = [];
        this.currentIndex = -1;
        this.originalState = null;
    }

    executeCommand(command) {
        command.execute();
        this.history = this.history.slice(0, this.currentIndex + 1);
        this.history.push(command);
        this.currentIndex++;

        this.notifyStateChanged();
    }

    undo() {
        if (this.canUndo()) {
            const command = this.history[this.currentIndex];
            command.undo();
            this.currentIndex--;
            this.history = this.history.slice(0, this.currentIndex + 1);

            this.notifyStateChanged();
        }
    }

    canUndo() {
        return this.currentIndex >= 0;
    }

    discardAllChanges() {
        while (this.canUndo()) {
            this.undo();
        }
        this.clear();
    }

    clear() {
        this.history = [];
        this.currentIndex = -1;
    }

    notifyStateChanged() {
        document.dispatchEvent(new CustomEvent('commandStateChanged', {
            detail: {
                canUndo: this.canUndo(),
                hasChanges: this.history.length > 0
            }
        }));
    }
}

class AddRowCommand extends ListEditCommand {
    constructor(dataManager, listContainer, rowContainer, row, insertIndex) {
        super();
        this.dataManager = dataManager;
        this.listContainer = listContainer;
        this.rowContainer = rowContainer;
        this.row = row;
        this.insertIndex = insertIndex;
    }

    execute() {
        this.dataManager.addRow(this.row);

        const children = this.listContainer.children;
        if (this.insertIndex >= children.length) {
            this.listContainer.appendChild(this.rowContainer);
        }
        else {
            this.listContainer.insertBefore(this.rowContainer, children[this.insertIndex]);
        }
    }

    undo() {
        this.dataManager.deleteRow(this.row.id);
        this.listContainer.removeChild(this.rowContainer);
    }
}