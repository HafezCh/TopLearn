using System;

namespace TopLearn.Core.DTOs
{
    public class WalletViewModel
    {
        public int Amount { get; set; }
        public int Type { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
    }

    public class ChargeWalletViewModel
    {
        public int Amount { get; set; }
    }
}