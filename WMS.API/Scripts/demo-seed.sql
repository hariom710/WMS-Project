USE WMSDB;
GO

-- ============================================================
-- CLEANUP: Remove audit tester login (not a real user)
-- ============================================================
DELETE FROM UserLogins WHERE Username = 'audit.tester@test.com';
GO

-- ============================================================
-- PHASE 1: EMPLOYEES (10 total: 1 admin + 1 manager + 8 employees)
-- Existing: EmployeeId 1 (Hariom Balang), EmployeeId 5 (Aniket Wasnik)
-- ============================================================

-- Fix existing employee: Hariom Balang should be Admin
UPDATE Employees SET
    Gender = 'M',
    DateOfBirth = '1990-03-15',
    DateOfJoining = '2022-01-10',
    Status = 'Active'
WHERE EmployeeId = 1;

-- Fix existing employee: Aniket Wasnik
UPDATE Employees SET
    Gender = 'M',
    DateOfBirth = '1992-07-22',
    DateOfJoining = '2022-06-15',
    Status = 'Active'
WHERE EmployeeId = 5;

-- Delete existing allocations and reseed
DELETE FROM ProjectAllocations;
DBCC CHECKIDENT ('ProjectAllocations', RESEED, 0);
GO

-- Insert 8 new employees
INSERT INTO Employees (FirstName, LastName, Email, PhoneNumber, Gender, DateOfBirth, DateOfJoining, DepartmentId, RoleId, Status, CreatedBy, CreatedDate, IsDeleted)
VALUES
('Rahul',   'Sharma',    'rahul.sharma@wms.com',     '9876543210', 'M', '1991-04-12', '2022-02-15', 1,  5,  'Active', 'admin', GETUTCDATE(), 0),
('Priya',   'Patel',     'priya.patel@wms.com',      '9876543211', 'F', '1993-08-25', '2022-04-01', 2,  6,  'Active', 'admin', GETUTCDATE(), 0),
('Amit',    'Verma',     'amit.verma@wms.com',       '9876543212', 'M', '1990-11-08', '2022-03-10', 3,  7,  'Active', 'admin', GETUTCDATE(), 0),
('Sneha',   'Kulkarni',  'sneha.kulkarni@wms.com',   '9876543213', 'F', '1994-01-30', '2023-01-05', 4,  8,  'Active', 'admin', GETUTCDATE(), 0),
('Rohit',   'Gupta',     'rohit.gupta@wms.com',      '9876543214', 'M', '1989-06-18', '2021-08-20', 6,  16, 'Active', 'admin', GETUTCDATE(), 0),
('Neha',    'Joshi',     'neha.joshi@wms.com',       '9876543215', 'F', '1992-09-05', '2022-09-12', 10, 24, 'Active', 'admin', GETUTCDATE(), 0),
('Karan',   'Mehta',     'karan.mehta@wms.com',      '9876543216', 'M', '1993-03-14', '2023-03-20', 11, 19, 'Active', 'admin', GETUTCDATE(), 0),
('Anjali',  'Deshmukh',  'anjali.deshmukh@wms.com',  '9876543217', 'F', '1991-12-01', '2022-07-01', 13, 22, 'Active', 'admin', GETUTCDATE(), 0);
GO

-- ============================================================
-- PHASE 7: USER LOGINS (5 users: admin + manager + 3 employees)
-- ============================================================

-- Admin: admin / Admin@123 (BCrypt hash)
-- Hash for Admin@123: $2a$11$... (pre-computed)
DECLARE @AdminHash NVARCHAR(200) = '$2a$11$rWbH1LzG1DjR0j3BpC9zMuQxKxJ8xK5V5xY5xZ5xY5xZ5xY5xZ5xZ';
DECLARE @EmpHash NVARCHAR(200)  = '$2a$11$rWbH1LzG1DjR0j3BpC9zMuQxKxJ8xK5V5xY5xZ5xY5xY5xZ5xZ5xZ';

