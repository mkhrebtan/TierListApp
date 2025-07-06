import { tierListLibraryDataManager } from "../dataManager.js";

export async function renderLibrary() {
    const token = tierListLibraryDataManager.getToken();
    const app = document.getElementById('app');
    
    // Force a fresh fetch of lists to avoid caching issues
    try {
        // Clear any cached data to ensure fresh fetch
        tierListLibraryDataManager.clearCache();
        
        // const lists = await tierListLibraryDataManager.getAllLists();
        // if (!lists) {
        //     app.innerHTML = `<h1>Error loading lists</h1>`;
        //     return;
        // }
        
        // ${lists.map(list => `
        //             <div class="list-item" id="list${list.id}" onclick="location.hash = '#/list/${list.id}'">
        //                 <h3>${list.title}</h3>
        //             </div>
        //         `).join('')}

        app.innerHTML = `
        <div class="library-main">
            <h1 class="app-header">Tier List App</h1>
            <div class="library-header">
                <button class="auth-btn" id="loginBtn">
                    Log In
                </button>
                <button class="auth-btn" id="signupBtn">
                    Sign Up
                </button>
            </div>
            <div class="lists-container">
                
            </div>
            <div class="library-footer">
                <button class="add-list-btn" id="addListBtn">
                    Add New List
                </button>
            </div>
        </div>
        <div class="blur-overlay" id="blurOverlay"></div>
        <div class="popup listPop" id="addListPopup">
            <div class="popup-header" id="popupHeader">
                <h3>Add New List</h3>
                <button class="close-popup-btn" id="closeAddListPopup">
                    <i class="fa-solid fa-xmark fa-2xl"></i>
                </button>
            </div>
            <div class="popup-add-list-content">
                <input class="popup-input" type="text" id="listTitleInput" placeholder="Enter new list title..."/>
                <button class="popup-input-save hidden" id="listCreateBtn">
                    <i class="fa-solid fa-check fa-xl"></i>
                </button>
            </div>
        </div>
        <div class="popup listPop" id="renameListPopup">
            <div class="popup-header" id="renamePopupHeader">
                <h3>Rename List</h3>
                <button class="close-popup-btn" id="closeRenameListPopup">
                    <i class="fa-solid fa-xmark fa-2xl"></i>
                </button>
            </div>
            <div class="popup-add-list-content">
                <input class="popup-input" type="text" id="listTitleInputRename" placeholder="Enter new list title..."/>
                <button class="popup-input-save hidden" id="listTitleSaveBtn">
                    <i class="fa-solid fa-check fa-xl"></i>
                </button>
            </div>
        </div>
        <div class="popup authPop" id="loginPopup">
            <div class="popup-header" id="loginHeader">
                <h3>Log In</h3>
                <button class="close-popup-btn" id="closeLoginPopup">
                    <i class="fa-solid fa-xmark fa-2xl"></i>
                </button>
            </div>
            
        </div>
        `;

        await setupListAdding();
        await setupListRenaming();
        await setupContextMenus();
    } catch (error) {
        console.error('Error rendering home:', error);
        app.innerHTML = `<h1>Error loading lists</h1>`;
    }
} 

async function setupListAdding() {
    const addListBtn = document.getElementById('addListBtn');
    addListBtn.addEventListener('click', openAddListPopup);
    const closeAddListPopupBtn = document.getElementById('closeAddListPopup');
    closeAddListPopupBtn.addEventListener('click', closeAddListPopup);
    const listTitleInput = document.getElementById('listTitleInput');
    const listCreateBtn = document.getElementById('listCreateBtn');
    listTitleInput.addEventListener('input', () => {
        if (listTitleInput.value.trim() !== '') {
            showSaveButton(listCreateBtn, listTitleInput);
        } else {
            hideSaveButton(listCreateBtn, listTitleInput);
        }
    });

    listCreateBtn.addEventListener('click', async () => {
        const title = listTitleInput.value.trim();
        if (title === '') {
            alert('List title cannot be empty');
            return;
        }
        try {
            const newList = await tierListLibraryDataManager.addList(title);
            if (newList.success === true) {
                await createListItem(newList.list);
                closeAddListPopup();
            } else {
                alert('Failed to create new list');
            }
        } catch (error) {
            console.error('Error creating list:', error);
            alert('An error occurred while creating the list');
        }
    });
}

