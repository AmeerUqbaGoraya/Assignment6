-- Author: Your Name
-- Date: August 19, 2025
-- Patient Visit Manager Stored Procedures

USE PatientVisitManager;
GO

-- Stored Procedure: Get Patient by ID with Visit History
CREATE PROCEDURE sp_GetPatientWithVisits
    @PatientID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Patient Information
    SELECT 
        p.PatientID, p.FirstName, p.LastName, p.DateOfBirth, p.Gender,
        p.Email, p.Phone, p.Address, p.EmergencyContact, p.EmergencyPhone,
        p.IsActive, p.CreatedDate, p.ModifiedDate
    FROM Patients p
    WHERE p.PatientID = @PatientID AND p.IsActive = 1;
    
    -- Patient's Visit History
    SELECT 
        v.VisitID, v.VisitDate, v.VisitType, v.Status, v.Reason, v.Notes,
        v.Duration, v.TotalAmount,
        d.FirstName + ' ' + d.LastName AS DoctorName,
        d.Specialization
    FROM Visits v
    INNER JOIN Doctors d ON v.DoctorID = d.DoctorID
    WHERE v.PatientID = @PatientID
    ORDER BY v.VisitDate DESC;
END
GO

-- Stored Procedure: Get Doctor Schedule
CREATE PROCEDURE sp_GetDoctorSchedule
    @DoctorID INT,
    @StartDate DATE,
    @EndDate DATE
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        v.VisitID, v.VisitDate, v.VisitType, v.Status, v.Reason,
        v.Duration, v.TotalAmount,
        p.FirstName + ' ' + p.LastName AS PatientName,
        p.Phone AS PatientPhone
    FROM Visits v
    INNER JOIN Patients p ON v.PatientID = p.PatientID
    WHERE v.DoctorID = @DoctorID 
        AND CAST(v.VisitDate AS DATE) BETWEEN @StartDate AND @EndDate
        AND v.Status != 'Cancelled'
    ORDER BY v.VisitDate ASC;
END
GO

-- Stored Procedure: Schedule New Visit
CREATE PROCEDURE sp_ScheduleVisit
    @PatientID INT,
    @DoctorID INT,
    @VisitDate DATETIME2,
    @VisitType NVARCHAR(50) = 'Regular',
    @Reason NVARCHAR(500),
    @Duration INT = 30,
    @UserID INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    
    BEGIN TRY
        -- Check if doctor is available at that time
        IF EXISTS (
            SELECT 1 FROM Visits 
            WHERE DoctorID = @DoctorID 
                AND ABS(DATEDIFF(MINUTE, VisitDate, @VisitDate)) < @Duration
                AND Status != 'Cancelled'
        )
        BEGIN
            RAISERROR('Doctor is not available at the selected time', 16, 1);
            RETURN;
        END
        
        -- Insert new visit
        DECLARE @VisitID INT;
        INSERT INTO Visits (PatientID, DoctorID, VisitDate, VisitType, Reason, Duration)
        VALUES (@PatientID, @DoctorID, @VisitDate, @VisitType, @Reason, @Duration);
        
        SET @VisitID = SCOPE_IDENTITY();
        
        -- Log activity
        INSERT INTO ActivityLogs (UserID, EntityType, EntityID, Action, Details)
        VALUES (@UserID, 'Visit', @VisitID, 'Created', 'New visit scheduled');
        
        SELECT @VisitID AS VisitID;
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- Stored Procedure: Update Visit Status
CREATE PROCEDURE sp_UpdateVisitStatus
    @VisitID INT,
    @Status NVARCHAR(20),
    @Notes NVARCHAR(1000) = NULL,
    @UserID INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    
    BEGIN TRY
        UPDATE Visits 
        SET Status = @Status, 
            Notes = COALESCE(@Notes, Notes),
            ModifiedDate = GETDATE()
        WHERE VisitID = @VisitID;
        
        -- Log activity
        INSERT INTO ActivityLogs (UserID, EntityType, EntityID, Action, Details)
        VALUES (@UserID, 'Visit', @VisitID, 'Updated', 'Visit status updated to ' + @Status);
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- Stored Procedure: Calculate Visit Total
CREATE PROCEDURE sp_CalculateVisitTotal
    @VisitID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Total DECIMAL(10,2);
    
    SELECT @Total = SUM(vs.Amount * vs.Quantity)
    FROM VisitServices vs
    WHERE vs.VisitID = @VisitID;
    
    UPDATE Visits 
    SET TotalAmount = COALESCE(@Total, 0),
        ModifiedDate = GETDATE()
    WHERE VisitID = @VisitID;
    
    SELECT COALESCE(@Total, 0) AS TotalAmount;
