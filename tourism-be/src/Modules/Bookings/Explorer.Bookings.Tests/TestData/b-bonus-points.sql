-- Insert test bonus points for tourists
INSERT INTO bookings."BonusPoints"(
    "Id", "TouristId", "AvailablePoints", "LastUpdated")
VALUES (-1, -21, 100.00, '2024-01-01 10:00:00 UTC');

INSERT INTO bookings."BonusPoints"(
    "Id", "TouristId", "AvailablePoints", "LastUpdated")
VALUES (-2, -22, 50.50, '2024-01-15 12:00:00 UTC');

INSERT INTO bookings."BonusPoints"(
    "Id", "TouristId", "AvailablePoints", "LastUpdated")
VALUES (-3, -23, 0.00, '2024-02-01 09:00:00 UTC');