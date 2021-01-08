


IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'Password'
          AND Object_ID = Object_ID(N'dbo.Student'))
BEGIN

	ALTER TABLE Student 
	ADD [Password] VARCHAR (MAX) NULL;

END


IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'RefreshToken'
          AND Object_ID = Object_ID(N'dbo.Student'))
BEGIN

	ALTER TABLE Student 
	ADD [RefreshToken] VARCHAR (MAX) NULL;

END

IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'Salt'
          AND Object_ID = Object_ID(N'dbo.Student'))
BEGIN

	ALTER TABLE Student 
	ADD [Salt] VARCHAR (MAX) NULL;

END