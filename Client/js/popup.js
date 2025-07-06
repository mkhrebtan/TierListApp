const popup = document.getElementById("popup");
const overlay = document.getElementById("blurOverlay");
const popupImageLayer = document.getElementById("popupImage");
const popupDescription = document.getElementById("popupDescription");
const popupNoteInput = document.getElementById("imageNoteInput");
const popupDeleteButton = document.getElementById("deleteBtn");
const popupChangeButton = document.getElementById("replaceBtn");
const fileInputChange = document.getElementById("fileInputChange");
const imageNoteInput = document.getElementById("imageNoteInput");
const noteSaveBtn = document.getElementById("noteSaveBtn");
let currentImageId = null;
let currentImageNote = null;
const rankPopup = document.getElementById('rankPopup');
const rankPreview = document.getElementById('rankPreview');
const rankPreviewSpan = document.getElementById('rankPreviewSpan'); 
const rankNameInput = document.getElementById('rankNameInput');
const rankColorPickerBtn = document.getElementById('rankColorPickerBtn');
const rankSaveBtn = document.getElementById('rankSaveBtn');

function initializePopup() {
    const draggableElements = document.querySelectorAll('.draggable');

    draggableElements.forEach(element => {
        addOpenPopupListener(element);
    });

    popupDeleteButton.addEventListener("click", deleteCurrentImage);
    popupChangeButton.addEventListener("click", () => {
        if (fileInputChange) {
            fileInputChange.click();
        }
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
    body.classList.add("body-noscroll");
}

function closePopup() {
    currentImageId = null;

    popup.classList.remove("active");
    overlay.classList.remove("active");
    body.classList.remove("body-noscroll");
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

async function deleteCurrentImage() {
  const confirmation = confirm("Are you sure you want to delete this image? This action cannot be undone.");
  
  if (!confirmation.isConfirmed) {
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

function openRankPopup(rankText, rankColor) {
  rankPreviewSpan.textContent = rankText;
  rankPreview.style.backgroundColor = rankColor;
  rankNameInput.value = rankText;

  rankPopup.classList.add('active');
  overlay.classList.add('active');
  body.classList.add("body-noscroll");
}

function closeRankPopup() {
  currentRankContainer = null;
  currentRow = null;
  currentRankText = null;

  hideSaveButton(rankSaveBtn, rankNameInput);

  rankPopup.classList.remove("active");
  overlay.classList.remove("active");
  body.classList.remove("body-noscroll");
}

function showNewImageAfterChange(image)
{
  const popupImage = document.createElement('img');
  popupImage.src = image.url;
  popupImage.alt = image.altText;

  popupNoteInput.value = image.note;

  popupImageLayer.innerHTML = popupImage.outerHTML;
}