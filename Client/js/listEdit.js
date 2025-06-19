const editButton = document.getElementById('editBtn');
const undoButton = document.getElementById('undoBtn');
const discardButton = document.getElementById('discardBtn');
let tierListSortable = null;
const commandManager = new CommandManager();
let isEditingMode = false;

function initializeEditing() {
    editButton.addEventListener('click', toggleEditMode);
    setupEditingEventListeners();
    undoButton.addEventListener('click', () => {
        commandManager.undo();
    });
    discardButton.addEventListener('click', () => {
        commandManager.discardAllChanges();
    });

    const tierRows = document.querySelectorAll('.tier-row');
    tierRows.forEach(row => {
        row.appendChild(createOperationContainerForRow(row));
        row.querySelector('.tier-rank').appendChild(createColorPickerForRow(row));
    });

    const tierRanks = document.querySelectorAll('.tier-rank');
    tierRanks.forEach(rank => {
        addInputEventListener(rank);
    });

    tierListSortable = new Sortable(tierList, {
        animation: 300,
        ghostClass: 'sortable-ghost',
        chosenClass: "sortable-chosen",
        onEnd: function (evt) {
            if (!evt.item) return;

            const newIndex = evt.newIndex;
            const oldIndex = evt.oldIndex;
            dataManager.updateRowOrder(oldIndex, newIndex);
            console.log(dataManager.toAPIFormat());
            console.log('Row order updated');
        },
        disabled: true // Initially disabled until edit mode is activated
    });
}

function toggleEditMode() {
    const isEditing = editButton.classList.toggle('editing');
    isEditingMode = isEditing;
    editButton.textContent = isEditing ? 'Save' : 'Edit';

    tierListSortable.option('disabled', !isEditing);

    const draggableElements = document.querySelectorAll('.draggable');
    draggableElements.forEach(element => {
        if (isEditing) {
            element.classList.add('editing');
        } else {
            element.classList.remove('editing');
        }
    });

    const tierRanks = document.querySelectorAll('.tier-rank');
    tierRanks.forEach(rank => {
        if (isEditing) {
            rank.setAttribute('contenteditable', 'true');
        } else {
            rank.removeAttribute('contenteditable');
        }
    });

    const tierOperations = document.querySelectorAll('.tier-operation');
    tierOperations.forEach(operation => {
        if (isEditing) {
            operation.classList.add('visible');
        } else {
            operation.classList.remove('visible');
        }
    });

    const colorPickers = document.querySelectorAll('.color-picker');
    colorPickers.forEach(picker => {
        if (isEditing) {
            picker.classList.add('visible');
        } else {
            picker.classList.remove('visible');
        }
    });
}

function setupEditingEventListeners() {
    document.addEventListener('commandStateChanged', (event) => {
        const { canUndo, hasChanges } = event.detail;
        
        undoButton.disabled = !canUndo;
        discardButton.disabled = !hasChanges;
        
        if (isEditingMode) {
            editButton.textContent = hasChanges ? 'Save Changes' : 'Save';
        }
    });
}

function createOperationContainerForRow(row) {
    const rowOperation = document.createElement('div');
    rowOperation.classList.add('tier-operation');

    const operationBtn = document.createElement('button');
    operationBtn.classList.add('tier-operation-expand-btn');
    rowOperation.appendChild(operationBtn);

    const operationIcon = document.createElement('i');
    operationIcon.classList.add('fa-solid', 'fa-chevron-left', 'fa-lg');
    operationBtn.appendChild(operationIcon);

    const operationContent = document.createElement('div');
    operationContent.classList.add('tier-operation-content');

    const addAboveBtn = document.createElement('button');
    addAboveBtn.classList.add('tier-operation-btn');
    addAboveBtn.textContent = 'Add Row Above';
    operationContent.appendChild(addAboveBtn);

    const addBelowBtn = document.createElement('button');
    addBelowBtn.classList.add('tier-operation-btn');
    addBelowBtn.textContent = 'Add Row Below';
    operationContent.appendChild(addBelowBtn);

    // const clearRowBtn = document.createElement('button');
    // clearRowBtn.classList.add('tier-operation-btn');
    // clearRowBtn.textContent = 'Clear Row';
    // operationContent.appendChild(clearRowBtn);

    const deleteRowBtn = document.createElement('button');
    deleteRowBtn.classList.add('tier-operation-btn');
    deleteRowBtn.textContent = 'Delete Row';
    operationContent.appendChild(deleteRowBtn);

    // Smooth expand/collapse functionality
    operationBtn.addEventListener('click', () => {
        const isExpanded = operationContent.classList.contains('visible');
        
        if (isExpanded) {
            // Collapse
            operationContent.classList.remove('visible');
            operationIcon.classList.remove('fa-chevron-right');
            operationIcon.classList.add('fa-chevron-left');
        } else {
            // Expand
            operationContent.classList.add('visible');
            operationIcon.classList.remove('fa-chevron-left');
            operationIcon.classList.add('fa-chevron-right');
        }
    });

    // Add event listeners for the operation buttons
    addAboveBtn.addEventListener('click', () => addRowAbove(row));
    addBelowBtn.addEventListener('click', () => addRowBelow(row));
    deleteRowBtn.addEventListener('click', () => deleteRow(row));

    rowOperation.appendChild(operationContent);
    return rowOperation;
}