-- Get the actual BCrypt hashes from the existing admin login
DECLARE @ExistingAdminHash NVARCHAR(200);
SELECT @ExistingAdminHash = PasswordHash FROM UserLogins WHERE Username = 'admin';

-- Create logins for employees
-- Use the same BCrypt hash as the admin so password works
-- Employee 1: rahul.sharma (Manager)
INSERT INTO UserLogins (Username, Password, RoleId)
SELECT 'rahul.sharma', @ExistingAdminHash, 2
WHERE NOT EXISTS (SELECT 1 FROM UserLogins WHERE Username = 'rahul.sharma');

-- Employee 2: priya.patel (Employee)
INSERT INTO UserLogins (Username, Password, RoleId)
SELECT 'priya.patel', @ExistingAdminHash, 33
WHERE NOT EXISTS (SELECT 1 FROM UserLogins WHERE Username = 'priya.patel');

-- Employee 3: amit.verma (Employee)
INSERT INTO UserLogins (Username, Password, RoleId)
SELECT 'amit.verma', @ExistingAdminHash, 33
WHERE NOT EXISTS (SELECT 1 FROM UserLogins WHERE Username = 'amit.verma');

-- Employee 4: sneha.kulkarni (Employee)
INSERT INTO UserLogins (Username, Password, RoleId)
SELECT 'sneha.kulkarni', @ExistingAdminHash, 33
WHERE NOT EXISTS (SELECT 1 FROM UserLogins WHERE Username = 'sneha.kulkarni');

GO

-- ============================================================
-- PHASE 2: PROJECT ALLOCATIONS
-- All employees assigned to at least one project, some to multiple
-- ============================================================

-- Employee 1 (Hariom - Admin, FullStack): Digital Banking + Employee Portal
INSERT INTO ProjectAllocations (EmpId, ProjectId, AssignedOn, Status, Note, CreatedBy, CreatedDate, IsDeleted)
VALUES
(1, 1, '2022-02-15', 1, 'Lead developer - Digital Banking Platform', 'admin', GETUTCDATE(), 0),
(1, 3, '2023-01-10', 1, 'Technical lead - Employee Management Portal', 'admin', GETUTCDATE(), 0);

-- Employee 2 (Aniket - Full Stack): Cloud Migration
INSERT INTO ProjectAllocations (EmpId, ProjectId, AssignedOn, Status, Note, CreatedBy, CreatedDate, IsDeleted)
VALUES
(2, 2, '2022-06-15', 1, 'Cloud infrastructure architect', 'admin', GETUTCDATE(), 0),
(2, 6, '2023-06-01', 1, 'Smart Manufacturing - backend services', 'admin', GETUTCDATE(), 0);

-- Employee 3 (Rahul - Manager): Digital Banking + AI Bot
INSERT INTO ProjectAllocations (EmpId, ProjectId, AssignedOn, Status, Note, CreatedBy, CreatedDate, IsDeleted)
VALUES
(3, 1, '2022-02-15', 1, 'Project manager - Digital Banking', 'admin', GETUTCDATE(), 0),
(3, 4, '2023-04-01', 1, 'Project manager - AI Customer Support', 'admin', GETUTCDATE(), 0);

-- Employee 4 (Priya - Frontend): Employee Portal + Retail Dashboard
INSERT INTO ProjectAllocations (EmpId, ProjectId, AssignedOn, Status, Note, CreatedBy, CreatedDate, IsDeleted)
VALUES
(4, 3, '2022-04-01', 1, 'Frontend lead - Angular development', 'admin', GETUTCDATE(), 0),
(4, 5, '2023-02-15', 1, 'UI/UX development - Analytics dashboard', 'admin', GETUTCDATE(), 0);

