namespace TransactAPI.Models;


#pragma warning disable CS8618 // Data MOdel Only
public class TrasnactionDataModel
{

    public string ID { get; set; }
    public string Description { get; set; }
    public double PurchaseTotal { get; set; }
    public DateTime PurchaseDate { get; set; }

    /// <summary>
    /// Currentcy used in the transaction
    /// </summary><remarks>IE: Mexico-Peso</remarks>
    public string Currency { get; set; }



}

#pragma warning restore CS8618 // Data MOdel Only


