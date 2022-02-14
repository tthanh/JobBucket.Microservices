﻿using AutoMapper;
using JB.Notification.Models.User;
using JB.Notification.DTOs.Chat;
using JB.Notification.Models.Chat;
using JB.Notification.DTOs.User;

namespace JB.Notification.AutoMapper
{
    public class ChatMapperProfile : Profile
    {
        public ChatMapperProfile()
        {
            CreateMap<int?, int>().ConvertUsing((src, dest) => { if (src.HasValue) return src.Value; return dest; });

            CreateMap<AddMessageRequest, ChatMessageModel>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UserModel, UserResponse>();
            CreateMap<ChatMessageModel, MessageResponse>();
            CreateMap<ChatConversationModel, ConversationResponse>();
        }
    }
}