-- Employee 5 (Amit - Backend): Cloud Migration + Supply Chain
INSERT INTO ProjectAllocations (EmpId, ProjectId, AssignedOn, Status, Note, CreatedBy, CreatedDate, IsDeleted)
VALUES
(5, 2, '2022-03-10', 1, 'Backend API development - .NET services', 'admin', GETUTCDATE(), 0),
(5, 8, '2023-05-01', 1, 'Supply chain optimization - backend', 'admin', GETUTCDATE(), 0);

-- Employee 6 (Sneha - FullStack): Employee Portal
INSERT INTO ProjectAllocations (EmpId, ProjectId, AssignedOn, Status, Note, CreatedBy, CreatedDate, IsDeleted)
VALUES
(6, 3, '2023-01-05', 1, 'Full stack development - React + Node.js', 'admin', GETUTCDATE(), 0);

-- Employee 7 (Rohit - DevOps): Cyber Security + Smart Manufacturing
INSERT INTO ProjectAllocations (EmpId, ProjectId, AssignedOn, Status, Note, CreatedBy, CreatedDate, IsDeleted)
VALUES
(7, 7, '2021-08-20', 1, 'DevOps lead - CI/CD pipelines', 'admin', GETUTCDATE(), 0),
(7, 6, '2022-10-01', 1, 'Infrastructure automation - manufacturing', 'admin', GETUTCDATE(), 0);

-- Employee 8 (Neha - QA): Digital Banking + Cloud Migration + ERP
INSERT INTO ProjectAllocations (EmpId, ProjectId, AssignedOn, Status, Note, CreatedBy, CreatedDate, IsDeleted)
VALUES
(8, 1, '2022-09-12', 1, 'QA lead - test automation', 'admin', GETUTCDATE(), 0),
(8, 2, '2023-03-01', 1, 'QA - Cloud migration testing', 'admin', GETUTCDATE(), 0),
(8, 10, '2023-08-01', 1, 'QA - ERP regression testing', 'admin', GETUTCDATE(), 0);

-- Employee 9 (Karan - Data): AI Bot + Retail Analytics
INSERT INTO ProjectAllocations (EmpId, ProjectId, AssignedOn, Status, Note, CreatedBy, CreatedDate, IsDeleted)
VALUES
(9, 4, '2023-03-20', 1, 'Data pipeline development', 'admin', GETUTCDATE(), 0),
(9, 5, '2023-06-15', 1, 'Data engineering - ETL processes', 'admin', GETUTCDATE(), 0);

-- Employee 10 (Anjali - AI): AI Bot + Healthcare
INSERT INTO ProjectAllocations (EmpId, ProjectId, AssignedOn, Status, Note, CreatedBy, CreatedDate, IsDeleted)
VALUES
(10, 4, '2022-07-01', 1, 'ML model development', 'admin', GETUTCDATE(), 0),
(10, 9, '2023-04-15', 1, 'Healthcare AI - data analysis', 'admin', GETUTCDATE(), 0);

GO

-- ============================================================
-- PHASE 3: ATTENDANCE (Last 30 days for all 10 employees)
-- ============================================================

-- Clear existing attendance
DELETE FROM Attendances;
DBCC CHECKIDENT ('Attendances', RESEED, 0);
GO

DECLARE @EmpId INT = 1;
DECLARE @Date DATE;
DECLARE @CheckIn DATETIME;
DECLARE @CheckOut DATETIME;
DECLARE @Hours FLOAT;
DECLARE @WorkMode NVARCHAR(20);
DECLARE @DayOfWeek INT;

