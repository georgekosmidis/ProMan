-- Correct entries in BOC when new project/projectrepository is added
UPDATE b 
SET b.ProjectID = t.ProjectID, b.ProjectRepositoryID = t.ProjectRepositoryID
FROM BOC b
INNER JOIN (
    SELECT b.ID AS BocID, pr.ID AS ProjectRepositoryID, pr.ProjectID FROM BOC b 
        INNER JOIN Files f ON b.FileID = f.ID
        INNER JOIN ProjectRepositories pr ON REPLACE(REPLACE(SUBSTRING(f.Filename, 1, LEN(pr.Path)+1), '//', '/'), ':/', '://') = pr.Path
) t ON b.ID = t.BocID