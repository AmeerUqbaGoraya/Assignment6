// Author: Your Name
// Date: August 19, 2025

// Global variables
let currentUser = null;
let authToken = null;
const API_BASE_URL = '/api';

// Initialize the application
document.addEventListener('DOMContentLoaded', function() {
    checkAuthStatus();
    setupEventListeners();
    showHome();
});

// Check if user is authenticated
function checkAuthStatus() {
    authToken = localStorage.getItem('authToken');
    const userInfo = localStorage.getItem('userInfo');
    
    if (authToken && userInfo) {
        currentUser = JSON.parse(userInfo);
        updateAuthUI(true);
    } else {
        updateAuthUI(false);
    }
}

// Update authentication UI
function updateAuthUI(isAuthenticated) {
    const loginNav = document.getElementById('loginNav');
    const registerNav = document.getElementById('registerNav');
    const userNav = document.getElementById('userNav');
    const userName = document.getElementById('userName');
    
    if (isAuthenticated) {
        loginNav.classList.add('d-none');
        registerNav.classList.add('d-none');
        userNav.classList.remove('d-none');
        userName.textContent = currentUser?.fullName || 'User';
    } else {
        loginNav.classList.remove('d-none');
        registerNav.classList.remove('d-none');
        userNav.classList.add('d-none');
    }
}

// Setup event listeners
function setupEventListeners() {
    document.getElementById('loginForm').addEventListener('submit', handleLogin);
    document.getElementById('registerForm').addEventListener('submit', handleRegister);
}

// Navigation functions
function showHome() {
    hideAllSections();
    document.getElementById('homeSection').classList.remove('d-none');
}

function showLogin() {
    hideAllSections();
    document.getElementById('loginSection').classList.remove('d-none');
}

function showRegister() {
    hideAllSections();
    document.getElementById('registerSection').classList.remove('d-none');
}

function showPatients() {
    if (!isAuthenticated()) {
        showAlert('Please login to access this section', 'warning');
        showLogin();
        return;
    }
    hideAllSections();
    document.getElementById('patientsSection').classList.remove('d-none');
    loadPatients();
}

function showDoctors() {
    if (!isAuthenticated()) {
        showAlert('Please login to access this section', 'warning');
        showLogin();
        return;
    }
    // TODO: Implement doctors section
    showAlert('Doctors section coming soon!', 'info');
}

function showVisits() {
    if (!isAuthenticated()) {
        showAlert('Please login to access this section', 'warning');
        showLogin();
        return;
    }
    // TODO: Implement visits section
    showAlert('Visits section coming soon!', 'info');
}

function showFeeSchedule() {
    if (!isAuthenticated()) {
        showAlert('Please login to access this section', 'warning');
        showLogin();
        return;
    }
    // TODO: Implement fee schedule section
    showAlert('Fee Schedule section coming soon!', 'info');
}

function showActivityLog() {
    if (!isAuthenticated()) {
        showAlert('Please login to access this section', 'warning');
        showLogin();
        return;
    }
    // TODO: Implement activity log section
    showAlert('Activity Log section coming soon!', 'info');
}

function showChangePassword() {
    // TODO: Implement change password modal
    showAlert('Change password functionality coming soon!', 'info');
}

function hideAllSections() {
    const sections = ['homeSection', 'loginSection', 'registerSection', 'patientsSection'];
    sections.forEach(section => {
        document.getElementById(section).classList.add('d-none');
    });
}

// Authentication functions
function isAuthenticated() {
    return authToken !== null && currentUser !== null;
}

async function handleLogin(event) {
    event.preventDefault();
    
    const email = document.getElementById('loginEmail').value;
    const password = document.getElementById('loginPassword').value;
    
    try {
        const response = await fetch(`${API_BASE_URL}/auth/login`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ email, password })
        });
        
        const result = await response.json();
        
        if (result.success) {
            authToken = result.data.token;
            currentUser = {
                email: result.data.email,
                fullName: result.data.fullName,
                role: result.data.role
            };
            
            localStorage.setItem('authToken', authToken);
            localStorage.setItem('userInfo', JSON.stringify(currentUser));
            
            updateAuthUI(true);
            showAlert('Login successful!', 'success');
            showHome();
            
            // Clear form
            document.getElementById('loginForm').reset();
        } else {
            showAlert(result.message || 'Login failed', 'danger');
        }
    } catch (error) {
        console.error('Login error:', error);
        showAlert('An error occurred during login', 'danger');
    }
}

