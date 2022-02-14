using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JB.Infrastructure.Constants
{
    public enum ErrorCode
    {
        #region Common 0 - 999
        [Description("Success")]
        Success = 0,

        [Description("Data is invalid")]
        InvalidData = 1,

        [Description("Invalid argument")]
        InvalidArgument = 2,

        [Description("Server error")]
        ServerError = 3,

        [Description("Configuration not found")]
        ConfigurationNotFound = 4,

        [Description("Invalid filter")]
        InvalidFilter = 5,

        [Description("Invalid sort")]
        InvalidSort = 6,

        [Description("No privilege on this item")]
        NoPrivilege = 7,

        [Description("Unauthorized")]
        Unauthorized = 8,

        [Description("Unauthenticated")]
        Unauthenticated = 9,
        #endregion

        #region User service 1000 - 1999
        [Description("Invalid password")]
        InvalidPassword = 1000,

        [Description("Email not confirmed")]
        EmailNotConfirmed = 1001,

        [Description("Email already confirmed")]
        EmailAlreadyConfirmed = 1002,

        [Description("Invalid verify code")]
        InvalidVerifyCode = 1003,

        [Description("User not exist")]
        UserNotExist = 1004,

        [Description("Email already registered")]
        EmailAlreadyRegistered = 1005,

        [Description("Account locked")]
        AccountLocked = 1006,

        [Description("Account not locked")]
        AccountNotLocked = 1007,

        [Description("Account can not be locked")]
        AccountCanNotBeLocked = 1008,

        [Description("Cannot get user info Grpc")]

        CannotGetUserInfoGrpc = 1010,

        [Description("User is not employee")]
        UserIsNotEmployee = 1020,

        [Description("User is not recruiter")]
        UserIsNotRecruiter = 1021,

        [Description("User is already employee")]
        UserIsEmployee = 1022,

        [Description("User is already recruiter")]
        UserIsRecruiter = 1023,
        #endregion

        #region Send email 2000 - 2999
        [Description("Unable to connect to email host")]

        UnableToConnectToEmailHost = 2000,

        [Description("Invalid email credential")]

        InvalidEmailCredential = 2001,
        #endregion

        #region Organization 3000-3999
        [Description("Organization not found")]
        OrganizationNull = 3000,
        #endregion

        #region Other 1000
        [Description("Unknown")]
        Unknown = int.MaxValue,
        #endregion

        #region cv service 5000 - 5999
        [Description("CV not found")]
        cvNull = 5000,

        [Description("Maximum CV number for account")]
        cvMax = 5001,

        #endregion

        #region interview service 6000 - 6999
        [Description("Interview not found")]
        InterviewNull = 6000,
        #endregion

        #region job service 7000 - 7999
        [Description("Job not found")]
        JobNull = 7000,

        [Description("Job expired")]
        JobExpired = 7001,

        [Description("Job already applied")]
        JobAlreadyApplied = 7002,

        [Description("Job not applied")]
        JobNotApplied = 7003,

        [Description("Job already interested")]
        JobAlreadyInterested = 7004,

        [Description("Job not interested")]
        JobNotInterested = 7005,
        #endregion

        #region customer service 8000 - 9999
        [Description("Report not found")]
        ReportNull = 8000,

        [Description("Review not found")]
        ReviewNull = 9000,
        #endregion

        #region payment service 10000 - 10999
        [Description("Payment not exist")]
        PaymentNull = 10001,
        #endregion

        #region blog service 11000 - 11999
        [Description("Blog not exist")]
        BlogNotExist = 11001,

        [Description("Blog comment already liked")]
        BlogCommentAlreadyLiked = 11002,

        [Description("Blog comment not liked")]
        BlogCommentNotLiked = 11003,

        [Description("Blog already liked")]
        BlogAlreadyLiked = 11004,

        [Description("Blog not liked")]
        BlogNotLiked = 11005,
        #endregion

        #region review service
        [Description("Review already exist")]
        ReviewAlreadyExist = 12001,
        #endregion
    }
}
