
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
    document.getElementById('createPatientForm').addEventListener('submit', createPatient);
    document.getElementById('editPatientForm').addEventListener('submit', updatePatient);
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
async function loadPatients(searchTerm = '') {
    try {
        let url = `${API_BASE_URL}/patients`;
        if (searchTerm) {
            url += `?searchTerm=${encodeURIComponent(searchTerm)}`;
        }
        
        const response = await fetch(url, {
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
    if (searchTerm.trim()) {
        loadPatients(searchTerm);
    } else {
        loadPatients();
    }
}

function showCreatePatient() {
    if (!isAuthenticated()) {
        showAlert('Please login to access this section', 'warning');
        return;
    }
    
    // Check if user has permission (Admin or Receptionist)
    if (currentUser?.role !== 'Admin' && currentUser?.role !== 'Receptionist') {
        showAlert('You do not have permission to create patients', 'danger');
        return;
    }
    
    hideAllSections();
    document.getElementById('createPatientSection').classList.remove('d-none');
}

async function createPatient(event) {
    event.preventDefault();
    
    const formData = {
        firstName: document.getElementById('createFirstName').value,
        lastName: document.getElementById('createLastName').value,
        dateOfBirth: document.getElementById('createDateOfBirth').value,
        gender: document.getElementById('createGender').value,
        email: document.getElementById('createEmail').value,
        phone: document.getElementById('createPhone').value,
        address: document.getElementById('createAddress').value,
        emergencyContact: document.getElementById('createEmergencyContact').value,
        emergencyPhone: document.getElementById('createEmergencyPhone').value
    };

    try {
        const response = await fetch(`${API_BASE_URL}/patients`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${authToken}`
            },
            body: JSON.stringify(formData)
        });

        const result = await response.json();

        if (result.success) {
            showAlert('Patient created successfully!', 'success');
            document.getElementById('createPatientForm').reset();
            showPatients();
        } else {
            showAlert(result.message || 'Failed to create patient', 'danger');
        }
    } catch (error) {
        console.error('Create patient error:', error);
        showAlert('An error occurred while creating the patient', 'danger');
    }
}

function viewPatient(patientId) {
    if (!isAuthenticated()) {
        showAlert('Please login to access this section', 'warning');
        return;
    }
    
    loadPatientDetails(patientId);
}

async function loadPatientDetails(patientId) {
    try {
        const response = await fetch(`${API_BASE_URL}/patients/${patientId}`, {
            headers: {
                'Authorization': `Bearer ${authToken}`
            }
        });

        const result = await response.json();

        if (result.success) {
            displayPatientDetails(result.data);
        } else {
            showAlert(result.message || 'Failed to load patient details', 'danger');
        }
    } catch (error) {
        console.error('Load patient details error:', error);
        showAlert('An error occurred while loading patient details', 'danger');
    }
}

function displayPatientDetails(patient) {
    hideAllSections();
    document.getElementById('patientDetailsSection').classList.remove('d-none');
    
    document.getElementById('detailPatientName').textContent = patient.fullName;
    document.getElementById('detailPatientAge').textContent = patient.age;
    document.getElementById('detailPatientGender').textContent = patient.gender;
    document.getElementById('detailPatientEmail').textContent = patient.email || 'N/A';
    document.getElementById('detailPatientPhone').textContent = patient.phone || 'N/A';
    document.getElementById('detailPatientAddress').textContent = patient.address || 'N/A';
    document.getElementById('detailPatientEmergencyContact').textContent = patient.emergencyContact || 'N/A';
    document.getElementById('detailPatientEmergencyPhone').textContent = patient.emergencyPhone || 'N/A';
    
    // Store patient ID for edit functionality
    document.getElementById('patientDetailsSection').dataset.patientId = patient.patientID;
}

function editPatient(patientId) {
    if (!isAuthenticated()) {
        showAlert('Please login to access this section', 'warning');
        return;
    }
    
    // Check if user has permission (Admin or Receptionist)
    if (currentUser?.role !== 'Admin' && currentUser?.role !== 'Receptionist') {
        showAlert('You do not have permission to edit patients', 'danger');
        return;
    }
    
    loadPatientForEdit(patientId);
}

async function loadPatientForEdit(patientId) {
    try {
        const response = await fetch(`${API_BASE_URL}/patients/${patientId}`, {
            headers: {
                'Authorization': `Bearer ${authToken}`
            }
        });

        const result = await response.json();

        if (result.success) {
            populateEditForm(result.data);
        } else {
            showAlert(result.message || 'Failed to load patient for editing', 'danger');
        }
    } catch (error) {
        console.error('Load patient for edit error:', error);
        showAlert('An error occurred while loading patient for editing', 'danger');
    }
}

function populateEditForm(patient) {
    hideAllSections();
    document.getElementById('editPatientSection').classList.remove('d-none');
    
    document.getElementById('editPatientId').value = patient.patientID;
    document.getElementById('editFirstName').value = patient.firstName;
    document.getElementById('editLastName').value = patient.lastName;
    document.getElementById('editDateOfBirth').value = patient.dateOfBirth.split('T')[0];
    document.getElementById('editGender').value = patient.gender;
    document.getElementById('editEmail').value = patient.email || '';
    document.getElementById('editPhone').value = patient.phone || '';
    document.getElementById('editAddress').value = patient.address || '';
    document.getElementById('editEmergencyContact').value = patient.emergencyContact || '';
    document.getElementById('editEmergencyPhone').value = patient.emergencyPhone || '';
}

async function updatePatient(event) {
    event.preventDefault();
    
    const patientId = document.getElementById('editPatientId').value;
    const formData = {
        firstName: document.getElementById('editFirstName').value,
        lastName: document.getElementById('editLastName').value,
        dateOfBirth: document.getElementById('editDateOfBirth').value,
        gender: document.getElementById('editGender').value,
        email: document.getElementById('editEmail').value,
        phone: document.getElementById('editPhone').value,
        address: document.getElementById('editAddress').value,
        emergencyContact: document.getElementById('editEmergencyContact').value,
        emergencyPhone: document.getElementById('editEmergencyPhone').value
    };

    try {
        const response = await fetch(`${API_BASE_URL}/patients/${patientId}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${authToken}`
            },
            body: JSON.stringify(formData)
        });

        const result = await response.json();

        if (result.success) {
            showAlert('Patient updated successfully!', 'success');
            showPatients();
        } else {
            showAlert(result.message || 'Failed to update patient', 'danger');
        }
    } catch (error) {
        console.error('Update patient error:', error);
        showAlert('An error occurred while updating the patient', 'danger');
    }
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
