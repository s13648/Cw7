


IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'Password'
          AND Object_ID = Object_ID(N'dbo.Student'))
BEGIN

	ALTER TABLE Student 
	ADD [Password] VARCHAR (MAX) NULL;

END