-- Loop through 30 days
DECLARE @Day INT = 0;
WHILE @Day < 30
BEGIN
    SET @Date = DATEADD(DAY, -@Day, CAST(GETUTCDATE() AS DATE));
    SET @DayOfWeek = DATEPART(WEEKDAY, @Date);

    -- Skip weekends (1=Sunday, 7=Saturday)
    IF @DayOfWeek IN (1, 7)
    BEGIN
        SET @Day = @Day + 1;
        CONTINUE;
    END

    -- For each employee (1-10)
    SET @EmpId = 1;
    WHILE @EmpId <= 10
    BEGIN
        -- Vary work mode by employee and day
        SET @WorkMode = CASE
            WHEN (@EmpId + @Day) % 7 = 0 THEN 'Remote'
            WHEN (@EmpId + @Day) % 5 = 0 THEN 'Hybrid'
            ELSE 'Office'
        END;

        -- Vary check-in time: most days 9:00-9:30, some late (9:45-10:15)
        SET @CheckIn = CASE
            WHEN (@EmpId + @Day) % 8 = 0 THEN DATEADD(MINUTE, 45 + ((@EmpId * 3 + @Day * 7) % 30), CAST(CAST(@Date AS DATETIME) + ' 09:00:00' AS DATETIME))  -- Late: 9:45-10:15
            WHEN (@EmpId + @Day) % 11 = 0 THEN DATEADD(MINUTE, 50 + ((@EmpId * 5 + @Day * 3) % 25), CAST(CAST(@Date AS DATETIME) + ' 09:00:00' AS DATETIME))  -- Late: 9:50-10:15
            ELSE DATEADD(MINUTE, ((@EmpId * 2 + @Day * 5) % 30), CAST(CAST(@Date AS DATETIME) + ' 09:00:00' AS DATETIME))  -- On time: 9:00-9:30
        END;

        -- Check-out: 5:00 PM to 6:30 PM
        SET @CheckOut = DATEADD(MINUTE, 300 + ((@EmpId * 3 + @Day * 7) % 90), CAST(CAST(@Date AS DATETIME) + ' 09:00:00' AS DATETIME));

        -- Total hours
        SET @Hours = DATEDIFF(MINUTE, @CheckIn, @CheckOut) / 60.0;

        INSERT INTO Attendances (EmpId, CheckIn, CheckOut, TotalHours, WorkMode, AttendanceDate)
        VALUES (@EmpId, @CheckIn, @CheckOut, @Hours, @WorkMode, @Date);

        SET @EmpId = @EmpId + 1;
    END

    SET @Day = @Day + 1;
END

-- Verify
SELECT COUNT(*) AS TotalAttendance FROM Attendances;
SELECT WorkMode, COUNT(*) AS Cnt FROM Attendances GROUP BY WorkMode;
GO

-- ============================================================
-- PHASE 4: LEAVES (Pending, Approved, Rejected for multiple employees)
-- ============================================================

DELETE FROM Leaves;
DBCC CHECKIDENT ('Leaves', RESEED, 0);
GO

DECLARE @Today DATE = CAST(GETUTCDATE() AS DATE);

-- PENDING LEAVES
INSERT INTO Leaves (EmpId, LeaveType, Reason, FromDate, ToDate, Status, CreatedBy, CreatedDate, IsDeleted)
VALUES
(2, 'Sick',      'Need to visit doctor for routine health checkup and follow-up consultation', DATEADD(DAY, 2, @Today), DATEADD(DAY, 3, @Today), 'Pending', 'priya.patel', GETUTCDATE(), 0),
(4, 'Casual',    'Family function - cousin wedding ceremony in hometown', DATEADD(DAY, 5, @Today), DATEADD(DAY, 7, @Today), 'Pending', 'sneha.kulkarni', GETUTCDATE(), 0),
(5, 'Vacation',  'Planned family vacation to Goa for summer holidays with kids', DATEADD(DAY, 10, @Today), DATEADD(DAY, 15, @Today), 'Pending', 'amit.verma', GETUTCDATE(), 0),
(9, 'Casual',    'Personal work - need to visit bank for property registration documents', DATEADD(DAY, 3, @Today), DATEADD(DAY, 3, @Today), 'Pending', 'karan.mehta', GETUTCDATE(), 0);

