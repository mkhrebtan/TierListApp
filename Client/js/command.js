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
        this.dataManager.addRow(this.row, this.insertIndex);

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

class DeleteRowCommand extends ListEditCommand {
    constructor(dataManager, listContainer, rowContainer, row, rowIndex) {
        super();
        this.dataManager = dataManager;
        this.listContainer = listContainer;
        this.rowContainer = rowContainer;
        this.row = row;
        this.rowIndex = rowIndex;
    }

    execute() {
        this.dataManager.deleteRow(this.row.id);
        this.listContainer.removeChild(this.rowContainer);
    }

    undo() {
        this.dataManager.addRow(this.row);

        const children = this.listContainer.children;
        if (this.rowIndex >= children.length) {
            this.listContainer.appendChild(this.rowContainer);
        }
        else {
            this.listContainer.insertBefore(this.rowContainer, children[this.rowIndex]);
        }
    }
}

class ChangeRankColorCommand extends ListEditCommand {
    constructor(dataManager, tierRankContainer, row, newColorHex) {
        super();
        this.dataManager = dataManager;
        this.row = row;
        this.tierRankContainer = tierRankContainer;
        this.newColorHex = newColorHex;
        this.oldColorHex = null;
    }

    execute() {
        this.oldColorHex = this.row.colorHex;
        this.dataManager.updateRankColor(this.row.id, this.newColorHex);
        this.tierRankContainer.style.backgroundColor = this.newColorHex;
    }

    undo() {
        this.dataManager.updateRankColor(this.row.id, this.oldColorHex);
        this.tierRankContainer.style.backgroundColor = this.oldColorHex;
    }
}

class ChangeRankTextCommand extends ListEditCommand {
    constructor(dataManager, tierRankContainer, row, newRank) {
        super();
        this.dataManager = dataManager;
        this.row = row;
        this.tierRankContainer = tierRankContainer;
        this.newRank = newRank;
        this.oldRank = null;
    }

    execute() {
        this.oldRank = this.row.rank;
        this.dataManager.updateRowRank(this.row.id, this.newRank);
        // this.tierRankContainer.textContent = this.newRank;
    }

    undo() {
        this.dataManager.updateRowRank(this.row.id, this.oldRank);
        this.tierRankContainer.textContent = this.oldRank;
    }
}

class ChangeRowOrderCommand extends ListEditCommand {
    constructor(dataManager, listContainer, oldIndex, newIndex) {
        super();
        this.dataManager = dataManager;
        this.listContainer = listContainer;
        this.oldIndex = oldIndex;
        this.newIndex = newIndex;
    }

    execute() {
        this.dataManager.updateRowOrder(this.oldIndex, this.newIndex);
    }

    undo() {
        this.dataManager.updateRowOrder(this.newIndex, this.oldIndex);
        const children = Array.from(this.listContainer.children);
        const element = children.splice(this.newIndex, 1)[0];
        children.splice(this.oldIndex, 0, element);
        this.listContainer.innerHTML = '';
        children.forEach(child => this.listContainer.appendChild(child));
    }
}