const baseUrl = 'https://localhost:5001/';

class TierListDataManager {
  constructor() {
    this.listData = {
      id: null,
      title: '',
      rows: new Map(),
      images: new Map(),
      backupRowId: null,
    };
  }

  clearCache() {
    this.listData.id = null;
    this.listData.title = '';
    this.listData.rows.clear();
    this.listData.images.clear();
    this.listData.backupRowId = null;
  }

  async getTierList(id) {
    const jsonData = await this.makeApiCall(baseUrl + `lists/${id}`);
    if (!jsonData) throw new Error('Tier list fetch failed');
    this.listData.id = jsonData.id;
    this.listData.title = jsonData.title;

    jsonData.rows.forEach(row => {
      this.listData.rows.set(row.id, {
        id: row.id,
        rank: row.rank,
        colorHex: row.colorHex,
        order: row.order,
      });

      row.images.forEach(image => {
        this.listData.images.set(image.id, {
          id: image.id,
          storageKey: image.storageKey,
          url: image.url,
          note: image.note,
          containerId: image.containerId,
          order: image.order,
        });
      });
    });

    this.listData.backupRowId = jsonData.backupRow.id;
    jsonData.backupRow.images.forEach(image => {
      this.listData.images.set(image.id, {
        id: image.id,
        storageKey: image.storageKey,
        url: image.url,
        note: image.note,
        containerId: this.listData.backupRowId,
        order: image.order,
      });
    });

    return jsonData;
  }

  async updateTierListTitle(newTitle) {
    const updateTitleRequestBody = {
      id: this.listData.id,
      title: newTitle,
    };

    const updatedList = await this.makeApiCall(
      baseUrl + `lists/${this.listData.id}`,
      null,
      'PUT',
      updateTitleRequestBody
    );

    if (!updatedList) {
      console.error('Failed to update tier list title');
      return { success: false, message: 'Failed to update title' };
    }

    this.listData.title = updatedList.title;
    return { success: true, message: 'Tier list title updated successfully', list: updatedList };
  }

  async getImage(imageId) {
    return this.listData.images.get(imageId);
  }

  async getImageUploadUrl(fileName, contentType) {
    const params = new URLSearchParams();
    params.append('fileName', fileName);
    params.append('contentType', contentType);

    const uploadUrlResponse = await this.makeApiCall(baseUrl + `images/upload-url`, params, 'GET');
    if (!uploadUrlResponse) {
      console.error('Failed to get image upload URL');
      return { success: false, message: 'Failed to get upload URL' };
    }

    return uploadUrlResponse;
  }

