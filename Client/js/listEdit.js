const editButton = document.getElementById('editBtn');
const undoButton = document.getElementById('undoBtn');
let addRowButton = null;
let tierListSortable = null;
const commandManager = new CommandManager();
let isEditingMode = false;
const editIcon = document.createElement('i');
editIcon.classList.add('fa-solid', 'fa-pen', 'fa-lg');
// const undoIcon = document.createElement('i');
// undoIcon.classList.add('fa-solid', 'fa-arrow-turn-up', 'fa-rotate-270', 'fa-xl');
const checkIcon = document.createElement('i');
checkIcon.classList.add('fa-solid', 'fa-check', 'fa-xl');
let currentRow = null;
let currentRankContainer = null;
let currentRankText = null;

const pickr = Pickr.create({
        el: '.rank-popup-color',
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

            preview: false,
            opacity: false,
            hue: true,

            interaction: {
                hex: true,
                rgba: true,
                input: true,
                clear: false,
                save: true,
                cancel: false
            }
        }
    });

pickr.on('save', async (color, instance) => {
        if (!currentRow || !currentRankContainer) {
            return;
        }

        const hexColor = color.toHEXA().toString();
        const updateColorResult = await dataManager.updateRowColor(currentRow.id, hexColor);
        if (!updateColorResult.success) {
            alert(`Error updating color: ${updateColorResult.message}. Please try again.`);
            return;
        }

        currentRankContainer.style.backgroundColor = hexColor;
        rankPreview.style.backgroundColor = hexColor;
        instance.hide();
    }).on('cancel', (instance) => {
        instance.hide();
    });

function initializeEditing() {
    editButton.addEventListener('click', toggleEditMode);
    setupEditingEventListeners();
    undoButton.addEventListener('click', () => {
        commandManager.undo();
    });

    const tierRows = document.querySelectorAll('.tier-row');
    tierRows.forEach(row => {
        row.appendChild(createOperationContainerForRow(row));
        row.querySelector('.tier-rank').appendChild(createEditButtonForRow(row));
    });

    addRowButton = createAddRowButton();

    tierListSortable = new Sortable(tierList, {
        animation: 300,
        ghostClass: 'sortable-ghost',
        chosenClass: "sortable-chosen",
        onEnd: async function (evt) {
            if (!evt.item) return;

            const oldIndex = evt.oldIndex;
            const newIndex = evt.newIndex;
            if (oldIndex === newIndex) return;

            const rowId = parseInt(evt.item.id.replace('row-', ''));
            const updateRowResult = await dataManager.updateRowOrder(rowId, newIndex + 1);
            if (!updateRowResult.success) {
                evt.from.insertBefore(evt.item, evt.from.children[oldIndex]);
                alert(`Error updating row order: ${updateRowResult.message}. Please try again.`);
                return;
            }
        },
        disabled: true
    });

    rankNameInput.addEventListener('input', () => {
        const inputValue = rankNameInput.value.trim();
        rankPreviewSpan.textContent = inputValue;
        if (inputValue !== '' && inputValue !== currentRankText) {
            showSaveButton(rankSaveBtn, rankNameInput);
        } else {
            hideSaveButton(rankSaveBtn, rankNameInput);
        }
    });

    imageNoteInput.addEventListener('input', () => {
        const inputValue = imageNoteInput.value.trim();
        if (inputValue !== currentImageNote) {
            showSaveButton(noteSaveBtn, imageNoteInput);
        } else {
            hideSaveButton(noteSaveBtn, imageNoteInput);
        }
    });

    rankSaveBtn.addEventListener('click', async () => {
        if (!currentRow || !currentRankContainer) {
            alert(`Error updating rank. Please try again.`);
            return;
        }

        const newRank = rankNameInput.value.trim();
        if (newRank === '') {
            alert('Rank name cannot be empty.');
            return;
        }

        rankSaveBtn.classList.add('hidden');
        rankNameInput.style.flex = "1";

        const updateRankResult = await dataManager.updateRowRank(currentRow.id, newRank);
        if (!updateRankResult.success) {
            rankNameInput.value = currentRankText; // Reset to previous rank
            alert(`Error updating rank: ${updateRankResult.message}. Please try again.`);
            return;
        }

        currentRankContainer.querySelector('span').textContent = newRank;
        rankPreviewSpan.textContent = newRank;
    });

    noteSaveBtn.addEventListener('click', async () => {
        if (!currentImageId) {
            alert(`Error updating image note. Please try again.`);
            return;
        }

        const newNote = imageNoteInput.value.trim();

        noteSaveBtn.classList.add('hidden');
        imageNoteInput.style.flex = "1";

        const updateImageNoteResult = await dataManager.updateImageNote(currentImageId, newNote);
        if (!updateImageNoteResult.success) {
            imageNoteInput.value = currentImageNote; // Reset to previous note
            alert(`Error updating image note: ${updateImageNoteResult.message}. Please try again.`);
            return;
        }
    });
}

