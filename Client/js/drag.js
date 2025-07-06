function initializeDragAndDrop() {
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

