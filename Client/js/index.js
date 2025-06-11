const main = document.body.querySelector('.main');

window.addEventListener('DOMContentLoaded', async () => {
  const urlList = './data/list.json';
  const urlBackupImages = './data/images.json';
  const listData = await fetchJSONData(urlList);
  const backupImages = await fetchJSONData(urlBackupImages);
  renderList(listData);
  renderBackupImages(backupImages);
  initializeDragAndDrop();
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

function renderList(listData)
{
  if (!listData) {
    console.error('No data to render');
    return;
  }

  const listHeader = document.createElement('h2');
  listHeader.textContent = listData.name;

  const listContainer = document.createElement('div');
  listContainer.classList.add('tier-list');

  listData.rows.forEach(row => {
    const rowContainer = document.createElement('div');
    rowContainer.classList.add('tier-row');
    
    const rowRank = document.createElement('div');
    rowRank.classList.add('tier-rank');
    rowRank.textContent = row.rank;
    rowRank.style.backgroundColor = row.colorHex;

    const rowDropBox = document.createElement('div');
    rowDropBox.classList.add('tier-drop-box');
    rowDropBox.id = `drop-box-${row.rank}`;

    row.images.forEach(image => {
      const imageContainer = document.createElement('div');
      imageContainer.classList.add('draggable');
      imageContainer.id = `img${image.id}`;
      imageContainer.draggable = true;

      const imageElement = document.createElement('img');
      imageElement.src = image.url;
      imageElement.alt = image.altText;
      imageElement.draggable = false;

      imageContainer.appendChild(imageElement);
      rowDropBox.appendChild(imageContainer);
    });

    rowContainer.appendChild(rowRank);
    rowContainer.appendChild(rowDropBox);
    listContainer.appendChild(rowContainer);
    main.appendChild(listHeader);
    main.appendChild(listContainer);
  });
}

function renderBackupImages(backupImages) {
  if (!backupImages) {
    console.error('No backup images to render');
    return;
  }

  const backupContainer = document.createElement('div');
  backupContainer.classList.add('tier-drop-box');

  backupImages.forEach(image => {
    const imageContainer = document.createElement('div');
    imageContainer.classList.add('draggable');
    imageContainer.id = `img${image.id}`;
    imageContainer.draggable = true;

    const imageElement = document.createElement('img');
    imageElement.src = image.url;
    imageElement.alt = image.altText;
    imageElement.draggable = false;

    imageContainer.appendChild(imageElement);
    backupContainer.appendChild(imageContainer);
  });

  main.appendChild(backupContainer);
}