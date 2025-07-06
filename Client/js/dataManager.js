class TierListDataManager {
  constructor() {
    this.listData = {
      id: null,
      title: '',
      rows: new Map(), // rowId -> row data
      images: new Map(), // imageId -> image data
      rowImages: new Map(), // rowId -> [imageIds]
      backupRowId: null,
      backupImages: new Set() // imageIds
    };
  }

  loadData(jsonData) {
    this.listData.id = jsonData.id;
    this.listData.title = jsonData.title;
    
    jsonData.rows.forEach(row => {
      this.listData.rows.set(row.id, {
        id: row.id,
        rank: row.rank,
        colorHex: row.colorHex,
        order: row.order,
      });
      
      this.listData.rowImages.set(row.id, []);
      
      row.images.forEach(image => {
        this.listData.images.set(image.id, {
          id: image.id,
          storageKey: image.storageKey,
          url: image.url,
          note: image.note,
          containerId: image.containerId,
          order: image.order,
        });
        this.listData.rowImages.get(row.id).push(image.id);
      });
    });
    
    this.listData.backupRowId = jsonData.backupRow.id;

    jsonData.backupRow.images.forEach(image => {
      this.listData.images.set(image.id, {
        id: image.id,
        storageKey: image.storageKey,
        url: image.url,
        note: image.note,
        containerId: image.containerId,
        order: image.order,
      });
      this.listData.backupImages.add(image.id);
    });
  }

  async getImage(imageId) {
    return this.listData.images.get(imageId);
  }

  async addImage(imageToSave)
  {
    const saveResponse = await makeApiCall(apiUrl + 'images', null, 'POST', imageToSave);
    if (!saveResponse) {
      console.error('Failed to save image data');
      return { success: false, message: 'Failed to save image' };
    }

    dataManager.listData.images.set(imageToSave.id, saveResponse);
    return { success: true, message: 'Image saved successfully', image: saveResponse };
  }

  async updateImageOrder(rowId, imageId, newIndex) {
    if (rowId === 'backup-drop-box') {
      rowId = this.listData.backupRowId;
    }
    
    const moveImageRequestBody = {
      id: imageId,
      listId: this.listData.id,
      containerId: rowId,
      order: newIndex + 1,
    };

    const updatedImage = await makeApiCall(
      apiUrl + `images/${imageId}/reorder`,
      null,
      'PUT',
      moveImageRequestBody
    );

    if (!updatedImage) {
      console.error(`Failed to update image order for ID ${imageId}`);
      return { success: false, message: 'Failed to update image order' };
    }

    this.listData.images.set(imageId, updatedImage);
    return { success: true, message: 'Image order updated successfully' };
  }

  async moveImage(imageId, fromRowId, toRowId, newIndex) {
    let fromContainerId = null;
    if (fromRowId === 'backup-drop-box') {
      fromContainerId = this.listData.backupRowId;
    } else {
      fromContainerId = fromRowId;
    }
    
    let toContainerId = null;
    if (toRowId === 'backup-drop-box') {
      toContainerId = this.listData.backupRowId;
    }
    else {
      toContainerId = toRowId;
    }

    const moveImageRequestBody = {
      id: imageId,
      listId: this.listData.id,
      fromContainerId: fromContainerId,
      toContainerId: toContainerId,
      order: newIndex + 1,
    };

    const updatedImage = await makeApiCall(
      apiUrl + `images/${imageId}/move`,
      null,
      'PUT',
      moveImageRequestBody
    );

    if (!updatedImage) {
      console.error(`Failed to move image with ID ${imageId}`);
      return { success: false, message: 'Failed to move image' };
    }

    this.listData.images.set(imageId, updatedImage);
    return { success: true, message: 'Image moved successfully' };
  }

  async updateImageNote(imageId, newNote) {
    const image = this.listData.images.get(imageId);
    const updateImageNoteRequestBody = {
      id: imageId,
      listId: this.listData.id,
      containerId: image.containerId,
      note: newNote,
    };

    const updatedImage = await makeApiCall(
      apiUrl + `images/${imageId}/note`,
      null,
      'PUT',
      updateImageNoteRequestBody
    );

    if (!updatedImage) {
      console.error(`Failed to update note for image with ID ${imageId}`);
      return { success: false, message: 'Failed to update image note' };
    }

    this.listData.images.set(imageId, updatedImage);
    return { success: true, message: 'Image note updated successfully', image: updatedImage };
  }

  async updateImageUrl(imageId, newUrl) {
    const image = this.listData.images.get(imageId);
    const updateImageUrlRequestBody = {
      id: imageId,
      listId: this.listData.id,
      containerId: image.containerId,
      url: newUrl,
    };

    const updatedImage = await makeApiCall(
      apiUrl + `images/${imageId}/url`,
      null,
      'PUT',
      updateImageUrlRequestBody
    );

    if (!updatedImage) {
      console.error(`Failed to update URL for image with ID ${imageId}`);
      return { success: false, message: 'Failed to update image URL' };
    }

    this.listData.images.set(imageId, updatedImage);
    return { success: true, message: 'Image URL updated successfully', image: updatedImage };
  }

  async deleteImage(imageId) {
    const image = this.listData.images.get(imageId);
    const params = new URLSearchParams();
    params.append('listId', this.listData.id);
    params.append('containerId', image.containerId);

    const deleteResponse = await makeApiCall(
      apiUrl + `images/${imageId}`,
      params,
      'DELETE',
      null
    );

    if (!deleteResponse) {
      console.error(`Failed to delete image with ID ${imageId}`);
      return { success: false, message: 'Failed to delete image' };
    }
    else {
      this.listData.images.delete(imageId);
      return { success: true, message: 'Image deleted successfully' };
    }
  }

  async addRow()
  {
    const addRowRequestBody = {
      listId: this.listData.id,
      rank: 'New',
      colorHex: '#FFFFFF',
      order: this.listData.rows.size + 1,
    };

    const newRow = await makeApiCall(apiUrl + 'rows', null, 'POST', addRowRequestBody);
    if (!newRow) {
      console.error('Failed to add new row');
      return { success: false, message: 'Failed to add row' };
    }

    this.listData.rows.set(newRow.id, newRow);
    return { success: true, message: 'Row added successfully', row: newRow };
  }

  async updateRowRank(rowId, newRank) {
    const updateRankRequestBody = {
      id: rowId,
      listId: this.listData.id,
      rank: newRank,
    };

    const updatedRow = await makeApiCall(
      apiUrl + `rows/${rowId}/rank`,
      null,
      'PUT',
      updateRankRequestBody
    );

    if (!updatedRow) {
      console.error(`Failed to update rank for row with ID ${rowId}`);
      return { success: false, message: 'Failed to update row rank' };
    }

    this.listData.rows.set(rowId, updatedRow);
    return { success: true, message: 'Row rank updated successfully', row: updatedRow };
  }

  async updateRowColor(rowId, newColorHex) {
    const updateColorRequestBody = {
      id: rowId,
      listId: this.listData.id,
      colorHex: newColorHex,
    };

    const updatedRow = await makeApiCall(
      apiUrl + `rows/${rowId}/color`,
      null,
      'PUT',
      updateColorRequestBody
    );

    if (!updatedRow) {
      console.error(`Failed to update color for row with ID ${rowId}`);
      return { success: false, message: 'Failed to update row color' };
    }

    this.listData.rows.set(rowId, updatedRow);
    return { success: true, message: 'Row color updated successfully', row: updatedRow };
  }

  async updateRowOrder(rowId, newOrder) {
    const updateOrderRequestBody = {
      id: rowId,
      listId: this.listData.id,
      order: newOrder,
    };

    const updatedRow = await makeApiCall(
      apiUrl + `rows/${rowId}/order`,
      null,
      'PUT',
      updateOrderRequestBody
    );

    if (!updatedRow) {
      console.error(`Failed to update order for row with ID ${rowId}`);
      return { success: false, message: 'Failed to update row order' };
    }

    this.listData.rows.set(rowId, updatedRow);
    return { success: true, message: 'Row order updated successfully', row: updatedRow };
  }

  async deleteRow(rowId) {
    const deleteParams = new URLSearchParams();
    deleteParams.append('listId', this.listData.id);
    deleteParams.append('isDeleteWithImages', true);

    const deleteResponse = await makeApiCall(
      apiUrl + `rows/${rowId}`,
      deleteParams,
      'DELETE',
      null
    );

    if (!deleteResponse) {
      console.error(`Failed to delete row with ID ${rowId}`);
      return { success: false, message: 'Failed to delete row' };
    }

    this.listData.rows.delete(rowId);
    return { success: true, message: 'Row deleted successfully' };
  }

  getRowImages(rowId) {
    const imageIds = this.listData.rowImages.get(rowId) || [];
    return imageIds.map(id => this.listData.images.get(id));
  }

  getBackupImages() {
    return Array.from(this.listData.backupImages)
      .map(id => this.listData.images.get(id));
  }

  toAPIFormat() {
    const rows = Array.from(this.listData.rows.values()).map(row => ({
      ...row,
      images: this.getRowImages(row.id)
    }));
    
    return {
      id: this.listData.id,
      name: this.listData.name,
      rows,
      backupImages: this.getBackupImages()
    };
  }

  getRow(rowId) {
    return this.listData.rows.get(rowId);
  }
}