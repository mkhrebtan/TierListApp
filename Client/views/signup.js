import { authManager } from "../dataManager.js";

export async function renderSignup() {
    const app = document.getElementById('app');
    app.innerHTML = `
        <div class="login-container">
            <div class="login-card">
                <h1>Sign Up</h1>
                <form id="registerForm" class="auth-form">
                    <div class="form-group">
                        <label for="usernameRegister">Username</label>
                        <input type="text" placeholder="Username" id="usernameRegister" required>
                    </div>
                    
                    <div class="form-group">
                        <label for="passwordRegister">Password</label>
                        <input type="password" placeholder="Password" id="passwordRegister" required>
                    </div>
                    
                    <button type="submit" id="registerBtn" class="login-btn">Sign Up</button>
                </form>
                <p id="errorMessage" class="error-message hidden"></p>
                <div id="loadingMessage" class="loading-message hidden">Signing in...</div>
            </div>
            <div class="login-footer">
                <span>Already have an account? <a href="#/login" class="signup-link">Log in</a></span>
            </div>
        </div>
    `;

    await setupUserAuth();
}

async function setupUserAuth() {
    const registerForm = document.getElementById('registerForm');
    const errorMessage = document.getElementById('errorMessage');
    const loadingMessage = document.getElementById('loadingMessage');
    const registerBtn = document.getElementById('registerBtn');
    
    registerForm.addEventListener('submit', async (e) => {
        e.preventDefault();

        const username = document.getElementById('usernameRegister').value.trim();
        const password = document.getElementById('passwordRegister').value.trim();

        hideError();

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

        showLoading();
        
        try {
            const result = await authManager.register(username, password);
            
            if (result.success) {
                location.hash = '#/login';
            } else {
                showError(result.message || 'Sign Up failed. Please try again.');
            }
        } catch (error) {
            console.error('Error during sign up:', error);
            showError('An error occurred while signing up. Please try again.');
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
        registerBtn.disabled = true;
        registerBtn.textContent = 'Signing up...';
    }

    function hideLoading() {
        loadingMessage.classList.add('hidden');
        registerBtn.disabled = false;
        registerBtn.textContent = 'Sign Up';
    }
}