using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using JB.Infrastructure.DTOs.Subscriptions;
using JB.Notification.DTOs.Notification;
using JB.Notification.DTOs.Organization;
using JB.Notification.DTOs.User;
using JB.Notification.Models.Notification;
using JB.Notification.Models.Organization;
using JB.Notification.Models.User;
using System;

namespace JB.Notification.AutoMapper
{
    public class NotificationMapperProfile : Profile
    {
        public NotificationMapperProfile()
        {
            CreateMap<NotificationModel, NotificationResponse>();
            CreateMap<UserModel, NotificationUserResponse>();
            CreateMap<OrganizationModel, NotificationOrganizationResponse>();

            CreateMap<Timestamp, DateTime>().ConvertUsing(x => x.ToDateTime());
            CreateMap<JB.gRPC.User.User, UserModel>();
            CreateMap<JB.gRPC.Organization.Organization, OrganizationModel>();

            CreateMap<NotificationModel, SubscriptionsNotificationResponse>();
            CreateMap<UserModel, SubscriptionsUserResponse>();
            CreateMap<OrganizationModel, SubscriptionsOrganizationResponse>();
        }
    }
}
