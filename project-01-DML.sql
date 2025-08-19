-- Author: Your Name
-- Date: August 19, 2025
-- Patient Visit Manager Sample Data

USE PatientVisitManager;
GO

-- Sample Users (passwords should be hashed in real implementation)
INSERT INTO Users (Email, PasswordHash, PasswordSalt, Role, FirstName, LastName) VALUES
('admin@hospital.com', 'hashed_password_1', 'salt_1', 'Admin', 'System', 'Administrator'),
('dr.smith@hospital.com', 'hashed_password_2', 'salt_2', 'Doctor', 'John', 'Smith'),
('dr.johnson@hospital.com', 'hashed_password_3', 'salt_3', 'Doctor', 'Sarah', 'Johnson'),
('receptionist@hospital.com', 'hashed_password_4', 'salt_4', 'Receptionist', 'Mary', 'Williams');

-- Sample Doctors
INSERT INTO Doctors (UserID, FirstName, LastName, Specialization, LicenseNumber, Email, Phone) VALUES
(2, 'John', 'Smith', 'Cardiology', 'MD123456', 'dr.smith@hospital.com', '555-0101'),
(3, 'Sarah', 'Johnson', 'Neurology', 'MD789012', 'dr.johnson@hospital.com', '555-0102'),
(NULL, 'Michael', 'Brown', 'Orthopedics', 'MD345678', 'dr.brown@hospital.com', '555-0103'),
(NULL, 'Emily', 'Davis', 'Pediatrics', 'MD901234', 'dr.davis@hospital.com', '555-0104');

-- Sample Patients
INSERT INTO Patients (FirstName, LastName, DateOfBirth, Gender, Email, Phone, Address, EmergencyContact, EmergencyPhone) VALUES
('Alice', 'Johnson', '1985-03-15', 'Female', 'alice.johnson@email.com', '555-1001', '123 Main St, City, State 12345', 'Bob Johnson', '555-1002'),
('David', 'Wilson', '1978-07-22', 'Male', 'david.wilson@email.com', '555-1003', '456 Oak Ave, City, State 12345', 'Susan Wilson', '555-1004'),
('Emma', 'Thompson', '1992-11-08', 'Female', 'emma.thompson@email.com', '555-1005', '789 Pine Rd, City, State 12345', 'James Thompson', '555-1006'),
('Robert', 'Garcia', '1965-05-30', 'Male', 'robert.garcia@email.com', '555-1007', '321 Elm St, City, State 12345', 'Maria Garcia', '555-1008'),
('Lisa', 'Martinez', '1988-12-12', 'Female', 'lisa.martinez@email.com', '555-1009', '654 Cedar Ln, City, State 12345', 'Carlos Martinez', '555-1010');

-- Sample Fee Schedule
INSERT INTO FeeSchedule (ServiceName, ServiceCode, Amount, Description) VALUES
('General Consultation', 'GC001', 150.00, 'General medical consultation'),
('Cardiology Consultation', 'CC001', 250.00, 'Specialized cardiology consultation'),
('Neurology Consultation', 'NC001', 275.00, 'Specialized neurology consultation'),
('Blood Test', 'BT001', 75.00, 'Complete blood count test'),
('X-Ray', 'XR001', 125.00, 'Standard X-ray examination'),
('ECG', 'ECG001', 100.00, 'Electrocardiogram'),
('MRI Scan', 'MRI001', 800.00, 'Magnetic resonance imaging'),
('Physical Therapy', 'PT001', 85.00, 'Physical therapy session'),
('Follow-up Visit', 'FU001', 100.00, 'Follow-up consultation'),
('Emergency Consultation', 'EC001', 300.00, 'Emergency medical consultation');

-- Sample Visits
INSERT INTO Visits (PatientID, DoctorID, VisitDate, VisitType, Status, Reason, Notes, Duration, TotalAmount) VALUES
(1, 1, '2025-08-15 09:00:00', 'Regular', 'Completed', 'Annual checkup', 'Patient in good health. Recommended annual follow-up.', 30, 150.00),
(2, 2, '2025-08-16 10:30:00', 'Regular', 'Completed', 'Headache complaints', 'Neurological examination normal. Prescribed medication.', 45, 275.00),
(3, 1, '2025-08-17 14:00:00', 'Regular', 'Scheduled', 'Chest pain', 'Scheduled for cardiac evaluation', 60, 250.00),
(4, 3, '2025-08-18 11:00:00', 'Regular', 'InProgress', 'Knee pain', 'Orthopedic consultation in progress', 30, 150.00),
(5, 2, '2025-08-19 15:30:00', 'Regular', 'Scheduled', 'Migraine follow-up', 'Follow-up appointment for migraine treatment', 30, 100.00);

-- Sample Visit Services
INSERT INTO VisitServices (VisitID, FeeScheduleID, Quantity, Amount) VALUES
(1, 1, 1, 150.00), -- General Consultation
(2, 3, 1, 275.00), -- Neurology Consultation
(3, 2, 1, 250.00), -- Cardiology Consultation
(4, 1, 1, 150.00), -- General Consultation
(5, 9, 1, 100.00); -- Follow-up Visit

-- Sample Activity Logs
INSERT INTO ActivityLogs (UserID, EntityType, EntityID, Action, Details, IPAddress) VALUES
(1, 'Patient', 1, 'Created', 'New patient Alice Johnson registered', '192.168.1.100'),
(2, 'Visit', 1, 'Created', 'Visit scheduled for patient Alice Johnson', '192.168.1.101'),
(2, 'Visit', 1, 'Updated', 'Visit completed for patient Alice Johnson', '192.168.1.101'),
(3, 'Visit', 2, 'Created', 'Visit scheduled for patient David Wilson', '192.168.1.102'),
(1, 'Doctor', 3, 'Created', 'New doctor Michael Brown added to system', '192.168.1.100');

GO
