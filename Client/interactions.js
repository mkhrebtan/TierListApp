import { dataManager } from './dataManager.js';

// Image Popup Interaction

// Variables for DOM elements
let popup = document.getElementById("popup");
let overlay = document.getElementById("blurOverlay");
let popupNoteInput = document.getElementById("imageNoteInput");
let noteSaveBtn = document.getElementById("noteSaveBtn");
let popupImageLayer = document.getElementById("popupImage");

// Variables to keep track of the current image and note
let currentImageId = null;
let currentImageNote = null;

export async function setupPopupForImages() {
    popup = document.getElementById("popup");
    overlay = document.getElementById("blurOverlay");
    popupNoteInput = document.getElementById("imageNoteInput");
    noteSaveBtn = document.getElementById("noteSaveBtn");
    popupImageLayer = document.getElementById("popupImage");

    const draggableElements = document.querySelectorAll('.draggable');
    draggableElements.forEach(element => {
        addOpenPopupListener(element);
    });

    const closePopupButton = document.getElementById("closePopup");
    closePopupButton.addEventListener("click", closePopup);

    popupNoteInput.addEventListener('input', () => {
        const inputValue = popupNoteInput.value.trim();
        if (inputValue !== currentImageNote) {
            showSaveButton(noteSaveBtn, popupNoteInput);
        } else {
            hideSaveButton(noteSaveBtn, popupNoteInput);
        }
    });

    noteSaveBtn.addEventListener('click', async () => {
        if (!currentImageId) {
            alert(`Error updating image note. Please try again.`);
            return;
        }

        const newNote = imageNoteInput.value.trim();

        noteSaveBtn.classList.add('hidden');
        popupNoteInput.style.flex = "1";

        const updateImageNoteResult = await dataManager.updateImageNote(currentImageId, newNote);
        if (!updateImageNoteResult.success) {
            popupNoteInput.value = currentImageNote;
            alert(`Error updating image note: ${updateImageNoteResult.message}. Please try again.`);
            return;
        }
    });

    const popupImageDeleteButton = document.getElementById("deleteBtn");
    popupImageDeleteButton.addEventListener("click", deleteCurrentImage);

    const popupImageChangeButton = document.getElementById("replaceBtn");
    const fileInputChange = document.getElementById("fileInputChange");
    popupImageChangeButton.addEventListener("click", () => {
        if (fileInputChange) {
            fileInputChange.click();
        }
    });
}

function addOpenPopupListener(clickableElement) {
    const imageId = parseInt(clickableElement.id.replace('img', ''));
    clickableElement.addEventListener("click", async () => {
        const image = await dataManager.getImage(imageId);
        currentImageId = imageId;
        currentImageNote = image.note;
        openPopup(image);
    });
}

function openPopup(image) {
    const popupImage = document.createElement('img');
    popupImage.src = image.url;
    popupImage.alt = image.altText;

    popupNoteInput.value = image.note;

    popupImageLayer.innerHTML = popupImage.outerHTML;

    popup.classList.add("active");
    overlay.classList.add("active");
    document.body.classList.add("body-noscroll");
}

function closePopup() {
    currentImageId = null;
    currentImageNote = null;

    popup.classList.remove("active");
    overlay.classList.remove("active");
    document.body.classList.remove("body-noscroll");
}

function showSaveButton(btn, input) {
    btn.classList.remove('hidden');
    input.style.flex = "0.98";
}

function hideSaveButton(btn, input) {
    btn.classList.add('hidden');
    input.style.flex = "1";
}

async function deleteCurrentImage() {
    const confirmation = confirm("Are you sure you want to delete this image? This action cannot be undone.");
    if (!confirmation || !currentImageId) {
        return;
    }

    const deleteResult = await dataManager.deleteImage(currentImageId);

    if (deleteResult.success) {
        const imageElement = document.getElementById(`img${currentImageId}`);
        if (imageElement) {
            imageElement.remove();
        }

        closePopup();
    }

    currentImageId = null;
}

function showNewImageAfterChange(image) {
    const popupImage = document.createElement('img');
    popupImage.src = image.url;
    popupImage.alt = image.altText;

    popupNoteInput.value = image.note;

    popupImageLayer.innerHTML = popupImage.outerHTML;
}

// Image Upload Interaction

export async function setupImageUpload() {
    const fileSelect = document.getElementById("fileSelect");
    const fileInput = document.getElementById("fileInput");
    const fileInputChange = document.getElementById("fileInputChange");

    fileInput.addEventListener("change", handleImageUpload, false);
    fileSelect.addEventListener("click", (e) => {
        if (fileInput) {
            fileInput.click();
        }
    }, false,);
    fileInputChange.addEventListener("change", handleImageChange, false);
}

