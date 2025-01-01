﻿using AutoMapper;
using Booking.Database.Entities;
using Booking.App.DTOs;
using System;

namespace Booking.App.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mappa User till UserDto och omvänt
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ReverseMap();

            // Mappa Concert till ConcertDto och omvänt
            CreateMap<Concert, ConcertDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ReverseMap();

            // Mappa Performance till PerformanceDto och omvänt
            CreateMap<Performance, PerformanceDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.DateTime, opt => opt.MapFrom(src => src.DateTime))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
                .ForMember(dest => dest.ConcertId, opt => opt.MapFrom(src => src.ConcertId))
                .ReverseMap();

            // Mappa Bokning till BookingDto och omvänt
            CreateMap<Bokning, BookingDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PerformanceId, opt => opt.MapFrom(src => src.PerformanceId))
                .ForMember(dest => dest.BookingDate, opt => opt.MapFrom(src => src.BookingDate))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.CustomerName))
                .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.CustomerEmail))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId)) // Lägg till UserId
                .ReverseMap();

            // Mappa BookPerformanceDto till Bokning
            CreateMap<BookPerformanceDto, Bokning>()
                .ForMember(dest => dest.PerformanceId, opt => opt.MapFrom(src => src.PerformanceId))
                .ForMember(dest => dest.BookingDate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Performance, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore()) // Ignorera UserId för denna DTO
                .ReverseMap();

            CreateMap<BookPerformanceDto, BookingDto>();
        }
    }
}
