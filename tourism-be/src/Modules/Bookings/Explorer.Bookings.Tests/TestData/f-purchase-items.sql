-- Insert test purchase items
INSERT INTO bookings."PurchaseItems"(
    "Id", "TourId", "TourName", "TourPrice", "TourStartDate", "Quantity", "TourPurchaseId")
VALUES (-1, -2, 'Art Gallery Tour', 35.00, '2025-11-15 14:00:00 UTC', 1, -1);

INSERT INTO bookings."PurchaseItems"(
    "Id", "TourId", "TourName", "TourPrice", "TourStartDate", "Quantity", "TourPurchaseId")
VALUES (-2, -3, 'Mountain Hike', 45.75, '2025-10-20 08:00:00 UTC', 1, -1);

INSERT INTO bookings."PurchaseItems"(
    "Id", "TourId", "TourName", "TourPrice", "TourStartDate", "Quantity", "TourPurchaseId")
VALUES (-3, -2, 'Art Gallery Tour', 35.00, '2025-11-15 14:00:00 UTC', 1, -2);