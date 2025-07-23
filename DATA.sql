-- [User] INSERT
INSERT INTO [User] (FullName, PhoneNumber, Email) VALUES (N'Nguyễn Văn An', N'0901234567', N'an.nguyen@gmail.com');
INSERT INTO [User] (FullName, PhoneNumber, Email) VALUES (N'Trần Thị Bình', N'0912345678', N'binh.tran@gmail.com');
INSERT INTO [User] (FullName, PhoneNumber, Email) VALUES (N'Lê Hoàng Cường', N'0923456789', N'cuong.le@gmail.com');
INSERT INTO [User] (FullName, PhoneNumber, Email) VALUES (N'Phạm Minh Duy', N'0934567890', N'duy.pham@gmail.com');
INSERT INTO [User] (FullName, PhoneNumber, Email) VALUES (N'Võ Thị Hạnh', N'0945678901', N'hanh.vo@gmail.com');
-- Role INSERT
INSERT INTO Role (RoleName) VALUES (N'User');
INSERT INTO Role (RoleName) VALUES (N'Staff');
INSERT INTO Role (RoleName) VALUES (N'Manager');
INSERT INTO Role (RoleName) VALUES (N'Admin');
-- Account INSERT
INSERT INTO Account (PhoneNumber, PasswordHash, RegistDate, VerificationToken, IsVerified, RoleId, UserId) VALUES (N'0901234567', N'1', '2025-06-25 08:10:03', N'vfy123an', 1, 1, 1);
INSERT INTO Account (PhoneNumber, PasswordHash, RegistDate, VerificationToken, IsVerified, RoleId, UserId) VALUES (N'0912345678', N'2', '2025-06-25 08:12:03', N'vfy123binh', 1, 2, 2);
INSERT INTO Account (PhoneNumber, PasswordHash, RegistDate, VerificationToken, IsVerified, RoleId, UserId) VALUES (N'0923456789', N'3', '2025-06-25 08:15:03', N'vfy123cuong', 0, 3, 3);
INSERT INTO Account (PhoneNumber, PasswordHash, RegistDate, VerificationToken, IsVerified, RoleId, UserId) VALUES (N'0934567890', N'4', '2025-06-25 08:20:03', N'vfy123duy', 1, 4, 4);
INSERT INTO Account (PhoneNumber, PasswordHash, RegistDate, VerificationToken, IsVerified, RoleId, UserId) VALUES (N'0945678901', N'5', '2025-06-25 08:25:03', N'vfy123hanh', 0, 5, 5);
-- PasswordResetToken INSERT
INSERT INTO PasswordResetToken (AccountId, Token, ExpiryDate) VALUES (1, N'resetan2025', '2025-06-26 09:00:00');
INSERT INTO PasswordResetToken (AccountId, Token, ExpiryDate) VALUES (2, N'resetbinh2025', '2025-06-26 09:05:00');
INSERT INTO PasswordResetToken (AccountId, Token, ExpiryDate) VALUES (3, N'resetcuong2025', '2025-06-26 09:10:00');
INSERT INTO PasswordResetToken (AccountId, Token, ExpiryDate) VALUES (4, N'resetduy2025', '2025-06-26 09:15:00');
INSERT INTO PasswordResetToken (AccountId, Token, ExpiryDate) VALUES (5, N'resethanh2025', '2025-06-26 09:20:00');
-- Drink INSERT
INSERT INTO Drink (Name, Description, Price, IsAvailable) VALUES (N'Trà sữa truyền thống', N'Trà đen pha với sữa tươi, topping trân châu', 3.50, 1);
INSERT INTO Drink (Name, Description, Price, IsAvailable) VALUES (N'Cà phê sữa đá', N'Cà phê rang xay pha phin với sữa đặc', 2.80, 1);
INSERT INTO Drink (Name, Description, Price, IsAvailable) VALUES (N'Nước cam tươi', N'Nước cam nguyên chất ép lạnh', 3.20, 1);
INSERT INTO Drink (Name, Description, Price, IsAvailable) VALUES (N'Matcha Latte', N'Trà xanh Nhật Bản pha với sữa tươi', 4.00, 1);
INSERT INTO Drink (Name, Description, Price, IsAvailable) VALUES (N'Capuchino', N'Cà phê Ý với lớp bọt sữa mịn', 4.50, 1);
INSERT INTO Drink (Name, Description, Price, IsAvailable) VALUES (N'Hồng trà chanh', N'Trà đen pha với chanh tươi và đường', 2.50, 1);
INSERT INTO Drink (Name, Description, Price, IsAvailable) VALUES (N'Sinh tố bơ', N'Sinh tố bơ tươi, sữa đặc và đá xay', 3.80, 1);
INSERT INTO Drink (Name, Description, Price, IsAvailable) VALUES (N'Mocha đá xay', N'Cà phê espresso, chocolate và kem tươi', 4.70, 1);
INSERT INTO Drink (Name, Description, Price, IsAvailable) VALUES (N'Trà đào cam sả', N'Trà hoa quả mát lạnh với đào ngâm, cam lát và sả', 3.60, 1);
INSERT INTO Drink (Name, Description, Price, IsAvailable) VALUES (N'Sữa tươi trân châu đường đen', N'Sữa tươi kèm trân châu nấu đường đen đậm đà', 4.20, 1);
INSERT INTO Drink (Name, Description, Price, IsAvailable) VALUES (N'Cà phê đen nóng', N'Cà phê nguyên chất pha phin truyền thống', 2.00, 1);
INSERT INTO Drink (Name, Description, Price, IsAvailable) VALUES (N'Lemon tea', N'Trà đen pha với nước cốt chanh và mật ong', 2.70, 1);
INSERT INTO Drink (Name, Description, Price, IsAvailable) VALUES (N'Milo dằm', N'Milo pha sữa và đá xay, topping bánh Oreo', 3.90, 1);
INSERT INTO Drink (Name, Description, Price, IsAvailable) VALUES (N'Trà vải', N'Trà xanh ủ lạnh kết hợp vải tươi', 3.30, 1);
INSERT INTO Drink (Name, Description, Price, IsAvailable) VALUES (N'Soda việt quất', N'Nước soda pha siro việt quất mát lạnh', 3.00, 1);
INSERT INTO Drink (Name, Description, Price, IsAvailable) VALUES (N'Trà sen vàng', N'Trà nhài thơm kết hợp hạt sen và kem cheese', 3.80, 1);
INSERT INTO Drink (Name, Description, Price, IsAvailable) VALUES (N'Cà phê caramel', N'Cà phê espresso pha với caramel và sữa tươi', 4.50, 1);
INSERT INTO Drink (Name, Description, Price, IsAvailable) VALUES (N'Sinh tố xoài', N'Xoài chín xay nhuyễn với sữa đặc và đá', 3.70, 1);
INSERT INTO Drink (Name, Description, Price, IsAvailable) VALUES (N'Trà nhài mật ong', N'Trà hoa nhài ủ lạnh với mật ong nguyên chất', 3.10, 1);
INSERT INTO Drink (Name, Description, Price, IsAvailable) VALUES (N'Matcha macchiato', N'Matcha đậm vị phủ kem macchiato béo mịn', 4.60, 1);
INSERT INTO Drink (Name, Description, Price, IsAvailable) VALUES (N'Soda chanh dây', N'Nước soda pha chanh dây và hạt chia', 3.00, 1);
INSERT INTO Drink (Name, Description, Price, IsAvailable) VALUES (N'Trà sữa khoai môn', N'Trà sữa béo kết hợp hương khoai môn và trân châu trắng', 3.90, 1);
INSERT INTO Drink (Name, Description, Price, IsAvailable) VALUES (N'Cà phê kem trứng', N'Cà phê nóng phủ kem trứng béo mịn', 4.80, 1);
INSERT INTO Drink (Name, Description, Price, IsAvailable) VALUES (N'Sữa chua việt quất', N'Sữa chua đá xay kết hợp mứt việt quất', 3.60, 1);
INSERT INTO Drink (Name, Description, Price, IsAvailable) VALUES (N'Milo kem cheese', N'Milo đá xay phủ lớp kem cheese mặn ngọt', 4.20, 1);

