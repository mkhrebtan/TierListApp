function initializeDragAndDrop() {
    const dropBoxes = document.querySelectorAll('.tier-drop-box');

    dropBoxes.forEach(dropBox => {
        new Sortable(dropBox, {
            group: 'shared',
            animation: 300,
            ghostClass: 'sortable-ghost',
        });
    });
}

