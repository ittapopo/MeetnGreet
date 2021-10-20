CREATE TABLE dbo.Meeting(
	MeetingId int IDENTITY(1,1) NOT NULL,
	Title nvarchar(100) NOT NULL,
	Content nvarchar(max) NOT NULL,
	UserId nvarchar(150) NOT NULL,
	UserName nvarchar(150) NOT NULL,
	Created datetime2(7) NOT NULL,
 CONSTRAINT PK_Meeting PRIMARY KEY CLUSTERED 
(
	MeetingId ASC
)
) 
GO

CREATE TABLE dbo.Guest(
	GuestId int IDENTITY(1,1) NOT NULL,
	MeetingId int NOT NULL,
	Content nvarchar(max) NOT NULL,
	UserId nvarchar(150) NOT NULL,
	UserName nvarchar(150) NOT NULL,
	Created datetime2(7) NOT NULL,
 CONSTRAINT PK_Guest PRIMARY KEY CLUSTERED 
(
	GuestId ASC
)
) 
GO
ALTER TABLE dbo.Guest  WITH CHECK ADD  CONSTRAINT FK_Guest_Meeting FOREIGN KEY(MeetingId)
REFERENCES dbo.Meeting (MeetingId)
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE dbo.Guest CHECK CONSTRAINT FK_Guest_Meeting
GO

SET IDENTITY_INSERT dbo.Meeting ON 
GO
INSERT INTO dbo.Meeting(MeetingId, Title, Content, UserId, UserName, Created)
VALUES(1, 'Status update meeting1', 
		'Quick breifing about the latest developmentQuick breifing about the latest development',
		'1',
		'bob.test@test.com',
		'2019-05-18 14:32')

INSERT INTO dbo.Meeting(MeetingId, Title, Content, UserId, UserName, Created)
VALUES(2, 'Regarding the community potluck1', 
		'We still need more preperationsWe still need more preperations',
		'2',
		'jane.test@test.com',
		'2019-05-18 14:48')
GO
SET IDENTITY_INSERT dbo.Meeting OFF
GO

SET IDENTITY_INSERT dbo.Guest ON 
GO
INSERT INTO dbo.Guest(GuestId, MeetingId, Content, UserId, UserName, Created)
VALUES(1, 1, 'So excited about this! I will be there!', '2', 'jane.test@test.com', '2019-05-18 14:40')

INSERT INTO dbo.Guest(GuestId, MeetingId, Content, UserId, UserName, Created)
VALUES(2, 1, 'I cant make it this time. So sorry', '3', 'fred.test@test.com', '2019-05-18 16:18')
GO
SET IDENTITY_INSERT dbo.Guest OFF 
GO