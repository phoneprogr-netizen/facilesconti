namespace FacileSconti.Web.Areas.EndUser.ViewModels;

public class EndUserProfileViewModel
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime RegisteredAt { get; set; }
}

public class EndUserMyCouponsViewModel
{
    public List<EndUserCouponItemViewModel> Coupons { get; set; } = [];
}

public class EndUserCouponItemViewModel
{
    public string Title { get; set; } = string.Empty;
    public DateTime DownloadedAt { get; set; }
    public bool IsRedeemed { get; set; }
    public string Code { get; set; } = string.Empty;
}

public class EndUserNewsletterViewModel
{
    public List<NewsletterItemViewModel> Subscriptions { get; set; } = [];
}

public class NewsletterItemViewModel
{
    public string Email { get; set; } = string.Empty;
    public bool IsConfirmed { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
