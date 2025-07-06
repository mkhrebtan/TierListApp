const fileSelect = document.getElementById("fileSelect"),
fileInput = document.getElementById("fileInput");

function initializeImageUploading() {
    fileInput.addEventListener("change", handleImageUpload, false);
    fileSelect.addEventListener("click", (e) => {
        if (fileInput) {
            fileInput.click();
        }
    }, false,);
    fileInputChange.addEventListener("change", handleImageChange, false);
}

async function handleImageUpload() {
  if (this.files.length) {
    const file = this.files[0];
    
    const params = new URLSearchParams();
    params.append('fileName', file.name);
    params.append('contentType', file.type);

    const uploadRequestUrl = apiUrl + `images/upload-url`;
    const uploadUrlResponse = await makeApiCall(uploadRequestUrl, params, 'GET');
    console.log('Upload URL:', uploadUrlResponse.url.trim());

    const uploadResponse = await fetch(uploadUrlResponse.url, {
      headers: {
        'Content-Type': file.type,
        'x-amz-meta-file-name': file.name,
      },
      method: 'PUT',
      body: file
    })

    if (!uploadResponse.ok) {
      console.error(`Upload failed: ${uploadResponse.status} ${uploadResponse.statusText}`);
      return;
    }

    const imageUrl = uploadUrlResponse.url.split('?')[0];
    console.log('Image URL:', imageUrl);

    const imageToSave = {
      storageKey: uploadUrlResponse.storageKey,
      url: imageUrl,
      order: backupImagesDropBox.children.length + 1,
      note: "",
      containerId: dataManager.listData.backupRowId,
      listId: dataManager.listData.id,
    };

    const imageSaveResult = await dataManager.addImage(imageToSave);
    if (!imageSaveResult.success) {
      console.error('Failed to save image:', imageSaveResult.message);
      return;
    }

    const imageElement = createImageElement(imageSaveResult.image);
    addOpenPopupListener(imageElement);
    backupImagesDropBox.appendChild(imageElement);
  }
}

async function handleImageChange()
{
  if (this.files.length) {
    if (!currentImageId) {
      alert('Something went wrong, please try again.');
      return;
    }

    const file = this.files[0];
    
    const params = new URLSearchParams();
    params.append('fileName', file.name);
    params.append('contentType', file.type);

    const uploadRequestUrl = apiUrl + `images/upload-url`;
    const uploadUrlResponse = await makeApiCall(uploadRequestUrl, params, 'GET');

    const uploadResponse = await fetch(uploadUrlResponse.url, {
      headers: {
        'Content-Type': file.type,
        'x-amz-meta-file-name': file.name,
      },
      method: 'PUT',
      body: file
    })

    if (!uploadResponse.ok) {
      console.error(`Upload failed: ${uploadResponse.status} ${uploadResponse.statusText}`);
      return;
    }

    const imageUrl = uploadUrlResponse.url.split('?')[0];

    const imageUpdateResult = await dataManager.updateImageUrl(currentImageId, imageUrl);
    if (!imageUpdateResult.success) {
      alert('Failed to update image, please try again.');
      return;
    }
    
    const existingImageElement = document.getElementById(`img${currentImageId}`);
    if (existingImageElement) {
      existingImageElement.querySelector('img').src = imageUrl;
    }

    showNewImageAfterChange(imageUpdateResult.image);
  }
}