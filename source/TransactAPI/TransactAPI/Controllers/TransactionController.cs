using Microsoft.AspNetCore.Mvc;
using TransactAPI.Models;
using TransactAPI.Services;

namespace TransactAPI.Controllers;


[ApiController]
[Route("tran")]
public class TransactionController : Controller
{
    [HttpGet]
    public async Task<IEnumerable<TransactionDataModel>> Get(DateOnly? startDate, DateOnly? endDate = null)
    {

        DateOnly EndDate = (endDate ?? DateOnly.FromDateTime(DateTime.Now));
        if (startDate == null)
            throw new BadHttpRequestException(
                    "Start Date Missing",
                    StatusCodes.Status400BadRequest
            );


        if (startDate > EndDate)
        {
            throw new BadHttpRequestException(
                    "Start Date Must be earlier than End Date",
                    StatusCodes.Status400BadRequest
            );
        }
        var conn = MariaDBConn.GetConnection("127.0.0.1", 3306, "transact", "dbuser", "dbpassword");

        var results = await conn.GetTransactions((DateOnly)startDate!, EndDate);

        return results.ToArray();
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] TransactionDataModel transaction)
    {
        if (transaction == null)
        {
            return BadRequest("Transaction data is required");
        }

        await using var conn = MariaDBConn.GetConnection("127.0.0.1", 3306, "transact", "dbuser", "dbpassword");

        var resp = await conn.SaveTransactions(transaction);

        return resp.Code switch
        {
            0 => Ok(new { resp.Code, resp.Message }),
            1 => Conflict(new { resp.Code, resp.Message }),
            2 => BadRequest(new { resp.Code, resp.Message }),
            _ => StatusCode(500, new { resp.Code, resp.Message })
        };
    }




}