function toggleEditMode() {
    const isEditing = editButton.classList.toggle('editing');
    isEditingMode = isEditing;
    editButton.innerHTML = isEditing ? checkIcon.outerHTML : editIcon.outerHTML;
    if (isEditing) {
        tierList.appendChild(addRowButton);
    }
    else {
        tierList.removeChild(addRowButton);
    }

    tierListSortable.option('disabled', !isEditing);

    const draggableElements = document.querySelectorAll('.draggable');
    draggableElements.forEach(element => {
        if (isEditing) {
            element.classList.add('ghosted');
        } else {
            element.classList.remove('ghosted');
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

    const colorPickers = document.querySelectorAll('.rank-edit-btn');
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
        const { canUndo } = event.detail;
        undoButton.classList.toggle('hiden', !canUndo);
        undoButton.disabled = !canUndo;
    });
}

function createOperationContainerForRow(row) {
    const rowOperation = document.createElement('div');
    rowOperation.classList.add('tier-operation');

    const deleteRowBtn = document.createElement('button');
    deleteRowBtn.classList.add('delete-row-btn');
    rowOperation.appendChild(deleteRowBtn);

    const deleteIcon = document.createElement('i');
    deleteIcon.classList.add('fa-solid', 'fa-trash', 'fa-lg');
    deleteRowBtn.appendChild(deleteIcon);

    deleteRowBtn.addEventListener('click', () => deleteRow(row));

    return rowOperation;
}

function createRowContainer(row) {
    const rowContainer = document.createElement('div');
    rowContainer.classList.add('tier-row');
    rowContainer.id = `row-${row.id}`;
    
    const rowRank = document.createElement('div');
    rowRank.classList.add('tier-rank');
    rowRank.style.backgroundColor = row.colorHex;

    const rankText = document.createElement('span');
    rankText.textContent = row.rank;

    rowRank.appendChild(rankText);

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
    const rowEditBtn = createEditButtonForRow(rowContainer);
    rowEditBtn.classList.add('visible');
    
    rowContainer.appendChild(rowOperation);
    rowRank.appendChild(rowEditBtn);
    return rowContainer;
}

async function addRow() {
    const newRowResponse = await dataManager.addRow();
    if (!newRowResponse.success) {
        alert(`Error adding new row, please try again.`);
        return;
    }
    const newRowContainer = createRowContainer(newRowResponse.row);
    tierList.insertBefore(newRowContainer, addRowButton);
}

async function deleteRow(htmlRow) {
    const confirmation = confirm("Are you sure you want to delete this row? Images in this row will also be deleted.");

    if (!confirmation) {
        return;
    }

    const rowId = parseInt(htmlRow.id.replace('row-', ''));
    const deleteRowResponse = await dataManager.deleteRow(rowId);
    if (!deleteRowResponse.success) {
        alert(`Error deleting row, please try again.`);
        return;
    }

    tierList.removeChild(htmlRow);
}

function createAddRowButton() {
    const addRowBtn = document.createElement('button');
    addRowBtn.classList.add('add-row-btn');
    
    const addIcon = document.createElement('i');
    addIcon.classList.add('fa-solid', 'fa-plus', 'fa-lg');
    addRowBtn.appendChild(addIcon);

    addRowBtn.addEventListener('click', async (event) => {
        event.stopPropagation();
        if (isEditingMode) {
            await addRow();
        }
    });

    return addRowBtn;
}

function createEditButtonForRow(row) {
    const tierRank = row.querySelector('.tier-rank');

    const button = document.createElement('button');
    const icon = document.createElement('i');
    button.classList.add('rank-edit-btn');
    icon.classList.add('fa-solid', 'fa-pen', 'fa-sm');

    button.appendChild(icon);

    const rowId = parseInt(row.id.replace('row-', ''));
    const rowObj = dataManager.getRow(rowId);
    button.addEventListener('click', (event) => {
        event.stopPropagation();    
        if (isEditingMode) {
            pickr.setColor(tierRank.style.backgroundColor);
            currentRow = rowObj;
            currentRankContainer = tierRank;
            currentRankText = tierRank.querySelector('span').textContent;
            openRankPopup(tierRank.querySelector('span').textContent, tierRank.style.backgroundColor);
        }
    });

    return button;
}

function showSaveButton(btn, input) {
    btn.classList.remove('hidden');
    input.style.flex = "0.98";
}

function hideSaveButton(btn, input) {
    btn.classList.add('hidden');
    input.style.flex = "1";
}