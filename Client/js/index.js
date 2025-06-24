const body = document.body;
const main = document.body.querySelector('.main');
const dataManager = new TierListDataManager();
let tierList = null;
let backupImagesDropBox = null;

listName = '';
let rows = [];
let images = [];

window.addEventListener('DOMContentLoaded', async () => {
  const urlList = './data/list.json';
  const listData = await fetchJSONData(urlList);
  if (!listData) {
    console.error('No data to render');
    return;
  }

  dataManager.loadData(listData);
  renderList();
  renderBackupImages();
  initializeDragAndDrop();
  initializePopup();
  initializeEditing();
});

async function fetchJSONData(url) {
  try {
    const response = await fetch(url);
    if (!response.ok) {
      throw new Error(`HTTP error! Status: ${response.status}`);
    }
    return await response.json();
  } catch (error) {
    console.error('Failed to fetch data:', error);
    return null;
  }
}

function renderList() {
  const listHeader = document.createElement('h2');
  listHeader.textContent = dataManager.listData.name;

  const listContainer = document.createElement('div');
  listContainer.classList.add('tier-list');
  listContainer.id = 'tier-list-container';

  tierList = listContainer;

  dataManager.listData.rows.forEach(row => {
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

    // Get images for this row
    const rowImages = dataManager.getRowImages(row.id);
    rowImages.forEach(image => {
      const imageContainer = createImageElement(image);
      rowDropBox.appendChild(imageContainer);
    });

    rowContainer.appendChild(rowRank);
    rowContainer.appendChild(rowDropBox);
    listContainer.appendChild(rowContainer);
  });
  
  main.appendChild(listHeader);
  main.appendChild(listContainer);
}

function renderBackupImages() {
  const backupContainer = document.createElement('div');
  backupContainer.classList.add('backup-images');

  const dropBox = document.createElement('div');
  dropBox.classList.add('tier-drop-box');
  dropBox.id = 'backup-drop-box';

  backupImagesDropBox = dropBox;

  const backupImages = dataManager.getBackupImages();
  backupImages.forEach(image => {
    const imageContainer = createImageElement(image);
    dropBox.appendChild(imageContainer);
  });

  backupContainer.appendChild(dropBox);
  main.appendChild(backupContainer);
}

function createImageElement(image) {
  const imageContainer = document.createElement('div');
  imageContainer.classList.add('draggable');
  imageContainer.id = `img${image.id}`;
  imageContainer.draggable = true;

  const imageElement = document.createElement('img');
  imageElement.src = image.url;
  imageElement.alt = image.altText;
  imageElement.draggable = false;

  imageContainer.appendChild(imageElement);
  return imageContainer;
}

const fileSelect = document.getElementById("fileSelect"),
fileInput = document.getElementById("fileInput");

fileInput.addEventListener("change", handleFiles, false);

function handleFiles() {
  if (this.files.length) {
    for (const file of this.files) {
      const img = document.createElement("img");
      img.src = URL.createObjectURL(file);
      body.appendChild(img);
    }
  }
}