async function handleRegister(event) {
    event.preventDefault();
    
    const email = document.getElementById('registerEmail').value;
    const password = document.getElementById('registerPassword').value;
    const firstName = document.getElementById('registerFirstName').value;
    const lastName = document.getElementById('registerLastName').value;
    const role = document.getElementById('registerRole').value;
    
    try {
        const response = await fetch(`${API_BASE_URL}/auth/register`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ email, password, firstName, lastName, role })
        });
        
        const result = await response.json();
        
        if (result.success) {
            showAlert('Registration successful! Please login.', 'success');
            showLogin();
            
            // Clear form
            document.getElementById('registerForm').reset();
        } else {
            showAlert(result.message || 'Registration failed', 'danger');
        }
    } catch (error) {
        console.error('Registration error:', error);
        showAlert('An error occurred during registration', 'danger');
    }
}

function logout() {
    authToken = null;
    currentUser = null;
    localStorage.removeItem('authToken');
    localStorage.removeItem('userInfo');
    
    updateAuthUI(false);
    showAlert('Logged out successfully', 'info');
    showHome();
}

// Patient functions
async function loadPatients() {
    try {
        const response = await fetch(`${API_BASE_URL}/patients`, {
            headers: {
                'Authorization': `Bearer ${authToken}`
            }
        });
        
        const result = await response.json();
        
        if (result.success) {
            renderPatientsTable(result.data.items);
        } else {
            showAlert(result.message || 'Failed to load patients', 'danger');
        }
    } catch (error) {
        console.error('Load patients error:', error);
        showAlert('An error occurred while loading patients', 'danger');
    }
}

function renderPatientsTable(patients) {
    const tableContainer = document.getElementById('patientsTable');
    
    if (patients.length === 0) {
        tableContainer.innerHTML = '<div class="alert alert-info">No patients found.</div>';
        return;
    }
    
    let tableHTML = `
        <div class="table-responsive">
            <table class="table table-striped table-hover">
                <thead>
                    <tr>
                        <th>Name</th>
                        <th>Age</th>
                        <th>Gender</th>
                        <th>Email</th>
                        <th>Phone</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
    `;
    
    patients.forEach(patient => {
        tableHTML += `
            <tr>
                <td>${patient.fullName}</td>
                <td>${patient.age}</td>
                <td>${patient.gender}</td>
                <td>${patient.email || 'N/A'}</td>
                <td>${patient.phone || 'N/A'}</td>
                <td>
                    <button class="btn btn-sm btn-outline-primary" onclick="viewPatient(${patient.patientID})">View</button>
                    <button class="btn btn-sm btn-outline-secondary" onclick="editPatient(${patient.patientID})">Edit</button>
                </td>
            </tr>
        `;
    });
    
    tableHTML += `
                </tbody>
            </table>
        </div>
    `;
    
    tableContainer.innerHTML = tableHTML;
}

function searchPatients() {
    const searchTerm = document.getElementById('patientSearch').value;
    // Implement search functionality
    console.log('Searching for:', searchTerm);
}

function showCreatePatient() {
    showAlert('Create patient functionality coming soon!', 'info');
}

function viewPatient(patientId) {
    showAlert(`View patient ${patientId} functionality coming soon!`, 'info');
}

function editPatient(patientId) {
    showAlert(`Edit patient ${patientId} functionality coming soon!`, 'info');
}

// Utility functions
function showAlert(message, type = 'info') {
    const alertContainer = document.getElementById('alertContainer');
    const alertHTML = `
        <div class="alert alert-${type} alert-dismissible fade show" role="alert">
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `;
    
    alertContainer.innerHTML = alertHTML;
    
    // Auto-dismiss after 5 seconds
    setTimeout(() => {
        const alert = alertContainer.querySelector('.alert');
        if (alert) {
            const bsAlert = new bootstrap.Alert(alert);
            bsAlert.close();
        }
    }, 5000);
}

function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric'
    });
}

function formatDateTime(dateString) {
    const date = new Date(dateString);
    return date.toLocaleString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
    });
}
