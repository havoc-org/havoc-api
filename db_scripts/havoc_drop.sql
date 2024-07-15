-- Created by Vertabelo (http://vertabelo.com)
-- Last modification date: 2024-07-14 18:35:45.32

-- foreign keys
ALTER TABLE Assignment DROP CONSTRAINT Assignment_Task;

ALTER TABLE Assignment DROP CONSTRAINT Assingment_User;

ALTER TABLE Attachment DROP CONSTRAINT Attachment_Task;

ALTER TABLE Attachment DROP CONSTRAINT Attachment_User;

ALTER TABLE Comment DROP CONSTRAINT Comment_Task;

ALTER TABLE Comment DROP CONSTRAINT Comment_User;

ALTER TABLE Participation DROP CONSTRAINT Participation_Project;

ALTER TABLE Participation DROP CONSTRAINT Participation_Role;

ALTER TABLE Participation DROP CONSTRAINT Participation_User;

ALTER TABLE Project DROP CONSTRAINT Project_ProjectStatus;

ALTER TABLE Project DROP CONSTRAINT Project_User;

ALTER TABLE Task DROP CONSTRAINT Task_Project;

ALTER TABLE Task DROP CONSTRAINT Task_TaskStatus;

ALTER TABLE Task DROP CONSTRAINT Task_User;

ALTER TABLE TaskTag DROP CONSTRAINT Type_Tag;

ALTER TABLE TaskTag DROP CONSTRAINT Type_Task;

-- tables
DROP TABLE Assignment;

DROP TABLE Attachment;

DROP TABLE Comment;

DROP TABLE Participation;

DROP TABLE Project;

DROP TABLE ProjectStatus;

DROP TABLE Role;

DROP TABLE Tag;

DROP TABLE Task;

DROP TABLE TaskStatus;

DROP TABLE TaskTag;

DROP TABLE "User";

-- End of file.

