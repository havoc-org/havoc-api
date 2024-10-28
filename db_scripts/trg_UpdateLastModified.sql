CREATE TRIGGER trg_UpdateLastModified
ON Project
AFTER UPDATE
AS
BEGIN
    UPDATE Project
    SET LastModified = GETDATE()
    WHERE ProjectId IN (SELECT DISTINCT ProjectId FROM Inserted);
END;