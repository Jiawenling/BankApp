using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AwesomeGICBank.Models
{
    public class Transactions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string AccountId { get; set; }
        public DateTime Date { get; set; }
        public string TxnId { get; set; }
        public double Amount { get; set; }
        public char TransactionType { get; set; }
        public double Balance { get; set; }
    }
}