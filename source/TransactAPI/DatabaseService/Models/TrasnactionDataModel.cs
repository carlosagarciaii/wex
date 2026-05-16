using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace DatabaseService.Models;

public class TrasnactionDataModel
{
#pragma warning disable CS8618 // Data MOdel Only

    [Column("ID")]
    [JsonPropertyName("ID")]
    public Guid ID { get; set; }    
    
    [Column("description")]
    [JsonPropertyName("Description")]
    public string Description { get; set; }

    [Column("createdOn")]
    [JsonPropertyName("TransactionDate")]
    public DateTime TransactionDate { get; set; }

    [Column("purchaseTotal")]
    [JsonPropertyName("PurchaseTotal")]
    public double PurchaseTotal { get; set; }

    [Column("currency")]
    [JsonPropertyName("Currency")]
    public string CurrencyType { get; set; }




#pragma warning restore CS8618
}


/*
* 
* Field requirements
● Description: must not exceed 50 characters
● Transaction date: must be a valid date format
● Purchase amount: must be a valid positive amount rounded to the nearest cent
● Unique identifier: must uniquely identify the purchase

*/