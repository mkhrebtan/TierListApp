const popup = document.getElementById("popup");
const overlay = document.getElementById("blurOverlay");
const popupImageLayer = document.getElementById("popupImage");
const popupDescription = document.getElementById("popupDescription");
const popupDeleteButton = document.getElementById("deleteBtn");
let currentImageId = null;

function initializePopup() {
    const draggableElements = document.querySelectorAll('.draggable');

    draggableElements.forEach(element => {
        addOpenPopupListener(element);
    });

    popupDeleteButton.addEventListener("click", deleteCurrentImage);
}

function openPopup(imageId) {
    currentImageId = imageId;

    const image = dataManager.getImage(imageId);

    if (!image) {
        console.error(`Image with ID ${imageId} not found.`);
        return;
    }

    const popupImageContainer = document.createElement('div');
    popupImageContainer.classList.add('popup-image-container');

    const popupImage = document.createElement('img');
    popupImage.src = image.url;
    popupImage.alt = image.altText;

    popupImageContainer.appendChild(popupImage);

    const imageDescription = document.createElement('p');
    imageDescription.textContent = image.note || "Image description...";

    popupDescription.appendChild(imageDescription);
    popupImageLayer.appendChild(popupImageContainer);

    popup.classList.add("active"); 
    overlay.classList.add("active");
    body.classList.add("body-noscroll");
}

function closePopup() {
    currentImageId = null;

    popup.classList.remove("active");
    overlay.classList.remove("active");
    popupImageLayer.innerHTML = '';
    popupDescription.innerHTML = '';
    body.classList.remove("body-noscroll");
}

function addOpenPopupListener(clickableElement) {
    const imageId = parseInt(clickableElement.id.replace('img', ''));
    clickableElement.addEventListener("click", () => openPopup(imageId));
}

function deleteCurrentImage() {
  console.log(dataManager.toAPIFormat());
  if (!currentImageId) return;
  
  const deleteResult = dataManager.deleteImage(currentImageId);
  
  if (deleteResult.success) {
    const imageElement = document.getElementById(`img${currentImageId}`);
    if (imageElement) {
      imageElement.remove();
    }
    
    closePopup();
    
    console.log(`Image ${currentImageId} deleted successfully`);
    console.log(dataManager.toAPIFormat());
  }
  
  currentImageId = null;
}