-- =============================================
-- Short Notes Dynamic Schema
-- =============================================
-- Description: This script creates the dynamic subjects and notes schema
-- Run this script in your SQL Server database before using the dynamic features

-- =============================================
-- Create Subjects Table
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Subjects' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Subjects (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        Slug NVARCHAR(100) NOT NULL UNIQUE,
        Description NVARCHAR(500) NULL,
        CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
        CONSTRAINT CK_Subjects_Slug_LowerCase CHECK (Slug = LOWER(Slug))
    );
    
    PRINT 'Subjects table created successfully';
END
ELSE
BEGIN
    PRINT 'Subjects table already exists';
END
GO

-- =============================================
-- Create Notes Table
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Notes' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Notes (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        SubjectId INT NOT NULL,
        Title NVARCHAR(200) NOT NULL,
        Content NVARCHAR(MAX) NOT NULL,
        CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_Notes_Subjects FOREIGN KEY (SubjectId) 
            REFERENCES dbo.Subjects(Id) ON DELETE CASCADE
    );
    
    -- Create index for better query performance
    CREATE NONCLUSTERED INDEX IX_Notes_SubjectId ON dbo.Notes(SubjectId);
    
    PRINT 'Notes table created successfully';
END
ELSE
BEGIN
    PRINT 'Notes table already exists';
END
GO

-- =============================================
-- Insert Sample Subjects (Optional)
-- =============================================
-- Uncomment the lines below to insert sample subjects
/*
IF NOT EXISTS (SELECT * FROM dbo.Subjects WHERE Slug = 'physics')
BEGIN
    INSERT INTO dbo.Subjects (Name, Slug, Description) 
    VALUES ('Physics', 'physics', 'Physics notes and concepts');
    PRINT 'Sample Physics subject created';
END

IF NOT EXISTS (SELECT * FROM dbo.Subjects WHERE Slug = 'chemistry')
BEGIN
    INSERT INTO dbo.Subjects (Name, Slug, Description) 
    VALUES ('Chemistry', 'chemistry', 'Chemistry notes and concepts');
    PRINT 'Sample Chemistry subject created';
END

IF NOT EXISTS (SELECT * FROM dbo.Subjects WHERE Slug = 'combined-maths')
BEGIN
    INSERT INTO dbo.Subjects (Name, Slug, Description) 
    VALUES ('Combined Maths', 'combined-maths', 'Combined Mathematics notes');
    PRINT 'Sample Combined Maths subject created';
END
*/
GO

-- =============================================
-- MIGRATION TEMPLATE: Migrating Old Tables
-- =============================================
-- Use these templates to migrate data from old per-subject tables
-- to the new unified schema. Replace 'physics' with your actual table name.

/*
-- Step 1: Create subject entry if it doesn't exist
DECLARE @SubjectId INT;

IF NOT EXISTS (SELECT * FROM dbo.Subjects WHERE Slug = 'physics')
BEGIN
    INSERT INTO dbo.Subjects (Name, Slug, Description)
    VALUES ('Physics', 'physics', 'Physics notes and concepts');
END

-- Get the subject ID
SELECT @SubjectId = Id FROM dbo.Subjects WHERE Slug = 'physics';

-- Step 2: Migrate notes from old physics table (adjust column names as needed)
-- Assuming old table structure: physics (id, title, content, date)
INSERT INTO dbo.Notes (SubjectId, Title, Content, CreatedDate)
SELECT 
    @SubjectId,
    title,              -- Adjust column name if different
    content,            -- Adjust column name if different
    ISNULL(date, GETDATE())  -- Adjust column name if different, use current date if null
FROM physics
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.Notes n 
    WHERE n.SubjectId = @SubjectId 
    AND n.Title = physics.title
);

PRINT 'Physics notes migrated successfully';
*/

-- =============================================
-- Repeat the above template for other subjects:
-- =============================================
/*
-- CHEMISTRY MIGRATION
DECLARE @ChemistrySubjectId INT;

IF NOT EXISTS (SELECT * FROM dbo.Subjects WHERE Slug = 'chemistry')
BEGIN
    INSERT INTO dbo.Subjects (Name, Slug, Description)
    VALUES ('Chemistry', 'chemistry', 'Chemistry notes and concepts');
END

SELECT @ChemistrySubjectId = Id FROM dbo.Subjects WHERE Slug = 'chemistry';

INSERT INTO dbo.Notes (SubjectId, Title, Content, CreatedDate)
SELECT 
    @ChemistrySubjectId,
    title,
    content,
    ISNULL(date, GETDATE())
FROM chemistry
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.Notes n 
    WHERE n.SubjectId = @ChemistrySubjectId 
    AND n.Title = chemistry.title
);

PRINT 'Chemistry notes migrated successfully';
*/

/*
-- COMBINED MATHS MIGRATION
DECLARE @MathsSubjectId INT;

IF NOT EXISTS (SELECT * FROM dbo.Subjects WHERE Slug = 'combined-maths')
BEGIN
    INSERT INTO dbo.Subjects (Name, Slug, Description)
    VALUES ('Combined Maths', 'combined-maths', 'Combined Mathematics notes');
END

SELECT @MathsSubjectId = Id FROM dbo.Subjects WHERE Slug = 'combined-maths';

INSERT INTO dbo.Notes (SubjectId, Title, Content, CreatedDate)
SELECT 
    @MathsSubjectId,
    title,
    content,
    ISNULL(date, GETDATE())
FROM combinedMaths  -- Adjust table name as needed
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.Notes n 
    WHERE n.SubjectId = @MathsSubjectId 
    AND n.Title = combinedMaths.title
);

PRINT 'Combined Maths notes migrated successfully';
*/

-- =============================================
-- Verification Queries
-- =============================================
-- Run these queries to verify the migration

/*
-- Count subjects
SELECT COUNT(*) AS TotalSubjects FROM dbo.Subjects;

-- Count notes per subject
SELECT 
    s.Name AS SubjectName,
    COUNT(n.Id) AS NoteCount
FROM dbo.Subjects s
LEFT JOIN dbo.Notes n ON s.Id = n.SubjectId
GROUP BY s.Name
ORDER BY s.Name;

-- View all subjects
SELECT * FROM dbo.Subjects ORDER BY Name;

-- View sample notes
SELECT TOP 10
    s.Name AS Subject,
    n.Title,
    LEFT(n.Content, 50) AS ContentPreview,
    n.CreatedDate
FROM dbo.Notes n
INNER JOIN dbo.Subjects s ON n.SubjectId = s.Id
ORDER BY n.CreatedDate DESC;
*/
