import { dataManager, authManager } from '../dataManager.js';
import { setupDragAndDrop, setupImageUpload, setupListEditing, setupPopupForImages } from '../interactions.js';

export async function renderList(id) {
    const app = document.getElementById('app');

    if (!authManager.isAuthenticated()) {
        location.hash = '#/login';
        authManager.logout();
        return;
    }

    try {
        dataManager.clearCache(); // Clear any cached data for the list
        const list = await dataManager.getTierList(id);
        app.innerHTML = `
        <div class="header">
            <button class="back-btn" id="backBtn" onclick="location.hash = '#/'">
                <i class="fas fa-arrow-left fa-xl"></i>
            </button>
            <h2 contenteditable="true" style="padding: 5px;" id="listTitle">${list.title}</h2>
            <button class="list-title-input-save hidden" id="listTitleSaveBtn">
                <i class="fa-solid fa-check fa-xl"></i>
            </button>
        </div>
        <div class="main">
            <div class="tier-list" id="tier-list-container">
                ${list.rows.sort((a, b) => a.order - b.order).map(row => `
                    <div class="tier-row" id="row-${row.id}">
                        <div class="tier-rank" style="background-color: ${row.colorHex}">
                            <span>${row.rank}</span>
                        </div>
                        <div class="tier-drop-box" id="drop-box-${row.id}">
                            ${row.images.sort((a, b) => a.order - b.order).map(image => `
                                <div class="draggable" id="img${image.id}" draggable="true">
                                    <img src="${image.url}" draggable="false"/>
                                </div>
                            `).join('')}
                        </div>
                    </div>
                `).join('')}
            </div>
            <div class="backup-images">
                <div class="tier-drop-box" id="backup-drop-box">
                    ${list.backupRow.images.sort((a, b) => a.order - b.order).map(image => `
                        <div class="draggable" id="img${image.id}" draggable="true">
                            <img src="${image.url}" draggable="false"/>
                        </div>
                    `).join('')}
                </div>
            </div>
            <div class="main-footer">
                <input type="file" id="fileInput" accept="image/*" />
                <button id="fileSelect" type="button">
                    Add Image
                </button>
                <button class="edit-save-btn" id="editBtn">
                    Edit List
                </button>
            </div>
        </div>
        <div class="blur-overlay" id="blurOverlay"></div>
        <div class="popup imagePop" id="popup">
            <div class="popup-header" id="popupHeader">
                <h3>Tier Image</h3>
                <button class="close-popup-btn imagePop" id="closePopup">
                    <i class="fa-solid fa-xmark fa-2xl"></i>
                </button>
            </div>
            <div class="popup-image" id="popupImage">
                
            </div>
            <div class="popup-description rank-popup-input-container" id="popupDescription">
                <input class="popup-input" type="text" id="imageNoteInput" placeholder="Image note..."/>
                <button class="popup-input-save hidden" id="noteSaveBtn">
                    <i class="fa-solid fa-check fa-xl"></i>
                </button>
            </div>
            <div class="popup-footer" id="popupFooter">
                <input type="file" id="fileInputChange" accept="image/*" />
                <button id="replaceBtn">
                    <i class="fas fa-sync-alt"></i> 
                    Change
                </button>
                <button id="deleteBtn">
                    <i class="fa-solid fa-trash"></i>
                    Delete
                </button>
            </div> 
        </div>
        <div class="popup rank" id="rankPopup">
            <div class="popup-header" id="rankPopupHeader">
                <h3>Edit Rank</h3>
                <button class="close-popup-btn" id="rankClosePopup">
                    <i class="fa-solid fa-xmark fa-2xl"></i>
                </button>
            </div>
            <div class="rank-popup-content" id="rankPopupContent">
                <div class="tier-rank preview" id="rankPreview">
                    <span id="rankPreviewSpan">A</span>
                </div>
                <div class="rank-popup-rank-edit">
                    <div class="rank-popup-input-container">
                        <input class="popup-input" type="text" id="rankNameInput" placeholder="Enter rank name"/>
                        <button class="popup-input-save hidden" id="rankSaveBtn">
                            <i class="fa-solid fa-check fa-xl"></i>
                        </button>
                    </div>
                    <button class="rank-popup-color" id="rankColorPickerBtn">
                        Change Color
                    </button>
                </div>
            </div>
        </div>
    `;
    } catch (e) {
        app.innerHTML = '<p>Error loading list.</p>';
    }

    await setupPopupForImages();
    await setupImageUpload();
    await setupDragAndDrop();
    await setupListEditing();
}
