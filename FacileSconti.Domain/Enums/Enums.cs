namespace FacileSconti.Domain.Enums;

public enum ContractStatus { Draft = 1, Active = 2, Suspended = 3, Expired = 4, Terminated = 5 }
public enum CouponStatus { Draft = 1, Active = 2, Suspended = 3, Expired = 4, Exhausted = 5 }
public enum CouponType { Free = 1, PartialPaid = 2, Paid = 3 }
public enum PaymentStatus { Pending = 1, Paid = 2, Failed = 3, Refunded = 4 }
public enum PaymentMethod { ManualBankTransfer = 1, Cash = 2, Pos = 3, OnlineGateway = 4 }
public enum BoostStatus { Requested = 1, Active = 2, Completed = 3, Rejected = 4 }
public enum UserType { Admin = 1, Customer = 2, EndUser = 3 }