async function setupListRenaming() {
    const closeRenameListPopupBtn = document.getElementById('closeRenameListPopup');
    closeRenameListPopupBtn.addEventListener('click', closeRenameListPopup);
    const listTitleInput = document.getElementById('listTitleInputRename');
    const listTitleSaveBtn = document.getElementById('listTitleSaveBtn');
    listTitleInput.addEventListener('input', () => {
        if (listTitleInput.value.trim() !== '' && listTitleInput.value.trim() !== currentListTitle) {
            showSaveButton(listTitleSaveBtn, listTitleInput);
        } else {
            hideSaveButton(listTitleSaveBtn, listTitleInput);
        }
    });

    listTitleSaveBtn.addEventListener('click', async () => {
        const newTitle = listTitleInput.value.trim();
        if (newTitle === '') {
            alert('List title cannot be empty');
            return;
        }
        try {
            const result = await tierListLibraryDataManager.updateListTitle(currentListId, newTitle);
            if (result.success) {
                currentListItem.querySelector('h3').textContent = newTitle;
                closeRenameListPopup();
            } else {
                alert('Failed to rename the list');
            }
        } catch (error) {
            console.error('Error renaming list:', error);
            alert('An error occurred while renaming the list');
        }
    });
}

async function setupContextMenus() {
    const listItems = document.querySelectorAll('.list-item');
    listItems.forEach(list => {
        addContextMenuToListItem(list);
    });
}

let currentListId; // Global variable to hold the current list item id for renaming
let currentListTitle; // Global variable to hold the current list title for renaming
let currentListItem; // Global variable to hold the current list item for context menu actions

async function addContextMenuToListItem(listItem) {
    const menuItems = [
        {
            content: `Rename`,
            events: {
                click: () => {
                    const listTitle = listItem.querySelector('h3').textContent.trim();
                    currentListId = listItem.id.replace('list', '');
                    currentListTitle = listTitle;
                    currentListItem = listItem; // Store the current list item for later use
                    const listTitleInput = document.getElementById('listTitleInputRename');
                    listTitleInput.value = listTitle;
                    openRenameListPopup();
                }
            }
        },
        {
            content: `Delete`,
            events: {
                click: async (e) => {
                    if (confirm('Are you sure you want to delete this list?')) {
                        const listId = listItem.id.replace('list', '');
                        try {
                            const result = await tierListLibraryDataManager.deleteList(listId);
                            if (result.success) {
                                // Remove from DOM immediately
                                listItem.remove();
                                
                                // If no lists remain, show empty state
                                const remainingLists = document.querySelectorAll('.list-item');
                                if (remainingLists.length === 0) {
                                    const listsContainer = document.querySelector('.lists-container');
                                    listsContainer.innerHTML = '<p style="text-align: center; color: #888;">No lists created yet</p>';
                                }
                            } else {
                                alert('Failed to delete the list');
                            }
                        } catch (error) {
                            console.error('Error deleting list:', error);
                            alert('An error occurred while deleting the list');
                        }
                    }
                }
            }
        },
    ];

    const contextMenu = new ContextMenu({
        target: `#${listItem.id}`,
        menuItems
    });

    contextMenu.init();
}

async function createListItem(newList) {
    const newListItem = document.createElement('div');
    newListItem.className = 'list-item';
    newListItem.id = `list${newList.id}`;
    newListItem.innerHTML = `<h3>${newList.title}</h3>`;

    newListItem.addEventListener('click', () => {
        location.hash = `#/list/${newList.id}`;
    });

    app.querySelector('.lists-container').appendChild(newListItem);
    addContextMenuToListItem(newListItem);
}

function openAddListPopup() {
    const popup = document.getElementById("addListPopup");
    const overlay = document.getElementById("blurOverlay");

    popup.classList.add("active");
    overlay.classList.add("active");
    document.body.classList.add("body-noscroll");
}

function closeAddListPopup() {
    const popup = document.getElementById("addListPopup");
    const overlay = document.getElementById("blurOverlay");

    const listTitleInput = document.getElementById('listTitleInput');
    listTitleInput.value = '';

    popup.classList.remove("active");
    overlay.classList.remove("active");
    document.body.classList.remove("body-noscroll");
}

function openRenameListPopup() {
    const popup = document.getElementById("renameListPopup");
    const overlay = document.getElementById("blurOverlay");

    popup.classList.add("active");
    overlay.classList.add("active");
    document.body.classList.add("body-noscroll");
}

function closeRenameListPopup() {
    const popup = document.getElementById("renameListPopup");
    const overlay = document.getElementById("blurOverlay");

    popup.classList.remove("active");
    overlay.classList.remove("active");
    document.body.classList.remove("body-noscroll");
}

function openLoginPopup() {
    const popup = document.getElementById("loginPopup");
    const overlay = document.getElementById("blurOverlay");

    popup.classList.add("active");
    overlay.classList.add("active");
    document.body.classList.add("body-noscroll");

    // Clear input fields
    // document.getElementById('usernameInputLogin').value = '';
    // document.getElementById('passwordInputLogin').value = '';
}

function closeLoginPopup() {
    const popup = document.getElementById("loginPopup");
    const overlay = document.getElementById("blurOverlay");

    popup.classList.remove("active");
    overlay.classList.remove("active");
    document.body.classList.remove("body-noscroll");
}

function showSaveButton(btn, input) {
    btn.classList.remove('hidden');
    input.style.flex = "0.98";
}

function hideSaveButton(btn, input) {
    btn.classList.add('hidden');
    input.style.flex = "1";
}