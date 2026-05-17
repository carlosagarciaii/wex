using Microsoft.AspNetCore.Mvc;
using TransactAPI.Models;
using TransactAPI.Services;

namespace TransactAPI.Controllers;


[ApiController]
[Route("tran")]
public class TransactionController : Controller
{

    [HttpGet]
    public async Task<IEnumerable<TrasnactionDataModel>> Get(DateOnly startDate, DateOnly? endDate = null)
    {
        if (startDate > endDate)
        {
            throw new BadHttpRequestException(
                    "Start Date Must be earlier than End Date",
                    StatusCodes.Status400BadRequest
            );
        }
        var conn = MariaDBConn.GetConnection("127.0.0.1", 3306, "transact", "dbuser", "password");

        var results = await conn.GetTransactions(startDate, endDate);

        return results.ToArray();
    }


}
