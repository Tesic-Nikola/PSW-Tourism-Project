﻿namespace Explorer.Tours.API.Dtos;

public class KeyPointDto
{
    public long Id { get; set; }
    public long TourId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string ImageUrl { get; set; }
    public int Order { get; set; }
}