UPDATE Drink SET img = N'https://cafeholic-imgs.s3.ap-southeast-1.amazonaws.com/drinks/tra-sua-truyen-thong.jpg' WHERE Name = N'Trà sữa truyền thống';
UPDATE Drink SET img = N'https://cafeholic-imgs.s3.ap-southeast-1.amazonaws.com/drinks/ca-phe-sua-da.jpg' WHERE Name = N'Cà phê sữa đá';
UPDATE Drink SET img = N'https://cafeholic-imgs.s3.ap-southeast-1.amazonaws.com/drinks/nuoc-cam-tuoi.jpg' WHERE Name = N'Nước cam tươi';
UPDATE Drink SET img = N'https://cafeholic-imgs.s3.ap-southeast-1.amazonaws.com/drinks/matcha-latte.jpg' WHERE Name = N'Matcha Latte';
UPDATE Drink SET img = N'https://cafeholic-imgs.s3.ap-southeast-1.amazonaws.com/drinks/capuchino.jpg' WHERE Name = N'Capuchino';
UPDATE Drink SET img = N'https://cafeholic-imgs.s3.ap-southeast-1.amazonaws.com/drinks/hong-tra-chanh.jpg' WHERE Name = N'Hồng trà chanh';
UPDATE Drink SET img = N'https://cafeholic-imgs.s3.ap-southeast-1.amazonaws.com/drinks/sinh-to-bo.jpg' WHERE Name = N'Sinh tố bơ';
UPDATE Drink SET img = N'https://cafeholic-imgs.s3.ap-southeast-1.amazonaws.com/drinks/mocha-da-xay.jpg' WHERE Name = N'Mocha đá xay';
UPDATE Drink SET img = N'https://cafeholic-imgs.s3.ap-southeast-1.amazonaws.com/drinks/tra-dao-cam-sa.jpg' WHERE Name = N'Trà đào cam sả';
UPDATE Drink SET img = N'https://cafeholic-imgs.s3.ap-southeast-1.amazonaws.com/drinks/sua-tuoi-tran-chau-duong-den.jpg' WHERE Name = N'Sữa tươi trân châu đường đen';
UPDATE Drink SET img = N'https://cafeholic-imgs.s3.ap-southeast-1.amazonaws.com/drinks/ca-phe-en-nong.jpg' WHERE Name = N'Cà phê đen nóng';
UPDATE Drink SET img = N'https://cafeholic-imgs.s3.ap-southeast-1.amazonaws.com/drinks/lemon-tea.jpg' WHERE Name = N'Lemon tea';
UPDATE Drink SET img = N'https://cafeholic-imgs.s3.ap-southeast-1.amazonaws.com/drinks/milo-dam.jpg' WHERE Name = N'Milo dằm';
UPDATE Drink SET img = N'https://cafeholic-imgs.s3.ap-southeast-1.amazonaws.com/drinks/tra-vai.jpg' WHERE Name = N'Trà vải';
UPDATE Drink SET img = N'https://cafeholic-imgs.s3.ap-southeast-1.amazonaws.com/drinks/soda-viet-quat.jpg' WHERE Name = N'Soda việt quất';
UPDATE Drink SET img = N'https://cafeholic-imgs.s3.ap-southeast-1.amazonaws.com/drinks/soda-chanh-day.jpg' WHERE Name = N'Soda chanh dây';
UPDATE Drink SET img = N'https://cafeholic-imgs.s3.ap-southeast-1.amazonaws.com/drinks/tra-sen-vang.jpg' WHERE Name = N'Trà sen vàng';
UPDATE Drink SET img = N'https://cafeholic-imgs.s3.ap-southeast-1.amazonaws.com/drinks/ca-phe-caramel.jpeg' WHERE Name = N'Cà phê caramel';
UPDATE Drink SET img = N'https://cafeholic-imgs.s3.ap-southeast-1.amazonaws.com/drinks/sinh-to-xoai.jpg' WHERE Name = N'Sinh tố xoài';
UPDATE Drink SET img = N'https://cafeholic-imgs.s3.ap-southeast-1.amazonaws.com/drinks/tra-nhai-mat-ong.jpg' WHERE Name = N'Trà nhài mật ong';
UPDATE Drink SET img = N'https://cafeholic-imgs.s3.ap-southeast-1.amazonaws.com/drinks/matcha-macchiato.jpg' WHERE Name = N'Matcha macchiato';
UPDATE Drink SET img = N'https://cafeholic-imgs.s3.ap-southeast-1.amazonaws.com/drinks/tra-sua-khoai-mon.png' WHERE Name = N'Trà sữa khoai môn';
UPDATE Drink SET img = N'https://cafeholic-imgs.s3.ap-southeast-1.amazonaws.com/drinks/ca-phe-kem-trung.jpeg' WHERE Name = N'Cà phê kem trứng';
UPDATE Drink SET img = N'https://cafeholic-imgs.s3.ap-southeast-1.amazonaws.com/drinks/sua-chua-viet-quat.jpg' WHERE Name = N'Sữa chua việt quất';
UPDATE Drink SET img = N'https://cafeholic-imgs.s3.ap-southeast-1.amazonaws.com/drinks/milo-kem-cheese.jpg' WHERE Name = N'Milo kem cheese';

