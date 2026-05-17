
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
CREATE TABLE IF NOT EXISTS transact.transactions (
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

CREATE OR REPLACE PROCEDURE transact.GetAlltransactions(
	IN inStartDate	DATE,
    IN inEndDate	DATE
)
BEGIN
    SELECT
        ID,
        Description,
        PurchaseTotal,
        PurchaseDate,
        Currency
    FROM transact.transactions
		WHERE PurchaseDate >= inStartDate 
				AND PurchaseDate < DATE_ADD(inEndDate, INTERVAL 1 DAY) 
    ORDER BY PurchaseDate desc
    ;
END$$

CREATE OR REPLACE PROCEDURE transact.SaveTransaction(
	IN inID		VARCHAR(50),
    IN inDescription	VARCHAR(50),
    IN inPurchaseTotal	DOUBLE(15,2),
    IN inPurchaseDate	DATETIME,
    IN inCurrency		VARCHAR(20),
	OUT outCode			INT,
    OUT outMessage		VARCHAR(50)
)
BEGIN

	DECLARE rowCount INT;
    SET rowCount = 0;
    
	SELECT -1, 'An unhandled exception occurred' 
		INTO outCode, outMessage;
    
    mainLogic: BEGIN
		-- -------------------------------------------------------------
        -- Validate Transaction ID
		-- -------------------------------------------------------------
		IF EXISTS (SELECT 1 FROM transact.transactions WHERE ID = inID) THEN
			SELECT 1, 'Duplicate Transaction ID Rejected' 
				INTO outCode, outMessage;
			LEAVE mainLogic;
        END IF;
        
		-- -------------------------------------------------------------
        -- Validate Purchase Amount
		-- -------------------------------------------------------------
        IF (inPurchaseTotal < 0) THEN
			SELECT 2,'Purchase amount cannot be a negative number'
				INTO outCode, outMessage;
			LEAVE mainLogic;
        END IF;
        
		-- -------------------------------------------------------------
        -- Validate Currency Provided
		-- -------------------------------------------------------------
        IF (inCurrency IS NULL OR LENGTH(TRIM(inCurrency)) < 1) THEN
			SELECT 2,'Please provide a currency.'
				INTO outCode, outMessage;
			LEAVE mainLogic;
        END IF;
				
                
		-- -------------------------------------------------------------
        -- Start Transaction
		-- -------------------------------------------------------------
        
        INSERT INTO transact.transactions (
			ID,
			Description,
			PurchaseTotal,
			PurchaseDate,
			Currency
        )
        VALUES(
			inID,
            inDescription,
            inPurchaseTotal,
            inPurchaseDate,
            inCurrency
        );
        
        SELECT ROW_COUNT() INTO rowCount;
        IF (rowCount = 1) THEN
			SELECT 0,'Transaction Saved'
				INTO outCode, outMessage;
		ELSE
			SELECT -2,'Error in Commit : Unknown Exception'
				INTO outCode, outMessage;
        
        END IF;
	
    END mainLogic;
    
END$$





DELIMITER ;