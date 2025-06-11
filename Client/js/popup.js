const popup = document.getElementById("popup");
const overlay = document.getElementById("blurOverlay");

function initializePopup() {
    const draggableElements = document.querySelectorAll('.draggable');

    draggableElements.forEach(element => {
        addOpenPopupListener(element);
    });
}

function openPopup(clickedElement) {
    popup.classList.add("active"); 
    overlay.classList.add("active");

    const popupImageContainer = document.createElement('div');
    popupImageContainer.classList.add('popup-image-container');

    const imageElement = clickedElement.querySelector('img');

    if (imageElement) {
        const popupImage = document.createElement('img');
        popupImage.src = imageElement.src;
        popupImage.alt = imageElement.alt;
        popupImage.classList.add('popup-image');

        popupImageContainer.appendChild(popupImage);
        popup.appendChild(popupImageContainer);
    } else {
        console.error("No image found in the clicked element.");
    }
}

function closePopup() {
    popup.classList.remove("active");
    overlay.classList.remove("active");
    const popupImageContainer = popup.querySelector('.popup-image-container');
    if (popupImageContainer) {
        popup.removeChild(popupImageContainer);
    }
}

function addOpenPopupListener(clickableElement) {
    clickableElement.addEventListener("click", () => openPopup(clickableElement));
}