function createRow() {
    const newRow = dataManager.createRow();
    return newRow;
}

function createRowContainer(row) {
    const rowContainer = document.createElement('div');
    rowContainer.classList.add('tier-row');
    rowContainer.id = `row-${row.id}`;
    
    const rowRank = document.createElement('div');
    rowRank.classList.add('tier-rank');
    rowRank.textContent = row.rank;
    rowRank.style.backgroundColor = row.colorHex;
    addInputEventListener(rowRank);

    const rowDropBox = document.createElement('div');
    rowDropBox.classList.add('tier-drop-box');
    rowDropBox.id = `drop-box-${row.id}`;
    new Sortable(rowDropBox, {
        group: 'shared',
        animation: 300,
        ghostClass: 'sortable-ghost',
        chosenClass: "sortable-chosen",
        onEnd: function (evt) {
            updateDataAfterDrop(evt);
        }
    });

    rowContainer.appendChild(rowRank);
    rowContainer.appendChild(rowDropBox);

    const rowOperation = createOperationContainerForRow(rowContainer);
    rowOperation.classList.add('visible');
    const rowColorPicker = createColorPickerForRow(rowContainer);
    rowColorPicker.classList.add('visible');
    
    rowContainer.appendChild(rowOperation);
    rowRank.appendChild(rowColorPicker);
    rowRank.setAttribute('contenteditable', 'true');
    return rowContainer;
}

function getRowIndex(htmlRow) {
    const index = Array.from(tierList.children).indexOf(htmlRow);
    console.log(`Row index: ${index}`);
    return index;
}

function addRowAbove(htmlRow) {
    const newRow = createRow();
    const newRowContainer = createRowContainer(newRow);
    const rowIndex = getRowIndex(htmlRow);
    const addRowCommand = new AddRowCommand(dataManager, tierList, newRowContainer, newRow, rowIndex);
    commandManager.executeCommand(addRowCommand);
}

function addRowBelow(htmlRow) {
    const newRow = createRow();
    const newRowContainer = createRowContainer(newRow);
    const rowIndex = getRowIndex(htmlRow);
    const addRowCommand = new AddRowCommand(dataManager, tierList, newRowContainer, newRow, rowIndex + 1);
    commandManager.executeCommand(addRowCommand);
}

function deleteRow(htmlRow) {
    const rowImages = htmlRow.querySelectorAll('.draggable');
    rowImages.forEach(image => {
        image.parentNode.removeChild(image);
        backupImagesDropBox.appendChild(image);
    });
    tierList.removeChild(htmlRow);

    const rowId = parseInt(htmlRow.id.replace('row-', ''));
    dataManager.deleteRow(rowId);
}

function createColorPickerForRow(row) {
    const tierRank = row.querySelector('.tier-rank');

    const colorPicker = document.createElement('button');
    const colorPickerIcon = document.createElement('i');
    colorPicker.classList.add('color-picker');
    colorPickerIcon.classList.add('fa-solid', 'fa-palette');

    const pickr = Pickr.create({
        el: colorPicker,
        theme: 'monolith',

        swatches: [
            'rgba(244, 67, 54, 1)',
            'rgba(233, 30, 99, 1)',
            'rgba(156, 39, 176, 1)',
            'rgba(103, 58, 183, 1)',
            'rgba(63, 81, 181, 1)',
            'rgba(33, 150, 243, 1)',
            'rgba(3, 169, 244, 1)',
            'rgba(0, 188, 212, 1)',
            'rgba(0, 150, 136, 1)',
            'rgba(76, 175, 80, 1)',
            'rgba(139, 195, 74, 1)',
            'rgba(205, 220, 57, 1)',
            'rgba(255, 235, 59, 1)',
            'rgba(255, 193, 7, 1)'
        ],

        useAsButton: true,
        position: 'top-start',

        components: {

            preview: true,
            opacity: false,
            hue: true,

            interaction: {
                hex: true,
                rgba: true,
                input: true,
                clear: false,
                save: true,
                cancel: true
            }
        }
    });

    pickr.on('save', (color, instance) => {
        const hexColor = color.toHEXA().toString();
        const rowId = parseInt(row.id.replace('row-', ''));
        if (dataManager.updateRankColor(rowId, hexColor).success) {
            console.log(`Row color updated to ${hexColor}`);
            tierRank.style.backgroundColor = hexColor;
            instance.hide();
        }
        else {
            console.error('Failed to update row color');
        }
    }).on('cancel', (instance) => {
        instance.hide();
    });

    colorPicker.appendChild(colorPickerIcon);
    return colorPicker;
}

function addInputEventListener(tierRank) {
    tierRank.addEventListener('input', () => {
        console.log('Content updated:', tierRank.textContent);
    });
}