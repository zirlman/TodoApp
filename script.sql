create schema if not exists prointer_db;
use prointer_db;

CREATE TABLE IF NOT EXISTS user (
    userId INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(255),
    firstName VARCHAR(255),
    lastName VARCHAR(255),
    phoneNumber VARCHAR(255),
    eMail VARCHAR(255),
    password VARCHAR(511),
    passwordSalt VARCHAR(255)
)  ENGINE=INNODB DEFAULT CHARSET=UTF8MB4 COLLATE = UTF8MB4_UNICODE_CI;


CREATE TABLE IF NOT EXISTS taskItem (
    taskItemId INT AUTO_INCREMENT PRIMARY KEY,
    startDate TIMESTAMP NOT NULL,
    endDate TIMESTAMP NOT NULL,
    status VARCHAR(255) NOT NULL,
    priority TINYINT NOT NULL,
    notes TEXT NOT NULL,
    comment TEXT,
    userId INT NOT NULL,
    FOREIGN KEY (userId)
        REFERENCES user (userId)
        ON DELETE CASCADE
)  ENGINE=INNODB DEFAULT CHARSET=UTF8MB4 COLLATE = UTF8MB4_UNICODE_CI;