async function handleImageUpload() {
    if (this.files.length) {
        const file = this.files[0];

        const uploadUrlResponse = await dataManager.getImageUploadUrl(file.name, file.type);
        if (uploadUrlResponse.success === false) {
            alert(`Failed to get upload URL: ${uploadUrlResponse.message}`);
            return;
        }

        const uploadResponse = await fetch(uploadUrlResponse.url, {
            headers: {
                'Content-Type': file.type,
                'x-amz-meta-file-name': file.name,
            },
            method: 'PUT',
            body: file
        })

        if (!uploadResponse.ok) {
            alert(`Failed to upload image. Please try again.`);
            return;
        }

        const imageUrl = uploadUrlResponse.url.split('?')[0];

        const backupImagesDropBox = document.getElementById("backup-drop-box");

        const imageToSave = {
            storageKey: uploadUrlResponse.storageKey,
            url: imageUrl,
            order: backupImagesDropBox.children.length + 1,
            note: "",
            containerId: dataManager.listData.backupRowId,
            listId: dataManager.listData.id,
        };

        const imageSaveResult = await dataManager.addImage(imageToSave);
        if (imageSaveResult.success === false) {
            alert(`Failed to upload image. Please try again.`);
            return;
        }

        const imageElement = createImageElement(imageSaveResult.image);
        addOpenPopupListener(imageElement);
        backupImagesDropBox.appendChild(imageElement);
    }
}

async function handleImageChange() {
    if (this.files.length) {
        if (!currentImageId) {
            alert('Something went wrong, please try again.');
            return;
        }

        const file = this.files[0];

        const uploadUrlResponse = await dataManager.getImageUploadUrl(file.name, file.type);
        if (uploadUrlResponse.success === false) {
            alert(`Failed to get upload URL: ${uploadUrlResponse.message}`);
            return;
        }

        const uploadResponse = await fetch(uploadUrlResponse.url, {
            headers: {
                'Content-Type': file.type,
                'x-amz-meta-file-name': file.name,
            },
            method: 'PUT',
            body: file
        })

        if (!uploadResponse.ok) {
            console.error(`Upload failed: ${uploadResponse.status} ${uploadResponse.statusText}`);
            return;
        }

        const imageUrl = uploadUrlResponse.url.split('?')[0];

        const imageUpdateResult = await dataManager.updateImageUrl(currentImageId, imageUrl);
        if (!imageUpdateResult.success) {
            alert('Failed to update image, please try again.');
            return;
        }

        const existingImageElement = document.getElementById(`img${currentImageId}`);
        if (existingImageElement) {
            existingImageElement.querySelector('img').src = imageUrl;
        }

        showNewImageAfterChange(imageUpdateResult.image);
    }
}

function createImageElement(image) {
    const imageContainer = document.createElement('div');
    imageContainer.classList.add('draggable');
    imageContainer.id = `img${image.id}`;
    imageContainer.draggable = true;

    const imageElement = document.createElement('img');
    imageElement.src = image.url;
    imageElement.draggable = false;

    imageContainer.appendChild(imageElement);
    return imageContainer;
}

// Drag and Drop Interaction

export async function setupDragAndDrop() {
    const dropBoxes = document.querySelectorAll('.tier-drop-box');
    dropBoxes.forEach(dropBox => {
        new Sortable(dropBox, {
            group: 'shared',
            animation: 300,
            ghostClass: 'sortable-ghost',
            chosenClass: "sortable-chosen",
            onEnd: function (evt) {
                updateDataAfterDrop(evt);
            }
        });
    });
}

async function updateDataAfterDrop(evt) {
    if (!evt.item) return;

    const imageId = parseInt(evt.item.id.replace('img', ''));
    const newIndex = evt.newIndex;
    const oldIndex = evt.oldIndex;
    let fromBoxId = null;

    if (evt.from.id === 'backup-drop-box') {
        fromBoxId = 'backup-drop-box';
    }
    else {
        fromBoxId = parseInt(evt.from.id.replace('drop-box-', ''));
    }

    if (evt.from === evt.to) {
        if (newIndex === oldIndex) return;
        const updateImageResult = await dataManager.updateImageOrder(fromBoxId, imageId, newIndex);
        if (!updateImageResult.success) {
            evt.from.insertBefore(evt.item, evt.from.children[oldIndex]);
            alert(`Error updating image order: ${updateImageResult.message}. Please try again.`);
            return;
        }
    }
    else {
        let toBoxId = null;
        if (evt.to.id === 'backup-drop-box') {
            toBoxId = 'backup-drop-box';
        }
        else {
            toBoxId = parseInt(evt.to.id.replace('drop-box-', ''));
        }

        const updateImageResult = await dataManager.moveImage(imageId, fromBoxId, toBoxId, newIndex);
        if (!updateImageResult.success) {
            evt.from.insertBefore(evt.item, evt.from.children[oldIndex]);
            alert(`Error moving image: ${updateImageResult.message}. Please try again.`);
            return;
        }
    }
}

// List editing interaction

