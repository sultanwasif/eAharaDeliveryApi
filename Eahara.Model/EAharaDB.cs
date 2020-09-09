namespace Eahara.Model
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class EAharaDB : DbContext
    {
        public EAharaDB()
            : base("name=EAharaDB")
        {
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserSession> UserSessions { get; set; }
        public virtual DbSet<Enquiry> Enquiries { get; set; }
        public virtual DbSet<Feedback> Feedbacks { get; set; }
        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<ShopCategory> ShopCategories { get; set; }
        public virtual DbSet<ItemCategory> ItemCategories { get; set; }
        public virtual DbSet<Shop> Shops { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<ShopInfo> ShopInfos { get; set; }
        public virtual DbSet<ShopMenu> ShopMenus { get; set; }
        public virtual DbSet<ShopImage> ShopImages { get; set; }
        public virtual DbSet<ItemImage> ItemImages { get; set; }
        public virtual DbSet<Review> Reviews{ get; set; }
        public virtual DbSet<Offer> Offers { get; set; }
        public virtual DbSet<Subscription> Subscriptions { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<TraceNo> TraceNoes { get; set; }
        public virtual DbSet<Partner> Partners { get; set; }
        public virtual DbSet<FAQ> FAQs { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<BookingDetails> BokkingDetailes { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<CompanyProfile> CompanyProfiles { get; set; }
        public virtual DbSet<PromoOffer> PromoOffers { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<UserOTP> UserOTPs { get; set; }
        public virtual DbSet<CustomerOffer> CustomerOffers { get; set; }
        public virtual DbSet<Status> Status { get; set; }
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<Report> Reports { get; set; }
        public virtual DbSet<MEDCategory> MEDCategories { get; set; }
        public virtual DbSet<MEDStatus> MEDStatuses { get; set; }
        public virtual DbSet<MEDSubCategory> MEDSubCategories { get; set; }
        public virtual DbSet<MEDItem> MEDItems { get; set; }
        public virtual DbSet<MEDOffer> MEDOffers { get; set; }
        public virtual DbSet<MEDUpload> MEDUploads { get; set; }
        public virtual DbSet<MEDBooking> MEDBookings { get; set; }
        public virtual DbSet<MEDBookingDetail> MEDBookingDetails { get; set; }
        public virtual DbSet<MEDBrand> MEDBrands { get; set; }
        public virtual DbSet<MEDShop> MEDShops { get; set; }
        public virtual DbSet<CompanyExpense> CompanyExpenses { get; set; }
        public virtual DbSet<CompanyExpenseDetails> CompanyExpenseDetails { get; set; }
        public virtual DbSet<PaymentMode> PaymentModes { get; set; }
        public virtual DbSet<Expense> Expenses { get; set; }
        public virtual DbSet<MMethod> MMethods { get; set; }
        public virtual DbSet<CustomerMMethod> CustomerMMethods { get; set; }
        public virtual DbSet<DateReport> DateReports { get; set; }
    }

}