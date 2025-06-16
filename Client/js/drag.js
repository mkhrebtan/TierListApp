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

function updateDataAfterDrop(evt) {
    if (!evt.item) return;

    const imageId = parseInt(evt.item.id.replace('img', ''));
    const newIndex = evt.newIndex;
    let fromBoxId = null;

    if (evt.from.id === 'backup-drop-box') {
        fromBoxId = 'backup-drop-box';
    }
    else {
        fromBoxId = parseInt(evt.from.id.replace('drop-box-', ''));
    }
    
    if (evt.from === evt.to) {
        dataManager.updateImageOrder(fromBoxId, imageId, newIndex);
    }
    else {
        let toBoxId = null;
        if (evt.to.id === 'backup-drop-box') {
            toBoxId = 'backup-drop-box';
        }
        else {
            toBoxId = parseInt(evt.to.id.replace('drop-box-', ''));
        }
        dataManager.moveImage(imageId, fromBoxId, toBoxId, newIndex);
    }
    
    console.log(dataManager.toAPIFormat());
}

