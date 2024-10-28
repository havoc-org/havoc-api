-- Created by Vertabelo (http://vertabelo.com)
-- Last modification date: 2024-10-27 17:59:47.511

-- tables
-- Table: Assignment
CREATE TABLE Assignment (
    UserId int  NOT NULL,
    TaskId int  NOT NULL,
    Description varchar(200)  NULL CHECK (LEN(LTRIM(RTRIM(Description))) > 0),
    CONSTRAINT Assignment_pk PRIMARY KEY  (UserId,TaskId)
);

-- Table: Attachment
CREATE TABLE Attachment (
    AttachmentId int  NOT NULL IDENTITY,
    UserId int  NOT NULL,
    TaskId int  NOT NULL,
    FileLink varchar(255)  NOT NULL CHECK (LEN(LTRIM(RTRIM(FileLink))) > 0),
    CONSTRAINT Attachment_pk PRIMARY KEY  (AttachmentId)
);

-- Table: Comment
CREATE TABLE Comment (
    CommentId int  NOT NULL IDENTITY,
    TaskId int  NOT NULL,
    UserId int  NOT NULL,
    Content varchar(200)  NOT NULL CHECK (LEN(LTRIM(RTRIM(Content))) > 0),
    CommentDate datetime  NOT NULL DEFAULT GETDATE(),
    CONSTRAINT Comment_pk PRIMARY KEY  (CommentId)
);

-- Table: Participation
CREATE TABLE Participation (
    ProjectId int  NOT NULL,
    UserId int  NOT NULL,
    RoleId int  NOT NULL,
    CONSTRAINT Participation_pk PRIMARY KEY  (ProjectId,UserId)
);

-- Table: Project
CREATE TABLE Project (
    ProjectId int  NOT NULL IDENTITY,
    Name varchar(25)  NOT NULL CHECK (LEN(LTRIM(RTRIM(Name))) > 0),
    Description varchar(200)  NULL CHECK (LEN(LTRIM(RTRIM(Description))) > 0),
    Background varbinary(max)  NULL,
    CreatorId int  NOT NULL,
    Start datetime  NULL CHECK (Deadline IS NULL OR Start < Deadline),
    Deadline datetime  NULL CHECK (Deadline >= GETDATE()),
    LastModified datetime  NOT NULL DEFAULT GETDATE(),
    ProjectStatusId int  NOT NULL,
    CONSTRAINT Project_pk PRIMARY KEY  (ProjectId)
);

-- Table: ProjectStatus
CREATE TABLE ProjectStatus (
    ProjectStatusId int  NOT NULL IDENTITY,
    Name varchar(20)  NOT NULL CHECK (LEN(LTRIM(RTRIM(Name))) > 0),
    CONSTRAINT ProjectStatus_pk PRIMARY KEY  (ProjectStatusId)
);

-- Table: Role
CREATE TABLE Role (
    RoleId int  NOT NULL IDENTITY,
    Name varchar(25)  NOT NULL CHECK (LEN(LTRIM(RTRIM(Name))) > 0),
    CONSTRAINT Role_pk PRIMARY KEY  (RoleId)
);

-- Table: Tag
CREATE TABLE Tag (
    TagId int  NOT NULL IDENTITY,
    Name varchar(20)  NOT NULL CHECK (LEN(LTRIM(RTRIM(Name))) > 0),
    ColorHEX varchar(7)  NOT NULL CHECK (ColorHEX LIKE '#[0-9A-Fa-f][0-9A-Fa-f][0-9A-Fa-f][0-9A-Fa-f][0-9A-Fa-f][0-9A-Fa-f]'),
    CONSTRAINT Tag_pk PRIMARY KEY  (TagId)
);

-- Table: Task
CREATE TABLE Task (
    TaskId int  NOT NULL IDENTITY,
    ProjectId int  NOT NULL,
    Name varchar(25)  NOT NULL CHECK (LEN(LTRIM(RTRIM(Name))) > 0),
    Description varchar(200)  NULL CHECK (LEN(LTRIM(RTRIM(Description))) > 0),
    Start datetime  NULL CHECK (Deadline IS NULL OR Start < Deadline),
    Deadline datetime  NULL CHECK (Deadline >= GETDATE()),
    CreatorId int  NOT NULL,
    TaskStatusId int  NOT NULL,
    CONSTRAINT Task_pk PRIMARY KEY  (TaskId)
);

-- Table: TaskStatus
CREATE TABLE TaskStatus (
    TaskStatusId int  NOT NULL IDENTITY,
    Name varchar(20)  NOT NULL CHECK (LEN(LTRIM(RTRIM(Name))) > 0),
    CONSTRAINT TaskStatus_pk PRIMARY KEY  (TaskStatusId)
);

-- Table: TaskTag
CREATE TABLE TaskTag (
    TagId int  NOT NULL,
    TaskId int  NOT NULL,
    CONSTRAINT TaskTag_pk PRIMARY KEY  (TagId,TaskId)
);

-- Table: User
CREATE TABLE "User" (
    UserId int  NOT NULL IDENTITY,
    FirstName varchar(50)  NOT NULL CHECK (LEN(LTRIM(RTRIM(FirstName))) > 0),
    LastName varchar(50)  NOT NULL CHECK (LEN(LTRIM(RTRIM(LastName))) > 0),
    Email varchar(100)  NOT NULL CHECK (Email LIKE '%_@_%._%'),
    Password varchar(128)  NOT NULL,
    Avatar varbinary(max)  NULL,
    CONSTRAINT User_pk PRIMARY KEY  (UserId)
);

-- foreign keys
-- Reference: Assignment_Task (table: Assignment)
ALTER TABLE Assignment ADD CONSTRAINT Assignment_Task
    FOREIGN KEY (TaskId)
    REFERENCES Task (TaskId);

-- Reference: Assingment_User (table: Assignment)
ALTER TABLE Assignment ADD CONSTRAINT Assingment_User
    FOREIGN KEY (UserId)
    REFERENCES "User" (UserId);

-- Reference: Attachment_Task (table: Attachment)
ALTER TABLE Attachment ADD CONSTRAINT Attachment_Task
    FOREIGN KEY (TaskId)
    REFERENCES Task (TaskId);

-- Reference: Attachment_User (table: Attachment)
ALTER TABLE Attachment ADD CONSTRAINT Attachment_User
    FOREIGN KEY (UserId)
    REFERENCES "User" (UserId);

-- Reference: Comment_Task (table: Comment)
ALTER TABLE Comment ADD CONSTRAINT Comment_Task
    FOREIGN KEY (TaskId)
    REFERENCES Task (TaskId);

-- Reference: Comment_User (table: Comment)
ALTER TABLE Comment ADD CONSTRAINT Comment_User
    FOREIGN KEY (UserId)
    REFERENCES "User" (UserId);

-- Reference: Participation_Project (table: Participation)
ALTER TABLE Participation ADD CONSTRAINT Participation_Project
    FOREIGN KEY (ProjectId)
    REFERENCES Project (ProjectId);

-- Reference: Participation_Role (table: Participation)
ALTER TABLE Participation ADD CONSTRAINT Participation_Role
    FOREIGN KEY (RoleId)
    REFERENCES Role (RoleId);

-- Reference: Participation_User (table: Participation)
ALTER TABLE Participation ADD CONSTRAINT Participation_User
    FOREIGN KEY (UserId)
    REFERENCES "User" (UserId);

-- Reference: Project_ProjectStatus (table: Project)
ALTER TABLE Project ADD CONSTRAINT Project_ProjectStatus
    FOREIGN KEY (ProjectStatusId)
    REFERENCES ProjectStatus (ProjectStatusId);

-- Reference: Project_User (table: Project)
ALTER TABLE Project ADD CONSTRAINT Project_User
    FOREIGN KEY (CreatorId)
    REFERENCES "User" (UserId);

-- Reference: Task_Project (table: Task)
ALTER TABLE Task ADD CONSTRAINT Task_Project
    FOREIGN KEY (ProjectId)
    REFERENCES Project (ProjectId);

-- Reference: Task_TaskStatus (table: Task)
ALTER TABLE Task ADD CONSTRAINT Task_TaskStatus
    FOREIGN KEY (TaskStatusId)
    REFERENCES TaskStatus (TaskStatusId);

-- Reference: Task_User (table: Task)
ALTER TABLE Task ADD CONSTRAINT Task_User
    FOREIGN KEY (CreatorId)
    REFERENCES "User" (UserId);

-- Reference: Type_Tag (table: TaskTag)
ALTER TABLE TaskTag ADD CONSTRAINT Type_Tag
    FOREIGN KEY (TagId)
    REFERENCES Tag (TagId);

-- Reference: Type_Task (table: TaskTag)
ALTER TABLE TaskTag ADD CONSTRAINT Type_Task
    FOREIGN KEY (TaskId)
    REFERENCES Task (TaskId);

-- End of file.

