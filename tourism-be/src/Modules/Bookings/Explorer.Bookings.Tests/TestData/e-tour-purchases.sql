-- Insert test tour purchases
INSERT INTO bookings."TourPurchases"(
    "Id", "TouristId", "TotalPrice", "BonusPointsUsed", "FinalPrice", "PurchaseDate", "Status")
VALUES (-1, -21, 80.75, 10.00, 70.75, '2024-01-01 15:00:00 UTC', 0);

INSERT INTO bookings."TourPurchases"(
    "Id", "TouristId", "TotalPrice", "BonusPointsUsed", "FinalPrice", "PurchaseDate", "Status")
VALUES (-2, -22, 35.00, 0.00, 35.00, '2024-01-02 16:00:00 UTC', 0);