using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiloTaxi.Common.Enums
{
    public enum GenderType
    {
        Male,
        Female,
        Undefined,
    }

    public enum OtpStatus
    {
        Pending,
        Success,
        Expired,
    }

    public enum OptType
    {
        AccountOpen,
        ResetPassword,
        AccountCancellation,
    }

    public enum WalletStatus
    {
        Active,
        Disable,
    }

    public enum PaymentMethod
    {
        Wallet,
        BankAccount,
        MupCard,
        VisaMaster,
    }

    public enum PaymentType
    {
        Cash,
        Wallet,
        OnlinePayment,
    }

    public enum ScheduleOrderStatus
    {
        Active,
        Approved,
        Cancel,
    }

    public enum CustomerStatus
    {
        Pending,
        Active,
        Deactivate,
        Suspended,
    }

    public enum NotificationType
    {
        System,
        Alert,
        Promotion,
        Announcement,
        Other,
    }

    public enum SmsStatus
    {
        Pending,
        Delivered,
        Fail,
    }

    public enum WalletType
    {
        Customer,
        Driver,
        VipCustomer,
    }

    public enum GeneralStatus
    {
        Active,
        Disable,
        Deleted,
    }

    public enum KiloType
    {
        Normal,
        KiloPlus,
    }

    public enum PromotionStatus
    {
        Expired,
        Used,
        Active,
        Reject,
    }

    public enum KycStatus
    {
        Pending,
        FullKyc,
        Reject,
    }

    public enum NotificationStatus
    {
        Delivered,
        Read,
        Fail,
    }

    public enum DriverStatus
    {
        Pending,
        Active,
        Deactivate,
        Suspend,
    }

    public enum VehicleStatus
    {
        Active,
        Suspend,
    }

    public enum TopUpTransactionStatus
    {
        Pending,
        Success,
        Reject,
    }

    public enum OrderStatus
    {
        Completed,
        Cancelled,
        InProgress,
    }

    public enum PromotionType
    {
        FixAmount,
        Percentage,
    }

    public enum TransactionType
    {
        TopUp,
        Order,
        PromotionUsage,
    }

    public enum ScheduleStatus
    {
        Pending,
        Cancelled,
        Completed,
    }

    public enum ApplicableTo
    {
        Customer,
        Driver,
        Both,
    }
}
