import { authManager } from "../dataManager.js";

export async function renderLogin() {
    // Check if user is already authenticated
    if (await authManager.isAuthenticated()) {
        location.hash = '#/';
        return;
    }
    
    const app = document.getElementById('app');
    app.innerHTML = `
        <div class="login-container">
            <div class="login-card">
                <h1>Log In</h1>
                <form id="loginForm" class="auth-form">
                    <div class="form-group">
                        <label for="usernameLogin">Username</label>
                        <input type="text" placeholder="Username" id="usernameLogin" required>
                    </div>
                    
                    <div class="form-group">
                        <label for="passwordLogin">Password</label>
                        <input type="password" placeholder="Password" id="passwordLogin" required>
                    </div>
                    
                    <button type="submit" id="loginBtn" class="login-btn">Log In</button>
                </form>
                <p id="errorMessage" class="error-message hidden"></p>
                <div id="loadingMessage" class="loading-message hidden">Logging in...</div>
            </div>
            <div class="login-footer">
                <span>Don't have an account? <a href="#/signup" class="signup-link">Sign up</a></span>
            </div>
        </div>
    `;

    await setupUserAuth();
}

async function setupUserAuth() {
    const loginForm = document.getElementById('loginForm');
    const errorMessage = document.getElementById('errorMessage');
    const loadingMessage = document.getElementById('loadingMessage');
    const loginBtn = document.getElementById('loginBtn');
    
    loginForm.addEventListener('submit', async (e) => {
        e.preventDefault(); // Prevent default form submission

        const username = document.getElementById('usernameLogin').value.trim();
        const password = document.getElementById('passwordLogin').value.trim();

        // Clear previous error messages
        hideError();

        // Basic validation
        if (username === '' || password === '') {
            showError('Username and password cannot be empty');
            return;
        }
        
        if (password.length < 6 || password.length > 50) {
            showError('Password must be between 6 and 50 characters long');
            return;
        }
        
        if (username.length < 3 || username.length > 50) {
            showError('Username must be between 3 and 50 characters long');
            return;
        }

        // Show loading state
        showLoading();
        
        try {
            const result = await authManager.login(username, password);
            
            if (result.success === true) {
                // Login successful, redirect to home
                location.hash = '#/';
            } else {
                showError(result.message || 'Login failed. Please try again.');
            }
        } catch (error) {
            console.error('Error during login:', error);
            showError('An error occurred while logging in. Please try again.');
        } finally {
            hideLoading();
        }
    });

    function showError(message) {
        errorMessage.textContent = message;
        errorMessage.classList.remove('hidden');
    }

    function hideError() {
        errorMessage.classList.add('hidden');
    }

    function showLoading() {
        loadingMessage.classList.remove('hidden');
        loginBtn.disabled = true;
        loginBtn.textContent = 'Logging in...';
    }

    function hideLoading() {
        loadingMessage.classList.add('hidden');
        loginBtn.disabled = false;
        loginBtn.textContent = 'Log In';
    }
}