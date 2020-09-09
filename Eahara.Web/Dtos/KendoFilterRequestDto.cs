using Kendo.DynamicLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eahara.Web.Dtos
{
    public class KendoFilterRequestDto : DataSourceRequest
    {

        // for ledger  report
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool IsFilter { get; set; }
        public bool IsAllClient { get; set; }
        public bool IsBlacklist { get; set; }
        public ICollection<long> Accounts { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

        // for rentel report
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public string FlatType { get; set; }

        // for pdc Status

        public long PDCId { get; set; }
        public string ChequeNo { get; set; }
        public string Status { get; set; }
        public DateTime ChequeDate { get; set; }
        public long BankId { get; set; }
        public int Pagenation { get; set; }
        public float FineAmount { get; set; }
        public long FineIncomeAccountId { get; set; }
        public long FineVATAccountId { get; set; }

        public ICollection<long> Shops { get; set; }

        // vacant units
        public long ApartmetId { get; set; }
        public string Floor { get; set; }
        public long Days { get; set; }

        // for manager
        public string ContractPage { get; set; }
        public string FlatPage { get; set; }
        public string Keyword { get; set; }
        public string DocNo { get; set; }
        public long TenantId { get; set; }

        // for customer statement

        public long ApartmentId { get; set; }
        public long FlatId { get; set; }

        public long ShopId { get; set; }
        public long MEDShopId { get; set; }
        public long UserId { get; set; }
        public string PageName { get; set; }
        public string ButtonName { get; set; }
        public string Paid { get; set; }


        // for registration

        public string MacId { get; set; }
        public string SystemName { get; set; }
        public string Key { get; set; }
        public string RPTName { get; set; }


        // for Property statement

        public long PropertyTypeId { get; set; }
        public long CategoryId { get; set; }
        public string Location { get; set; }
        public float MinArea { get; set; }
        public float MaxArea { get; set; }
        public int MinBedRoom { get; set; }
        public int MaxBedRoom { get; set; }
        public float MinBudget { get; set; }
        public float MaxBudget { get; set; }
        public long EmirateId { get; set; }
        public long StatusId { get; set; }
        public long LocationId { get; set; }
    }
}