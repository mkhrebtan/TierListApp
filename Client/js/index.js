const body = document.body;
const main = document.body.querySelector('.main');
const dataManager = new TierListDataManager();

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

  const rows = Array.from(dataManager.listData.rows.values())
    .sort((a, b) => a.order - b.order);

  rows.forEach(row => {
    const rowContainer = document.createElement('div');
    rowContainer.classList.add('tier-row');
    
    const rowRank = document.createElement('div');
    rowRank.classList.add('tier-rank');
    rowRank.textContent = row.rank;
    rowRank.style.backgroundColor = row.colorHex;

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
