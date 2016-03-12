using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JabbR.ViewModels
{
    public enum Order
    {
        Id,
        Name,
        Email,
        Description
    }
    public enum Filters
    {
        All,
        Operators,
        Active
    }
    public enum BulkAction
    {
        None,
        Add,
        Delete,
        Disable,
        Ban,
        ChallengeEmail
    }
    public class IndexOptions
    {
        public string Search { get; set; }
        public Order Order { get; set; }
        public Filters Filter { get; set; }
        public BulkAction BulkAction { get; set; }
    }
}