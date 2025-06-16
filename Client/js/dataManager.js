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
        order: row.order
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
          order: image.order
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

  updateImageOrder(rowId, imageId, newIndex) {
    const image = this.listData.images.get(imageId);
    if (!image) {
      console.error(`Image with ID ${imageId} not found.`);
      return null;
    }

    const rowImagesIds = this.listData.rowImages.get(rowId);
    if (!rowImagesIds) {
      console.error(`Row with ID ${rowId} not found.`);
      return;
    }

    const rowImages = rowImagesIds.map(id => this.listData.images.get(id));

    const movedImages = rowImages.filter(image => image.order >= newIndex + 1);
    movedImages.forEach(img => {
      img.order += 1;
    });

    image.order = newIndex + 1;
  }

  // Image moved between rows or to/from backup
  moveImage(imageId, fromRowId, toRowId, newIndex) {
    if (fromRowId === 'backup-drop-box') {
      this.listData.backupImages.delete(imageId);
    } else {
      const fromArray = this.listData.rowImages.get(fromRowId);
      const index = fromArray.indexOf(imageId);
      if (index > -1) fromArray.splice(index, 1);
    }
    
    if (toRowId === 'backup-drop-box') {
      this.listData.backupImages.add(imageId);
    } else {
      const toArray = this.listData.rowImages.get(toRowId);
      toArray.push(imageId);
    }
    
    this.updateImageOrder(toRowId, imageId, newIndex);
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

  // Generate API-ready data
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
}