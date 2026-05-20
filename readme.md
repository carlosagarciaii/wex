# Transaction API

## Description

.Net Core API that receives transactions, stores them in a MariaDB, and facilitates the retrieval of entries. 

## Endpoints

 * GET /tran/help
 * GET /tran/coffee
 * GET /tran
 * POST /tran


### GET /tran/help
Provides basic usage examples.

### GET /tran/coffee

My favorite HTTP response code deserves its own endpoint. Enjoy the HTTP 418 "I'm a teapot" status.

*Response*: "No Coffee Here, only Tea." with status code 418.

### GET /tran
Retrieves data based on date range.

#### Query Parameters:

|Parameter|Required|Details|
|---|---|---|
|startDate|Y|Starting date in YYYY-MM-DD format|
|endDate|N|Ending date in YYYY-MM-DD format (defaults to current date)|

#### Request Example:

** startDate Only **
GET `<url>/tran?startDate=2026-01-01`

** startDate & endDate **
GET `<url>/tran?startDate=2026-01-01&endDate=2027-01-01`


#### Response Details:

|Field|Description|
|---|---|
|id|Unique identifier for the transaction|
|description|Description for the Transaction|
|usdPurchaseTotal|The total amount paid converted to USD|
|purchaseTotal|The original purchase amount in the original currency|
|purchaseDate|The date and time of the original purchase|
|currency|The currency for the original purchase|


#### Response Example:

```json

[{
	"id": "03a38048-98b5-40f9-939a-4fea5511c4bf",
	"description": "Tick Tock Clock",
	"usdPurchaseTotal": 2.37,
	"purchaseTotal": 3.25,
	"purchaseDate": "2026-03-21T19:21:49",
	"currency": "Canada-Dollar"
}]

```

#### Validation:
startDate must be earlier than endDate
Returns HTTP 400 if validation fails

*Response*: Array of TransactionDataModel objects.

### POST /tran
Submits a new transaction with automatic currency conversion to USD.

*Request Body*: TransactionDataModel with the following properties:
	
|Parameter|Data-Type|Description|
|---|---|---|
|ID|String/Guid|A Unique Identifier for the transaction|
|Description|String (50 char)|A description for the transaction|
|PurchaseDate|DateOnly|Date of the transaction|
|PurchaseTotal|double|The purpose amount in the original currency|
|Currency|string|Original currency the transaction was conducted in|

#### Example Request Body

Content-Type: application/json

```json

{
	"ID":"95124905-6334-4474-adca-85b02cdeb2a7",
	"Description": "My Item Description",
	"Currency": "Japan-Yen",
	"PurchaseDate": "2026-05-19",
	"PurchaseTotal": 100.00
}

```


*Process*:

1. Validates required transaction data
1. Fetches exchange rate for the specified currency on the purchase date
1. Calculates USDPurchaseTotal by dividing PurchaseTotal by the exchange rate
1. Saves transaction to database

*SQL/HTML Response Codes*:

|HTML Code|SQL Code|Description|
|---|---|---|
|200|0|Success|
|409|1|Conflict/Duplicate transaction|
|400|2,3,4,5|Bad Request / Various validation errors|
|500|`other`|Internal Server Error|

## Database Connection

The API connects to a MariaDB database using the following configuration:

|Setting|Value|
|---|---|
|Host|127.0.0.1|
|Port|3306|
|Database|transact|
|Username|dbuser|
|Password|dbpassword|


## Building

From the repo root, run `./buildProject.sh`


## Error Handling

    Missing or invalid parameters return HTTP 400 with descriptive messages
    Database errors return HTTP 500
    Currency conversion failures return HTTP 400

## Dependencies

    .NET Core
    MariaDB database
    External currency exchange rate service