  async addImage(imageToSave) {
    const saveResponse = await this.makeApiCall(baseUrl + 'images', null, 'POST', imageToSave);
    if (!saveResponse) {
      console.error('Failed to save image data');
      return { success: false, message: 'Failed to save image' };
    }

    this.listData.images.set(saveResponse.id, saveResponse);
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

    const updatedImage = await this.makeApiCall(
      baseUrl + `images/${imageId}/reorder`,
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

    const updatedImage = await this.makeApiCall(
      baseUrl + `images/${imageId}/move`,
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

    const updatedImage = await this.makeApiCall(
      baseUrl + `images/${imageId}/note`,
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

    const updatedImage = await this.makeApiCall(
      baseUrl + `images/${imageId}/url`,
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

    const deleteResponse = await this.makeApiCall(
      baseUrl + `images/${imageId}`,
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

  async addRow() {
    const addRowRequestBody = {
      listId: this.listData.id,
      rank: 'New',
      colorHex: '#FFFFFF',
      order: this.listData.rows.size + 1,
    };

    const newRow = await this.makeApiCall(baseUrl + 'rows', null, 'POST', addRowRequestBody);
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

    const updatedRow = await this.makeApiCall(
      baseUrl + `rows/${rowId}/rank`,
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

    const updatedRow = await this.makeApiCall(
      baseUrl + `rows/${rowId}/color`,
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

    const updatedRow = await this.makeApiCall(
      baseUrl + `rows/${rowId}/order`,
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

    const deleteResponse = await this.makeApiCall(
      baseUrl + `rows/${rowId}`,
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

  async getRow(rowId) {
    return this.listData.rows.get(rowId);
  }

  async makeApiCall(url, queryParams = null, method = 'GET', body = null) {
    try {
      const fullUrl = queryParams ? `${url}?${new URLSearchParams(queryParams)}` : url;
      const options = {
        method: method,
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${authManager.getTokenInfo()?.accessToken || ''}`
        }
      };
      if (body) {
        options.body = JSON.stringify(body);
      }

      const response = await fetch(fullUrl, options);
      
      if (response.status === 401) {
        console.warn('Unauthorized request, checking token validity');
        const tokenValidation = await authManager.tryRefreshToken();
        if (tokenValidation.success === false) {
          console.error('Token refresh failed, redirecting to login');
          authManager.logout();
          return null;
        }
        // Retry the original request with the new token
        options.headers['Authorization'] = `Bearer ${authManager.getTokenInfo()?.accessToken || ''}`;
        const retryResponse = await fetch(fullUrl, options);
        
        if (!retryResponse.ok) {
          console.error(`HTTP error! Status: ${retryResponse.status}, Error Text: ${(await retryResponse.text()).trim()}`);
          return null;
        }
        
        if (method === 'DELETE') {
          return { success: true, message: 'Operation successful' };
        }
        else {
          return await retryResponse.json();
        }
      }

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
}

class TierListLibraryDataManager {
  constructor() {
    this.lists = new Map();
  }

  // Method to clear cache and force fresh data fetch
  clearCache() {
    this.lists.clear();
  }

  async getAllLists() {
    const jsonData = await this.makeApiCall(baseUrl + 'lists');
    if (!jsonData || jsonData.success === false) throw new Error('Failed to fetch tier lists');

    // Clear and repopulate the lists map with fresh data from API
    this.lists.clear();
      jsonData.data.forEach(list => {
        this.lists.set(list.id, {
          id: list.id,
          title: list.title,
          created: list.created,
          lastModified: list.lastModified,
        });
    });

    return Array.from(this.lists.values()).sort((a, b) => new Date(a.created) - new Date(b.created));
  }

  async addList(title) {
    const addListRequestBody = {
      title: title,
    };

    const newList = await this.makeApiCall(baseUrl + 'lists', null, 'POST', addListRequestBody);
    if (!newList) {
      console.error('Failed to create new list');
      return { success: false, message: 'Failed to create list' };
    }

    this.lists.set(newList.id, newList);
    return { success: true, message: 'List created successfully', list: newList };
  }

  async updateListTitle(listId, newTitle) {
    const updateTitleRequestBody = {
      id: listId,
      title: newTitle,
    };

    const updatedList = await this.makeApiCall(
      baseUrl + `lists/${listId}`,
      null,
      'PUT',
      updateTitleRequestBody
    );

    if (!updatedList || updatedList.success === false) {
      console.error(`Failed to update title for list with ID ${listId}`);
      return { success: false, message: 'Failed to update list title' };
    }

    this.lists.set(listId, updatedList.data);
    return { success: true, message: 'List title updated successfully', list: updatedList };
  }

  async deleteList(listId) {
    const deleteResponse = await this.makeApiCall(
      baseUrl + `lists/${listId}`,
      null,
      'DELETE',
      null
    );

    if (!deleteResponse || deleteResponse.success === false) {
      console.error(`Failed to delete list with ID ${listId}`);
      return { success: false, message: 'Failed to delete list' };
    }

    this.lists.delete(listId);
    return { success: true, message: 'List deleted successfully' };
  }

  async makeApiCall(url, queryParams = null, method = 'GET', body = null) {
    try {
      const fullUrl = queryParams ? `${url}?${new URLSearchParams(queryParams)}` : url;
      const options = {
        method: method,
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${authManager.getTokenInfo()?.accessToken || ''}`
        }
      };
      if (body) {
        options.body = JSON.stringify(body);
      }

      const response = await fetch(fullUrl, options);

      if (response.status === 401) {
        console.warn('Unauthorized request, checking token validity');
        const tokenValidation = await authManager.tryRefreshToken();
        if (tokenValidation.success === false) {
          console.error('Token refresh failed, redirecting to login');
          authManager.logout();
          return { success: false, message: 'Authentication failed' };
        }
        // Retry the original request with the new token
        options.headers['Authorization'] = `Bearer ${authManager.getTokenInfo()?.accessToken || ''}`;
        const retryResponse = await fetch(fullUrl, options);

        if (!retryResponse.ok) {
          return { success: false, message: (await retryResponse.text()).trim() };
        }

        if (method === 'DELETE') {
          return { success: true, message: 'Operation successful' };
        }
        else {
          return { success: true, data: await retryResponse.json() };
        }
      }

      if (!response.ok) {
        return { success: false, message: (await response.text()).trim() };
      }

      if (method === 'DELETE') {
        return { success: true, message: 'Operation successful' };
      }
      else {
        return { success: true, data: await response.json() };
      }
    } catch (error) {
      console.error(`Error occurred: ${error.message}`);
      return null;
    }
  }
}

class AuthManager {
  async register(username, password) {
    const registerRequestBody = {
      username: username,
      password: password,
    };

    const registerResponse = this.makeApiCall(baseUrl + 'auth/register', null, 'POST', registerRequestBody);
    if (!registerResponse || registerResponse.success === false) {
      console.error('Registration failed');
      return { success: false, message: registerResponse.message || 'Registration failed' };
    }

    return { success: true, message: 'Registration successful' };
  }

  async login(username, password) {
    const loginRequestBody = {
      username: username,
      password: password,
    };

    const loginResponse = await this.makeApiCall(baseUrl + 'auth/login', null, 'POST', loginRequestBody);
    if (!loginResponse || loginResponse.success === false) {
      console.error('Login failed');
      return { success: false, message: loginResponse.message || 'Login failed' };
    }

    // Store the token in localStorage for future requests
    this.setTokenInfo(loginResponse.data);
    localStorage.setItem('username', username); // Store username for display purposes
    return { success: true, message: 'Login successful' };
  }

  getTokenInfo() {
    const tokenInfo = localStorage.getItem('tokenInfo');
    return tokenInfo ? JSON.parse(tokenInfo) : null;
  }

  setTokenInfo(apiResponse) {
    if (!apiResponse || !apiResponse.accessToken || !apiResponse.refreshToken) {
      console.error('Invalid token information provided');
      return;
    }

    const tokenInfo = {
      accessToken: apiResponse.accessToken,
      refreshToken: apiResponse.refreshToken,
      accessExpiresAt: apiResponse.accessExpiresAt,
      refreshExpiresAt: apiResponse.refreshExpiresAt,
    };

    localStorage.setItem('tokenInfo', JSON.stringify(tokenInfo));
  }

  async isAuthenticated() {
    if (this.isAccessTokenValid()) {
      return true;
    }

    if (this.isRefreshTokenValid()) {
      const refreshResult = await this.tryRefreshToken();
      if (refreshResult.success) {
        return true;
      } else {
        console.warn('Refresh token is invalid');
        return false;
      }
    }

    console.warn('No valid access or refresh token found');
    return false;
  }

  isAccessTokenValid() {
    const tokenInfo = this.getTokenInfo();
    if (!tokenInfo || !tokenInfo.accessToken) return false;

    const accessExpiresAt = new Date(tokenInfo.accessExpiresAt);
    const now = new Date();
    // Add 5 minute buffer to refresh before actual expiration
    const bufferTime = 5 * 60 * 1000; // 5 minutes in milliseconds
    
    if (accessExpiresAt.getTime() - now.getTime() < bufferTime) {
      console.warn('Access token will expire soon or has expired');
      return false;
    }
    return true;
  }

  isRefreshTokenValid() {
    const tokenInfo = this.getTokenInfo();
    if (!tokenInfo || !tokenInfo.refreshToken) return false;
    
    const refreshExpiresAt = new Date(tokenInfo.refreshExpiresAt);
    const now = new Date();
    
    if (refreshExpiresAt.getTime() <= now.getTime()) {
      console.warn('Refresh token has expired');
      return false;
    }
    return true;
  }

  async tryRefreshToken() {
    const tokenInfo = this.getTokenInfo();
    
    if (!tokenInfo || !tokenInfo.refreshToken) {
      console.error('No refresh token available');
      return { success: false, message: 'No refresh token available' };
    }

    if (!this.isRefreshTokenValid()) {
      console.error('Refresh token has expired');
      this.logout();
      return { success: false, message: 'Refresh token expired' };
    }

    try {
      const refreshResponse = await this.makeApiCall(baseUrl + 'auth/refresh', null, 'POST', {
        refreshToken: tokenInfo.refreshToken,
      });

      if (!refreshResponse || refreshResponse.success === false) {
        console.error('Failed to refresh token:', refreshResponse?.message);
        this.logout();
        return { success: false, message: refreshResponse?.message || 'Failed to refresh token' };
      }

      this.setTokenInfo(refreshResponse.data);
      console.log('Token refreshed successfully');
      return { success: true, message: 'Token refreshed successfully' };
    } catch (error) {
      console.error('Error during token refresh:', error);
      this.logout();
      return { success: false, message: 'Token refresh failed due to network error' };
    }
  }

  logout() {
    localStorage.removeItem('tokenInfo');
    localStorage.removeItem('username');
    window.location.hash = '#/login';
  }

  async makeApiCall(url, queryParams = null, method = 'GET', body = null) {
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
        return { success: false, message: (await response.text()).trim() };
      }

      if (method === 'DELETE') {
        return { success: true, message: 'Operation successful' };
      }
      else {
        return { success: true, data: await response.json() };
      }
    } catch (error) {
      console.error(`Error occurred: ${error.message}`);
      return null;
    }
  }
}

export const dataManager = new TierListDataManager();
export const tierListLibraryDataManager = new TierListLibraryDataManager();
export const authManager = new AuthManager();