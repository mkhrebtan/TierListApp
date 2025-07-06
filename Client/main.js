import { renderHome } from './views/home.js';
import { renderList } from './views/list.js';
import { renderLogin } from './views/login.js';
import { renderSignup } from './views/signup.js';

const routes = {
    '/': renderHome,
    '/list/:id': renderList,
    '/login': renderLogin,
    '/signup': renderSignup,
};

function matchRoute(path) {
    if (path === '/') return () => renderHome();
    const match = path.match(/^\/list\/(\d+)/);
    if (match) return () => renderList(match[1]);
    if (path === '/login') return () => renderLogin();
    if (path === '/signup') return () => renderSignup();
    return () => document.getElementById('app').innerHTML = '<h1>404</h1>';
}

function router() {
    const path = location.hash.slice(1) || '/';
    const handler = matchRoute(path);
    handler();
}

window.addEventListener('hashchange', router);
window.addEventListener('load', router);
