﻿using System;
using Module.MusicBee.Extension.LibraryQuerying.Entities;
using Module.MusicBee.Extension.LibraryQuerying.Enums;

namespace Module.MusicBee.Extension.LibraryQuerying.Helpers;

internal static class RepresentationHelper
{
    public static string RepresentString(string value)
    {
        return value;
    }

    public static string RepresentInt(int value)
    {
        return value.ToString();
    }

    public static string RepresentDateTime(DateTime dateTime)
    {
        throw new NotImplementedException();
    }

    public static string RepresentTimeOffset(TimeOffset timeOffset)
    {
        throw new NotImplementedException();
    }

    public static string RepresentRating(Rating rating)
    {
        throw new NotImplementedException();
    }
}