let pickr = null;
let tierListSortable = null;
let isEditingMode = false;
let editButton = null;
let addRowButton = null;
let tierList = null;
let currentRow = null;
let currentRankContainer = null;
let currentRankText = null;

let rankSaveBtn = null;
let rankNameInput = null;
let rankPreview = null;
let rankPreviewSpan = null;
let rankPopup = null;

export async function setupListEditing() {
    const listTitleSaveBtn = document.getElementById('listTitleSaveBtn');
    const listTitle = document.getElementById('listTitle');
    listTitle.addEventListener('input', async () => {
        const newTitle = listTitle.textContent.trim();
        if (newTitle !== dataManager.listData.title && newTitle !== '') {
            listTitleSaveBtn.classList.remove('hidden');
        }
        else {
            listTitleSaveBtn.classList.add('hidden');
        }
    });

    listTitleSaveBtn.addEventListener('click', async () => {
        const newTitle = listTitle.textContent.trim();
        if (newTitle === '') {
            alert('List title cannot be empty.');
            return;
        }

        listTitleSaveBtn.classList.add('hidden');
        const updateTitleResult = await dataManager.updateTierListTitle(newTitle);
        if (!updateTitleResult.success) {
            listTitle.textContent = dataManager.listData.title; // Reset to previous title
            alert(`Error updating list title: ${updateTitleResult.message}. Please try again.`);
            return;
        }
    });

    editButton = document.getElementById('editBtn');
    editButton.addEventListener('click', toggleEditMode);

    const tierRows = document.querySelectorAll('.tier-row');
    tierRows.forEach(row => {
        row.appendChild(createOperationContainerForRow(row));
        row.querySelector('.tier-rank').appendChild(createEditButtonForRow(row));
    });

    const closeRankPopupButton = document.getElementById("rankClosePopup");
    closeRankPopupButton.addEventListener("click", closeRankPopup);

    rankSaveBtn = document.getElementById("rankSaveBtn");
    rankNameInput = document.getElementById("rankNameInput");
    rankPreview = document.getElementById("rankPreview");
    rankPreviewSpan = document.getElementById("rankPreviewSpan");
    rankPopup = document.getElementById("rankPopup");

    rankNameInput.addEventListener('input', () => {
        const inputValue = rankNameInput.value.trim();
        rankPreviewSpan.textContent = inputValue;
        if (inputValue !== '' && inputValue !== currentRankText) {
            showSaveButton(rankSaveBtn, rankNameInput);
        } else {
            hideSaveButton(rankSaveBtn, rankNameInput);
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

    tierList = document.getElementById('tier-list-container');
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

    pickr = Pickr.create({
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

    addRowButton = createAddRowButton();
}

function toggleEditMode() {
    const isEditing = editButton.classList.toggle('editing');
    isEditingMode = isEditing;
    editButton.innerHTML = isEditing ? "Exit" : "Edit List";
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

    const rankEditButtons = document.querySelectorAll('.rank-edit-btn');
    rankEditButtons.forEach(button => {
        if (isEditing) {
            button.classList.add('visible');
        } else {
            button.classList.remove('visible');
        }
    });

    const fileSelect = document.getElementById("fileSelect");
    if (isEditing) {
        fileSelect.disabled = true;
        fileSelect.classList.add('disabled');
    }
    else {
        fileSelect.disabled = false;
        fileSelect.classList.remove('disabled');
    }
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

function createEditButtonForRow(row) {
    const tierRank = row.querySelector('.tier-rank');

    const button = document.createElement('button');
    const icon = document.createElement('i');
    button.classList.add('rank-edit-btn');
    icon.classList.add('fa-solid', 'fa-pen', 'fa-sm');

    button.appendChild(icon);

    const rowId = parseInt(row.id.replace('row-', ''));
    button.addEventListener('click', async (event) => {
        event.stopPropagation();    
        if (isEditingMode) {
            const rowObj = await dataManager.getRow(rowId);
            pickr.setColor(tierRank.style.backgroundColor);
            currentRow = rowObj;
            currentRankContainer = tierRank;
            currentRankText = tierRank.querySelector('span').textContent;
            openRankPopup(tierRank.querySelector('span').textContent, tierRank.style.backgroundColor);
        }
    });

    return button;
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

function openRankPopup(rankText, rankColor) {
  rankPreviewSpan.textContent = rankText;
  rankPreview.style.backgroundColor = rankColor;
  rankNameInput.value = rankText;

  rankPopup.classList.add('active');
  overlay.classList.add('active');
  document.body.classList.add("body-noscroll");
}

function closeRankPopup() {
  currentRankContainer = null;
  currentRow = null;
  currentRankText = null;

  hideSaveButton(rankSaveBtn, rankNameInput);

  rankPopup.classList.remove("active");
  overlay.classList.remove("active");
  document.body.classList.remove("body-noscroll");
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