END
GO

-- Stored Procedure: Get Activity Log
CREATE PROCEDURE sp_GetActivityLog
    @EntityType NVARCHAR(50) = NULL,
    @StartDate DATE = NULL,
    @EndDate DATE = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 50
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    SELECT 
        al.ActivityLogID, al.EntityType, al.EntityID, al.Action, 
        al.Details, al.IPAddress, al.CreatedDate,
        u.FirstName + ' ' + u.LastName AS UserName,
        u.Role
    FROM ActivityLogs al
    LEFT JOIN Users u ON al.UserID = u.UserID
    WHERE (@EntityType IS NULL OR al.EntityType = @EntityType)
        AND (@StartDate IS NULL OR CAST(al.CreatedDate AS DATE) >= @StartDate)
        AND (@EndDate IS NULL OR CAST(al.CreatedDate AS DATE) <= @EndDate)
    ORDER BY al.CreatedDate DESC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
    
    -- Return total count for pagination
    SELECT COUNT(*) AS TotalRecords
    FROM ActivityLogs al
    WHERE (@EntityType IS NULL OR al.EntityType = @EntityType)
        AND (@StartDate IS NULL OR CAST(al.CreatedDate AS DATE) >= @StartDate)
        AND (@EndDate IS NULL OR CAST(al.CreatedDate AS DATE) <= @EndDate);
END
GO

-- Stored Procedure: Search Patients
CREATE PROCEDURE sp_SearchPatients
    @SearchTerm NVARCHAR(100) = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 20
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    SELECT 
        p.PatientID, p.FirstName, p.LastName, p.DateOfBirth, p.Gender,
        p.Email, p.Phone, p.Address, p.CreatedDate,
        COUNT(v.VisitID) AS TotalVisits
    FROM Patients p
    LEFT JOIN Visits v ON p.PatientID = v.PatientID AND v.Status != 'Cancelled'
    WHERE p.IsActive = 1
        AND (@SearchTerm IS NULL OR 
             p.FirstName LIKE '%' + @SearchTerm + '%' OR
             p.LastName LIKE '%' + @SearchTerm + '%' OR
             p.Email LIKE '%' + @SearchTerm + '%' OR
             p.Phone LIKE '%' + @SearchTerm + '%')
    GROUP BY p.PatientID, p.FirstName, p.LastName, p.DateOfBirth, p.Gender,
             p.Email, p.Phone, p.Address, p.CreatedDate
    ORDER BY p.LastName, p.FirstName
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
    
    -- Return total count for pagination
    SELECT COUNT(*) AS TotalRecords
    FROM Patients p
    WHERE p.IsActive = 1
        AND (@SearchTerm IS NULL OR 
             p.FirstName LIKE '%' + @SearchTerm + '%' OR
             p.LastName LIKE '%' + @SearchTerm + '%' OR
             p.Email LIKE '%' + @SearchTerm + '%' OR
             p.Phone LIKE '%' + @SearchTerm + '%');
END
GO

-- Stored Procedure: Get Doctor Statistics
CREATE PROCEDURE sp_GetDoctorStatistics
    @DoctorID INT,
    @StartDate DATE,
    @EndDate DATE
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        COUNT(v.VisitID) AS TotalVisits,
        SUM(CASE WHEN v.Status = 'Completed' THEN 1 ELSE 0 END) AS CompletedVisits,
        SUM(CASE WHEN v.Status = 'Cancelled' THEN 1 ELSE 0 END) AS CancelledVisits,
        SUM(CASE WHEN v.Status = 'Scheduled' THEN 1 ELSE 0 END) AS ScheduledVisits,
        AVG(v.Duration) AS AverageDuration,
        SUM(v.TotalAmount) AS TotalRevenue
    FROM Visits v
    WHERE v.DoctorID = @DoctorID
        AND CAST(v.VisitDate AS DATE) BETWEEN @StartDate AND @EndDate;
END
GO
