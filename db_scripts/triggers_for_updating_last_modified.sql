CREATE TRIGGER trgUpdateLastModifiedWhenProjectUpdated
ON Project
AFTER UPDATE
AS
BEGIN
    UPDATE Project
    SET LastModified = SYSDATETIMEOFFSET() AT TIME ZONE 'Central European Standard Time'
    WHERE ProjectId IN (SELECT DISTINCT ProjectId FROM Inserted);
END;
GO

CREATE TRIGGER trgUpdateLastModifiedWhenParticipationInsertedOrDeleted
ON Participation
AFTER INSERT, DELETE
AS
BEGIN
    UPDATE Project
    SET LastModified = SYSDATETIMEOFFSET() AT TIME ZONE 'Central European Standard Time'
    WHERE ProjectId IN (SELECT DISTINCT ProjectId FROM Inserted)
        OR ProjectId IN (SELECT DISTINCT ProjectId FROM Deleted);
END;
GO

CREATE TRIGGER trgUpdateLastModifiedWhenTaskInsertedUpdatedOrDeleted
ON Task
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    UPDATE Project
    SET LastModified = SYSDATETIMEOFFSET() AT TIME ZONE 'Central European Standard Time'
    WHERE ProjectId IN (SELECT DISTINCT ProjectId FROM Inserted)
        OR ProjectId IN (SELECT DISTINCT ProjectId FROM Deleted);
END;
GO

CREATE TRIGGER trgUpdateLastModifiedWhenTaskTagInsertedOrDeleted
ON TaskTag
AFTER INSERT, DELETE
AS
BEGIN
    UPDATE Project
    SET LastModified = SYSDATETIMEOFFSET() AT TIME ZONE 'Central European Standard Time'
    WHERE ProjectId IN (
        SELECT DISTINCT Task.ProjectId
        FROM Task
        INNER JOIN Inserted ON Task.TaskId = Inserted.TaskId
        UNION
        SELECT DISTINCT Task.ProjectId
        FROM Task
        INNER JOIN Deleted ON Task.TaskId = Deleted.TaskId
    );
END;
GO

CREATE TRIGGER trgUpdateLastModifiedWhenAttachmentInsertedOrDeleted
ON Attachment
AFTER INSERT, DELETE
AS
BEGIN
    UPDATE Project
    SET LastModified = SYSDATETIMEOFFSET() AT TIME ZONE 'Central European Standard Time'
    WHERE ProjectId IN (
        SELECT DISTINCT Task.ProjectId
        FROM Task
        INNER JOIN Inserted ON Task.TaskId = Inserted.TaskId
        UNION
        SELECT DISTINCT Task.ProjectId
        FROM Task
        INNER JOIN Deleted ON Task.TaskId = Deleted.TaskId
    );
END;
GO

CREATE TRIGGER trgUpdateLastModifiedWhenCommentInsertedUpdatedOrDeleted
ON Comment
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    UPDATE Project
    SET LastModified = SYSDATETIMEOFFSET() AT TIME ZONE 'Central European Standard Time'
    WHERE ProjectId IN (
        SELECT DISTINCT Task.ProjectId
        FROM Task
        INNER JOIN Inserted ON Task.TaskId = Inserted.TaskId
        UNION
        SELECT DISTINCT Task.ProjectId
        FROM Task
        INNER JOIN Deleted ON Task.TaskId = Deleted.TaskId
    );
END;
GO

CREATE TRIGGER trgUpdateLastModifiedWhenAssignmentInsertedOrDeleted
ON Assignment
AFTER INSERT, DELETE
AS
BEGIN
    UPDATE Project
    SET LastModified = SYSDATETIMEOFFSET() AT TIME ZONE 'Central European Standard Time'
    WHERE ProjectId IN (
        SELECT DISTINCT Task.ProjectId
        FROM Task
        INNER JOIN Inserted ON Task.TaskId = Inserted.TaskId
        UNION
        SELECT DISTINCT Task.ProjectId
        FROM Task
        INNER JOIN Deleted ON Task.TaskId = Deleted.TaskId
    );
END;
GO