-- APPROVED LEAVES (past and future)
INSERT INTO Leaves (EmpId, LeaveType, Reason, FromDate, ToDate, Status, ApprovedBy, ApprovedOn, CreatedBy, CreatedDate, IsDeleted)
VALUES
(3, 'Sick',      'Fever and cold - doctor recommended bed rest for 2 days', DATEADD(DAY, -15, @Today), DATEADD(DAY, -14, @Today), 'Approved', 1, DATEADD(DAY, -16, @Today), 'rahul.sharma', DATEADD(DAY, -16, @Today), 0),
(6, 'Casual',    'Urgent family matter - need to travel to hometown immediately', DATEADD(DAY, -10, @Today), DATEADD(DAY, -9, @Today), 'Approved', 1, DATEADD(DAY, -11, @Today), 'nehajoshi', DATEADD(DAY, -11, @Today), 0),
(7, 'Vacation',  'Annual leave - planning trip to Manali with college friends group', DATEADD(DAY, -5, @Today), DATEADD(DAY, -2, @Today), 'Approved', 1, DATEADD(DAY, -6, @Today), 'rohit.gupta', DATEADD(DAY, -6, @Today), 0),
(2, 'Casual',    'Festival celebration - Diwali preparations and family gatherings', DATEADD(DAY, 20, @Today), DATEADD(DAY, 22, @Today), 'Approved', 1, DATEADD(DAY, -1, @Today), 'priya.patel', GETUTCDATE(), 0),
(8, 'Sick',      'Dental appointment - scheduled root canal treatment procedure', DATEADD(DAY, 7, @Today), DATEADD(DAY, 7, @Today), 'Approved', 1, DATEADD(DAY, -2, @Today), 'nehajoshi', DATEADD(DAY, -3, @Today), 0);

-- REJECTED LEAVES
INSERT INTO Leaves (EmpId, LeaveType, Reason, FromDate, ToDate, Status, ApprovedBy, ApprovedOn, CreatedBy, CreatedDate, IsDeleted)
VALUES
(10, 'Casual',   'Personal trip to Bangalore - visiting college friends for reunion', DATEADD(DAY, -3, @Today), DATEADD(DAY, -1, @Today), 'Rejected', 1, DATEADD(DAY, -4, @Today), 'anjali.deshmukh', DATEADD(DAY, -5, @Today), 0),
(6,  'Vacation', 'Extended holiday - trip to Europe for 2 weeks vacation', DATEADD(DAY, 15, @Today), DATEADD(DAY, 28, @Today), 'Rejected', 1, DATEADD(DAY, -1, @Today), 'nehajoshi', GETUTCDATE(), 0);

GO

-- ============================================================
-- PHASE 9: Verify AuditLog entries
-- ============================================================
SELECT TOP 20
    al.Action,
    al.EntityName,
    al.EntityId,
    al.Username,
    al.Timestamp
FROM AuditLogs al
ORDER BY al.Timestamp DESC;

-- ============================================================
-- FINAL COUNTS
-- ============================================================
PRINT '=== FINAL DEMO DATA COUNTS ===';
SELECT 'Employees' AS [Table], COUNT(*) AS [Count] FROM Employees WHERE IsDeleted = 0
UNION ALL SELECT 'UserLogins', COUNT(*) FROM UserLogins
UNION ALL SELECT 'Departments', COUNT(*) FROM Departments WHERE IsDeleted = 0
UNION ALL SELECT 'Roles', COUNT(*) FROM Roles
UNION ALL SELECT 'Clients', COUNT(*) FROM Clients WHERE IsDeleted = 0
UNION ALL SELECT 'Projects', COUNT(*) FROM Projects WHERE IsDeleted = 0
UNION ALL SELECT 'Allocations', COUNT(*) FROM ProjectAllocations WHERE IsDeleted = 0
UNION ALL SELECT 'Attendance', COUNT(*) FROM Attendances
UNION ALL SELECT 'Leaves', COUNT(*) FROM Leaves WHERE IsDeleted = 0
UNION ALL SELECT 'Announcements', COUNT(*) FROM Announcements WHERE IsDeleted = 0
UNION ALL SELECT 'AuditLogs', COUNT(*) FROM AuditLogs;
GO
