
-- ----------------------------------------------------
-- DATABASE
-- ----------------------------------------------------
CREATE DATABASE IF NOT EXISTS transact;

USE transact;
-- ----------------------------------------------------
-- USERS
-- ----------------------------------------------------
GRANT ALL PRIVILEGES ON transact.* TO 'dbuser'@'127.0.0.1' IDENTIFIED BY 'dbpassword';
FLUSH PRIVILEGES;

-- ----------------------------------------------------
-- TABLES
-- ----------------------------------------------------
CREATE TABLE IF NOT EXISTS transactions (
    ID            VARCHAR(50) PRIMARY KEY,
    Description   VARCHAR(50) NOT NULL,
    PurchaseTotal DOUBLE(15,2) NOT NULL,
    PurchaseDate  DATETIME NOT NULL DEFAULT(UTC_TIMESTAMP),
    Currency      VARCHAR(20) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;


-- ----------------------------------------------------
-- STORED PROCEDURES
-- ----------------------------------------------------

DELIMITER $$

CREATE PROCEDURE GetAllTransactions()
BEGIN
    SELECT
        ID,
        Description,
        PurchaseTotal,
        PurchaseDate,
        Currency
    FROM transactions
    ORDER BY PurchaseDate desc
    ;
END$$

CREATE PROCEDURE SaveTransaction(
	IN inID		VARCHAR(50),
    IN inDescription	VARCHAR(50),
    IN inPurchaseTotal	DOUBLE(15,2),
    IN inPurchaseDate	DATETIME,
    IN inCurrency		VARCHAR(20),
	OUT outCode			INT
)
BEGIN

	


END$$





DELIMITER ;