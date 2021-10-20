CREATE PROC dbo.Guest_Delete
	(
	@GuestId int
)
AS
BEGIN
	SET NOCOUNT ON

	DELETE
	FROM dbo.Guest
	WHERE GuestID = @GuestId
END
GO

CREATE PROC dbo.Guest_Get_ByMeetingId
	(
	@MeetingId int
)
AS
BEGIN
	SET NOCOUNT ON

	SELECT GuestId, MeetingId, Content, Username, Created
	FROM dbo.Guest
	WHERE MeetingId = @MeetingId
END
GO

CREATE PROC dbo.Guest_Post
	(
	@MeetingId int,
	@Content nvarchar(max),
	@UserId nvarchar(150),
	@UserName nvarchar(150),
	@Created datetime2
)
AS
BEGIN
	SET NOCOUNT ON

	INSERT INTO dbo.Guest
		(MeetingId, Content, UserId, UserName, Created)
	SELECT @MeetingId, @Content, @UserId, @UserName, @Created

	SELECT GuestId, Content, UserName, UserId, Created
	FROM dbo.Guest
	WHERE GuestId = SCOPE_IDENTITY()
END
GO

CREATE PROC dbo.Guest_Put
	(
	@GuestId int,
	@Content nvarchar(max)
)
AS
BEGIN
	SET NOCOUNT ON

	UPDATE dbo.Guest
	SET Content = @Content
	WHERE GuestId = @GuestId

	SELECT a.GuestId, a.MeetingId, a.Content, u.UserName, a.Created
	FROM dbo.Guest a
		LEFT JOIN AspNetUsers u ON a.UserId = u.Id
	WHERE GuestId = @GuestId
END
GO


CREATE PROC dbo.Meeting_AddForLoadTest
AS
BEGIN
	DECLARE @i int = 1

	WHILE @i < 10000
	BEGIN
		INSERT INTO dbo.Meeting
			(Title, Content, UserId, UserName, Created)
		VALUES('Meeting ' + CAST(@i AS nvarchar(5)), 'Content ' + CAST(@i AS nvarchar(5)), 'User1', 'User1', GETUTCDATE())
		SET @i = @i + 1
	END
END
GO

CREATE PROC dbo.Meeting_Delete
	(
	@MeetingId int
)
AS
BEGIN
	SET NOCOUNT ON

	DELETE
	FROM dbo.Meeting
	WHERE MeetingID = @MeetingId
END
GO

CREATE PROC dbo.Meeting_Exists
	(
	@MeetingId int
)
AS
BEGIN
	SET NOCOUNT ON

	SELECT CASE WHEN EXISTS (SELECT MeetingId
		FROM dbo.Meeting
		WHERE MeetingId = @MeetingId) 
        THEN CAST (1 AS BIT) 
        ELSE CAST (0 AS BIT) END AS Result

END
GO

CREATE PROC dbo.Meeting_GetMany
AS
BEGIN
	SET NOCOUNT ON

	SELECT MeetingId, Title, Content, UserId, UserName, Created
	FROM dbo.Meeting 
END
GO

CREATE PROC dbo.Meeting_GetMany_BySearch
	(
	@Search nvarchar(100)
)
AS
BEGIN
	SET NOCOUNT ON

		SELECT MeetingId, Title, Content, UserId, UserName, Created
		FROM dbo.Meeting 
		WHERE Title LIKE '%' + @Search + '%'

	UNION

		SELECT MeetingId, Title, Content, UserId, UserName, Created
		FROM dbo.Meeting 
		WHERE Content LIKE '%' + @Search + '%'
END
GO

CREATE PROC dbo.Meeting_GetMany_BySearch_WithPaging
	(
	@Search nvarchar(100),
	@PageNumber int,
	@PageSize int
)
AS
BEGIN
	SELECT MeetingId, Title, Content, UserId, UserName, Created
	FROM
		(	SELECT MeetingId, Title, Content, UserId, UserName, Created
			FROM dbo.Meeting 
			WHERE Title LIKE '%' + @Search + '%'

		UNION

			SELECT MeetingId, Title, Content, UserId, UserName, Created
			FROM dbo.Meeting 
			WHERE Content LIKE '%' + @Search + '%') Sub
	ORDER BY MeetingId
	OFFSET @PageSize * (@PageNumber - 1) ROWS
    FETCH NEXT @PageSize ROWS ONLY
END
GO

CREATE PROC dbo.Meeting_GetMany_WithGuests
AS
BEGIN
	SET NOCOUNT ON

	SELECT q.MeetingId, q.Title, q.Content, q.UserName, q.Created,
		a.MeetingId, a.GuestId, a.Content, a.Username, a.Created
	FROM dbo.Meeting q
		LEFT JOIN dbo.Guest a ON q.MeetingId = a.MeetingId
END
GO

CREATE PROC dbo.Meeting_GetSingle
	(
	@MeetingId int
)
AS
BEGIN
	SET NOCOUNT ON

	SELECT MeetingId, Title, Content, UserId, Username, Created
	FROM dbo.Meeting 
	WHERE MeetingId = @MeetingId
END
GO

CREATE PROC dbo.Meeting_GetUnanswered
AS
BEGIN
	SET NOCOUNT ON

	SELECT MeetingId, Title, Content, UserId, UserName, Created
	FROM dbo.Meeting q
	WHERE NOT EXISTS (SELECT *
	FROM dbo.Guest a
	WHERE a.MeetingId = q.MeetingId)
END
GO

CREATE PROC dbo.Meeting_Post
	(
	@Title nvarchar(100),
	@Content nvarchar(max),
	@UserId nvarchar(150),
	@UserName nvarchar(150),
	@Created datetime2
)
AS
BEGIN
	SET NOCOUNT ON

	INSERT INTO dbo.Meeting
		(Title, Content, UserId, UserName, Created)
	VALUES(@Title, @Content, @UserId, @UserName, @Created)

	SELECT SCOPE_IDENTITY() AS MeetingId
END
GO

CREATE PROC dbo.Meeting_Put
	(
	@MeetingId int,
	@Title nvarchar(100),
	@Content nvarchar(max)
)
AS
BEGIN
	SET NOCOUNT ON

	UPDATE dbo.Meeting
	SET Title = @Title, Content = @Content
	WHERE MeetingID = @MeetingId
END
GO

CREATE PROC dbo.Guest_Get_ByGuestId
	(
	@GuestId int
)
AS
BEGIN
	SET NOCOUNT ON

	SELECT GuestId, Content, Username, Created
	FROM dbo.Guest 
	WHERE GuestId = @GuestId
END
GO