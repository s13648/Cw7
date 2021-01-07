
CREATE PROCEDURE Enrollment_Promotions
	@Studies NVARCHAR(100),
	@Semester INT
AS
BEGIN
	BEGIN TRANSACTION 

	DECLARE @NewSemester INT = @Semester + 1

	DECLARE @NewEnrollmnetId INT;
	SELECT TOP 1
		@NewEnrollmnetId = E.IdEnrollment
	FROM 
		[Studies] AS S JOIN
		[Enrollment] AS E ON E.IdStudy = S.IdStudy
	WHERE
		S.[Name] = @Studies and e.Semester  = @NewSemester 

	IF @NewEnrollmnetId IS NULL
	BEGIN

		DECLARE @LastEnrollmentId INT = (SELECT MAX(E.IdEnrollment) FROM [Enrollment] AS E)
	
		DECLARE @StudyId INT = 
		(
			SELECT TOP 1 S.IdStudy
			FROM [Studies] AS S 
			WHERE S.[Name] = @Studies
		)
		
		SET @NewEnrollmnetId = @LastEnrollmentId + 1

		INSERT INTO [dbo].[Enrollment]
		(
				[IdEnrollment]
			   ,[Semester]
			   ,[IdStudy]
			   ,[StartDate])
		 VALUES
		 (	
				@NewEnrollmnetId
			   ,@NewSemester
			   ,@StudyId
			   ,GETDATE()
		)

	END


	UPDATE ST
	SET
		ST.IdEnrollment = @NewEnrollmnetId
	FROM 
		[Enrollment] AS E JOIN
		[Studies] AS S ON E.IdStudy = S.IdStudy JOIN
		[Student] AS ST ON ST.IdEnrollment = E.IdEnrollment
	WHERE
		S.[Name] = @Studies AND
		E.Semester = @Semester


	COMMIT
END
GO

--EXEC Enrollment_Promotions
--	@Studies = 'IT',
--	@Semester = 3