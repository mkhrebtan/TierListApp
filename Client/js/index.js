const body = document.body;
const main = document.body.querySelector('.main');
const listTitle = document.getElementById("listTitle");
const dataManager = new TierListDataManager();
let tierList = null;
let backupImagesDropBox = null;

const apiUrl = 'https://localhost:5001/'

listName = '';
let rows = [];
let images = [];

window.addEventListener('DOMContentLoaded', async () => {
  const urlList = apiUrl + 'lists/1';
  const listData = await makeApiCall(urlList);
  if (!listData) {
    console.error('No data to render');
    return;
  }

  console.log('List data loaded:', listData);

  dataManager.loadData(listData);
  renderList();
  renderBackupImages();
  initializeDragAndDrop();
  initializePopup();
  initializeEditing();
  initializeImageUploading();
});

async function fetchData(url) {
  try {
    const response = await fetch(url);
    if (!response.ok) {
      console.error(`HTTP error! Status: ${response.status}, Error Text: ${(await response.text()).trim()}`);
      return;
    }
    const data = await response.json();
    console.log("Data fetched successfully!");
    return data;
  } catch (error) {
    console.error(`Error occurred: ${error.message}`);
  }
}

async function makeApiCall(url, queryParams = null, method = 'GET', body = null) {
  try {
    const fullUrl = queryParams ? `${url}?${new URLSearchParams(queryParams)}` : url;
    const options = {
      method: method,
      headers: {
        'Content-Type': 'application/json'
      }
    };
    if (body) {
      options.body = JSON.stringify(body);
    }
    
    const response = await fetch(fullUrl, options);
    if (!response.ok) {
      console.error(`HTTP error! Status: ${response.status}, Error Text: ${(await response.text()).trim()}`);
      return null;
    }
    
    if (method === 'DELETE') {
      return { success: true, message: 'Operation successful' };
    }
    else {
      return await response.json();
    }
  } catch (error) {
    console.error(`Error occurred: ${error.message}`);
    return null;
  }
}

function renderList() {
  listTitle.textContent = dataManager.listData.title;

  const listContainer = document.createElement('div');
  listContainer.classList.add('tier-list');
  listContainer.id = 'tier-list-container';

  tierList = listContainer;

  const rows = Array.from(dataManager.listData.rows.values());

  rows.sort((a, b) => a.order - b.order).forEach(row => {
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

    const rowImages = Array.from(dataManager.getRowImages(row.id).values());
    rowImages.sort((a, b) => a.order - b.order).forEach(image => {
      const imageContainer = createImageElement(image);
      rowDropBox.appendChild(imageContainer);
    });

    rowContainer.appendChild(rowRank);
    rowContainer.appendChild(rowDropBox);
    listContainer.appendChild(rowContainer);
  });
  
  // main.appendChild(listHeader);
  main.appendChild(listContainer);
}

function renderBackupImages() {
  const backupContainer = document.createElement('div');
  backupContainer.classList.add('backup-images');

  const dropBox = document.createElement('div');
  dropBox.classList.add('tier-drop-box');
  dropBox.id = 'backup-drop-box';

  backupImagesDropBox = dropBox;

  const backupImages = Array.from(dataManager.getBackupImages().values());
  backupImages.sort((a, b) => a.order - b.order).forEach(image => {
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
  imageElement.draggable = false;

  imageContainer.appendChild(imageElement);
  return imageContainer;
}
