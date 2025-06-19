class TierListDataManager {
  constructor() {
    this.listData = {
      id: null,
      name: '',
      rows: new Map(), // rowId -> row data
      images: new Map(), // imageId -> image data
      rowImages: new Map(), // rowId -> [imageIds]
      backupImages: new Set() // imageIds
    };
  }

  // Initialize from API/JSON
  loadData(jsonData) {
    this.listData.id = jsonData.id;
    this.listData.name = jsonData.name;
    
    // Store rows
    jsonData.rows.forEach(row => {
      this.listData.rows.set(row.id, {
        id: row.id,
        rank: row.rank,
        colorHex: row.colorHex,
      });
      
      // Initialize row images array
      this.listData.rowImages.set(row.id, []);
      
      // Store images and their row assignments
      row.images.forEach(image => {
        this.listData.images.set(image.id, {
          id: image.id,
          url: image.url,
          altText: image.altText,
          note: image.note,
        });
        this.listData.rowImages.get(row.id).push(image.id);
      });
    });
    
    // Store backup images
    jsonData.backupImages.forEach(image => {
      this.listData.images.set(image.id, {
        id: image.id,
        url: image.url,
        altText: image.altText,
        note: image.note
      });
      this.listData.backupImages.add(image.id);
    });
  }

  updateImageOrder(rowId, imageId, oldIndex, newIndex) {
    const image = this.listData.images.get(imageId);
    if (!image) {
      console.error(`Image with ID ${imageId} not found.`);
      return { success: false, message: 'Image not found' };
    }

    let rowImagesIds = null;
    let inBackup = false;
    if (rowId === 'backup-drop-box') {
      rowImagesIds = Array.from(this.listData.backupImages);
      inBackup = true;
    }
    else {
      rowImagesIds = this.listData.rowImages.get(rowId);
      if (!rowImagesIds) {
        console.error(`Row with ID ${rowId} not found.`);
        return { success: false, message: 'Row not found' };
      }
    }

    const [element] = rowImagesIds.splice(oldIndex, 1); 
    rowImagesIds.splice(newIndex, 0, element);

    if (inBackup) {
      this.listData.backupImages.clear();
      rowImagesIds.forEach(id => this.listData.backupImages.add(id));
    }

    return { success: true, message: 'Image order updated successfully' };
  }

  moveImage(imageId, fromRowId, toRowId, newIndex) {
    if (fromRowId === 'backup-drop-box') {
      this.listData.backupImages.delete(imageId);
    } else {
      const fromArray = this.listData.rowImages.get(fromRowId);
      const index = fromArray.indexOf(imageId);
      if (index > -1) fromArray.splice(index, 1);
    }
    
    let lastIndex = 0;
    if (toRowId === 'backup-drop-box') {
      this.listData.backupImages.add(imageId);
      lastIndex = this.listData.backupImages.size - 1;
    } else {
      const toArray = this.listData.rowImages.get(toRowId);
      toArray.push(imageId);
      lastIndex = toArray.length - 1;
    }
    
    this.updateImageOrder(toRowId, imageId, lastIndex, newIndex);
  }

  getRowImages(rowId) {
    const imageIds = this.listData.rowImages.get(rowId) || [];
    return imageIds.map(id => this.listData.images.get(id));
  }

  getBackupImages() {
    return Array.from(this.listData.backupImages)
      .map(id => this.listData.images.get(id));
  }

  getImage(imageId) {
    return this.listData.images.get(imageId);
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

  deleteImage(imageId) {
    const image = this.listData.images.get(imageId);
    if (!image) {
      console.error(`Image with ID ${imageId} not found.`);
      return { success: false, message: 'Image not found' };
    }

    this.listData.rowImages.forEach((imageIds, rowId) => {
      const index = imageIds.indexOf(imageId);
      if (index > -1) {
        imageIds.splice(index, 1);
      }
    });

    this.listData.backupImages.delete(imageId);

    this.listData.images.delete(imageId);
    return { success: true, message: 'Image deleted successfully' };
  }

  updateRankColor(rowId, newColorHex) {
    const row = this.listData.rows.get(rowId);
    if (!row) {
      console.error(`Row with ID ${rowId} not found.`);
      return { success: false, message: 'Row not found' };
    }

    row.colorHex = newColorHex;
    return { success: true, message: 'Row color updated successfully' };
  }

  updateRowOrder(oldIndex, newIndex) {
    const rowsArray = Array.from(this.listData.rows.values());
    const [element] = rowsArray.splice(oldIndex, 1); 
    rowsArray.splice(newIndex, 0, element);
    this.listData.rows.clear();
    this.listData.rows = new Map(rowsArray.map(row => [row.id, row]));
    return { success: true, message: 'Row order updated successfully' };
  }

  addRow() {
    const newRow = {
      id: this.listData.rows.size + 1,
      rank: 'New',
      colorHex: '#FFFFFF',
      images: []
    };
    this.listData.rows.set(newRow.id, newRow);
    this.listData.rowImages.set(newRow.id, []);
    return { success: true, message: 'Row added successfully', row: newRow };
  }

  deleteRow(rowId) {
    const row = this.listData.rows.get(rowId);
    if (!row) {
      console.error(`Row with ID ${rowId} not found.`);
      return { success: false, message: 'Row not found' };
    }

    const imageIds = this.listData.rowImages.get(rowId) || [];
    imageIds.forEach(imageId => {
      this.listData.backupImages.add(imageId);
    });

    this.listData.rows.delete(rowId);
    this.listData.rowImages.delete(rowId);
    
    return { success: true, message: 'Row deleted successfully' };
  }
}