-- RoomType INSERT
INSERT INTO RoomType (Name, MinCapacity, MaxCapacity, Description) VALUES (N'Phòng đơn', 1, 2, N'Phòng nhỏ, yên tĩnh cho 1-2 người');
INSERT INTO RoomType (Name, MinCapacity, MaxCapacity, Description) VALUES (N'Phòng nhóm nhỏ', 2, 4, N'Phòng cho nhóm bạn học, làm việc');
INSERT INTO RoomType (Name, MinCapacity, MaxCapacity, Description) VALUES (N'Phòng nhóm lớn', 4, 8, N'Phòng họp nhóm, thuyết trình');
INSERT INTO RoomType (Name, MinCapacity, MaxCapacity, Description) VALUES (N'Phòng VIP', 1, 4, N'Phòng riêng tư, tiện nghi cao cấp');
INSERT INTO RoomType (Name, MinCapacity, MaxCapacity, Description) VALUES (N'Phòng sự kiện', 10, 20, N'Tổ chức sự kiện, workshop');
-- StudyRoom INSERT
INSERT INTO StudyRoom (Name, IsAvailable, RoomTypeId) VALUES (N'Phòng Sakura', 1, 1);
INSERT INTO StudyRoom (Name, IsAvailable, RoomTypeId) VALUES (N'Phòng Tulip', 1, 2);
INSERT INTO StudyRoom (Name, IsAvailable, RoomTypeId) VALUES (N'Phòng Rose', 1, 3);
INSERT INTO StudyRoom (Name, IsAvailable, RoomTypeId) VALUES (N'Phòng Orchid', 1, 4);
INSERT INTO StudyRoom (Name, IsAvailable, RoomTypeId) VALUES (N'Phòng Event', 1, 5);
-- Reservation INSERT
INSERT INTO Reservation (UserId, RoomId, StartTime, EndTime, Status) VALUES (1, 1, '2025-07-01 09:00:00', '2025-07-01 11:00:00', N'Đã đặt');
INSERT INTO Reservation (UserId, RoomId, StartTime, EndTime, Status) VALUES (2, 2, '2025-07-01 13:00:00', '2025-07-01 15:00:00', N'Đã đặt');
INSERT INTO Reservation (UserId, RoomId, StartTime, EndTime, Status) VALUES (3, 3, '2025-07-02 09:00:00', '2025-07-02 11:00:00', N'Đã hủy');
INSERT INTO Reservation (UserId, RoomId, StartTime, EndTime, Status) VALUES (4, 4, '2025-07-02 13:00:00', '2025-07-02 15:00:00', N'Đã đặt');
INSERT INTO Reservation (UserId, RoomId, StartTime, EndTime, Status) VALUES (5, 5, '2025-07-03 09:00:00', '2025-07-03 11:00:00', N'Đã đặt');
-- Voucher INSERT
INSERT INTO Voucher (VoucherCode, Description, DiscountPercent, ExpiryDate, IsActive) VALUES (N'WELCOME10', N'Giảm 10% cho khách mới', 10.0, '2025-08-01 23:59:59', 1);
INSERT INTO Voucher (VoucherCode, Description, DiscountPercent, ExpiryDate, IsActive) VALUES (N'SUMMER15', N'Ưu đãi hè 15%', 15.0, '2025-08-15 23:59:59', 1);
INSERT INTO Voucher (VoucherCode, Description, DiscountPercent, ExpiryDate, IsActive) VALUES (N'STUDENT5', N'Giảm 5% cho sinh viên', 5.0, '2025-12-31 23:59:59', 1);
INSERT INTO Voucher (VoucherCode, Description, DiscountPercent, ExpiryDate, IsActive) VALUES (N'FREESHIP', N'Miễn phí vận chuyển', 0.0, '2025-09-30 23:59:59', 1);
INSERT INTO Voucher (VoucherCode, Description, DiscountPercent, ExpiryDate, IsActive) VALUES (N'VIP20', N'Khách VIP giảm 20%', 20.0, '2025-12-31 23:59:59', 1);
-- [Order] INSERT
INSERT INTO [Order] (UserId, OrderDate, TotalAmount, Status, VoucherId) VALUES (1, '2025-07-01 09:05:00', 70.0, N'Chờ xác nhận', 1);
INSERT INTO [Order] (UserId, OrderDate, TotalAmount, Status, VoucherId) VALUES (2, '2025-07-01 13:10:00', 45.0, N'Đã thanh toán', 2);
INSERT INTO [Order] (UserId, OrderDate, TotalAmount, Status, VoucherId) VALUES (3, '2025-07-02 09:15:00', 30.0, N'Đã hủy', 3);
INSERT INTO [Order] (UserId, OrderDate, TotalAmount, Status, VoucherId) VALUES (4, '2025-07-02 13:20:00', 55.0, N'Chờ xác nhận', 4);
INSERT INTO [Order] (UserId, OrderDate, TotalAmount, Status, VoucherId) VALUES (5, '2025-07-03 09:25:00', 80.0, N'Đã thanh toán', 5);
-- OrderItem INSERT
INSERT INTO OrderItem (OrderId, DrinkId, Quantity, Price) VALUES (1, 1, 2, 7.0);
INSERT INTO OrderItem (OrderId, DrinkId, Quantity, Price) VALUES (1, 3, 1, 3.2);
INSERT INTO OrderItem (OrderId, DrinkId, Quantity, Price) VALUES (2, 2, 2, 5.6);
INSERT INTO OrderItem (OrderId, DrinkId, Quantity, Price) VALUES (3, 4, 1, 4.0);
INSERT INTO OrderItem (OrderId, DrinkId, Quantity, Price) VALUES (4, 5, 3, 13.5);
INSERT INTO OrderItem (OrderId, DrinkId, Quantity, Price) VALUES (5, 6, 2, 5.0);
-- Transaction INSERT
INSERT INTO Transactions (OrderId, TransactionDate, AmountPaid, PaymentMethod, Status) VALUES (1, '2025-07-01 09:10:00', 70.0, N'Momo', N'Đã thanh toán');
INSERT INTO Transactions (OrderId, TransactionDate, AmountPaid, PaymentMethod, Status) VALUES (2, '2025-07-01 13:15:00', 45.0, N'Tiền mặt', N'Đã thanh toán');
INSERT INTO Transactions (OrderId, TransactionDate, AmountPaid, PaymentMethod, Status) VALUES (3, '2025-07-02 09:20:00', 0.0, N'Chưa thanh toán', N'Đã hủy');
INSERT INTO Transactions (OrderId, TransactionDate, AmountPaid, PaymentMethod, Status) VALUES (4, '2025-07-02 13:25:00', 55.0, N'ZaloPay', N'Đã thanh toán');
INSERT INTO Transactions (OrderId, TransactionDate, AmountPaid, PaymentMethod, Status) VALUES (5, '2025-07-03 09:30:00', 80.0, N'Credit Card', N'Đã thanh toán');
-- Payment INSERT
INSERT INTO Payment (TransactionId, PaymentType, PaymentDate, Amount) VALUES (1, N'Momo', '2025-07-01 09:10:00', 70.0);
INSERT INTO Payment (TransactionId, PaymentType, PaymentDate, Amount) VALUES (2, N'Tiền mặt', '2025-07-01 13:15:00', 45.0);
INSERT INTO Payment (TransactionId, PaymentType, PaymentDate, Amount) VALUES (3, N'Không thanh toán', '2025-07-02 09:20:00', 0.0);
INSERT INTO Payment (TransactionId, PaymentType, PaymentDate, Amount) VALUES (4, N'ZaloPay', '2025-07-02 13:25:00', 55.0);
INSERT INTO Payment (TransactionId, PaymentType, PaymentDate, Amount) VALUES (5, N'Credit Card', '2025-07-03 09:30:00', 80.0);
-- Menu INSERT
INSERT INTO Menu (DrinkId, Name) VALUES (1, N'Trà sữa truyền thống');
INSERT INTO Menu (DrinkId, Name) VALUES (2, N'Cà phê sữa đá');
INSERT INTO Menu (DrinkId, Name) VALUES (3, N'Nước cam tươi');
INSERT INTO Menu (DrinkId, Name) VALUES (4, N'Matcha Latte');
INSERT INTO Menu (DrinkId, Name) VALUES (5, N'Capuchino');
-- Review INSERT
INSERT INTO Review (UserId, DrinkId, Rating, Comment, ReviewDate) VALUES (1, 1, 5, N'Rất ngon, sẽ quay lại!', '2025-07-01 10:00:00');
INSERT INTO Review (UserId, DrinkId, Rating, Comment, ReviewDate) VALUES (2, 2, 4, N'Cà phê thơm, giá hợp lý.', '2025-07-01 14:00:00');
INSERT INTO Review (UserId, DrinkId, Rating, Comment, ReviewDate) VALUES (3, 3, 3, N'Nước cam hơi ngọt.', '2025-07-02 10:00:00');
INSERT INTO Review (UserId, DrinkId, Rating, Comment, ReviewDate) VALUES (4, 4, 5, N'Matcha rất chuẩn vị.', '2025-07-02 14:00:00');
INSERT INTO Review (UserId, DrinkId, Rating, Comment, ReviewDate) VALUES (5, 5, 4, N'Capuchino ổn, phục vụ tốt.', '2025-07-03 10:00:00');
-- Suggestion INSERT
INSERT INTO Suggestion (UserId, Content, SuggestionDate) VALUES (1, N'Nên thêm nhiều loại bánh ngọt.', '2025-07-01 11:00:00');
INSERT INTO Suggestion (UserId, Content, SuggestionDate) VALUES (2, N'Có thể mở thêm phòng nhóm lớn.', '2025-07-01 15:00:00');
INSERT INTO Suggestion (UserId, Content, SuggestionDate) VALUES (3, N'Giảm giá cho sinh viên vào thứ 2.', '2025-07-02 11:00:00');
INSERT INTO Suggestion (UserId, Content, SuggestionDate) VALUES (4, N'Thêm nhiều loại trà trái cây.', '2025-07-02 15:00:00');
INSERT INTO Suggestion (UserId, Content, SuggestionDate) VALUES (5, N'Có thể mở cửa sớm hơn.', '2025-07-03 11:00:00');
-- SuggestionItem INSERT
INSERT INTO SuggestionItem (SuggestionId, DrinkId) VALUES (1, 7);
INSERT INTO SuggestionItem (SuggestionId, DrinkId) VALUES (2, 9);
INSERT INTO SuggestionItem (SuggestionId, DrinkId) VALUES (3, 12);
INSERT INTO SuggestionItem (SuggestionId, DrinkId) VALUES (4, 14);
INSERT INTO SuggestionItem (SuggestionId, DrinkId) VALUES (5, 1);