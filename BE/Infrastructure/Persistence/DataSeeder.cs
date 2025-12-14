using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApiProject.Infrastructure.Persistence;
using WebApiProject.Domain.Entities;


namespace WebApiProject.Infrastructure.Persistence
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(AppDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            var rnd = new Random();
            // await context.Database.MigrateAsync(); // It's better to run migrations via command line
            if (!context.ProductTypes.Any())
            {
                var types = new List<ProductType>
                {
                    new() { 
                        Name = "Điện tử", 
                        Description = "Các sản phẩm điện tử như laptop, điện thoại, máy tính bảng, TV, tai nghe và các thiết bị công nghệ khác"
                    },
                    new() { 
                        Name = "Đồ chơi", 
                        Description = "Đồ chơi trẻ em, đồ chơi giáo dục, mô hình, búp bê và các sản phẩm giải trí cho trẻ em"
                    },
                    new() { 
                        Name = "Quần áo", 
                        Description = "Quần áo nam, nữ, trẻ em bao gồm áo sơ mi, quần jean, váy, áo khoác và các phụ kiện thời trang"
                    },
                    new() { 
                        Name = "Phụ kiện", 
                        Description = "Túi xách, ví, đồng hồ, kính mát, thắt lưng và các phụ kiện thời trang, công nghệ"
                    },
                    new() { 
                        Name = "Thể thao", 
                        Description = "Dụng cụ thể thao, quần áo thể thao, giày thể thao, thiết bị tập luyện và các sản phẩm liên quan đến thể thao"
                    }
                };
                context.ProductTypes.AddRange(types);
                await context.SaveChangesAsync();
            }
            if (!context.Products.Any())
            {
                var types = context.ProductTypes.ToList();
                var electronicsType = types.FirstOrDefault(t => t.Name.Contains("Điện tử")) ?? types[0];
                var toysType = types.FirstOrDefault(t => t.Name.Contains("Đồ chơi")) ?? types[0];
                var clothingType = types.FirstOrDefault(t => t.Name.Contains("Quần áo")) ?? types[0];
                var accessoriesType = types.FirstOrDefault(t => t.Name.Contains("Phụ kiện")) ?? types[0];
                var sportsType = types.FirstOrDefault(t => t.Name.Contains("Thể thao")) ?? types[0];

                var products = new List<Product>
                {
                    // Điện tử
                    new() { Name = "Laptop Dell XPS 15", Description = "Laptop cao cấp với màn hình 15.6 inch Full HD, CPU Intel Core i7, RAM 16GB, SSD 512GB. Phù hợp cho công việc và giải trí.", Price = 25000000, ProductTypeId = electronicsType.Id, Quantity = 25 },
                    new() { Name = "iPhone 15 Pro Max", Description = "Điện thoại thông minh flagship với chip A17 Pro, camera 48MP, màn hình 6.7 inch Super Retina XDR. Hỗ trợ 5G và Face ID.", Price = 32000000, ProductTypeId = electronicsType.Id, Quantity = 40 },
                    new() { Name = "Samsung Galaxy S24 Ultra", Description = "Smartphone Android cao cấp với bút S Pen, camera 200MP, màn hình Dynamic AMOLED 2X 6.8 inch. Pin 5000mAh.", Price = 28000000, ProductTypeId = electronicsType.Id, Quantity = 35 },
                    new() { Name = "MacBook Air M3", Description = "Laptop Apple với chip M3, màn hình 13.6 inch Liquid Retina, RAM 8GB, SSD 256GB. Thiết kế mỏng nhẹ, pin lâu.", Price = 29000000, ProductTypeId = electronicsType.Id, Quantity = 30 },
                    new() { Name = "iPad Pro 12.9 inch", Description = "Máy tính bảng cao cấp với chip M2, màn hình Liquid Retina XDR, hỗ trợ Apple Pencil và Magic Keyboard.", Price = 35000000, ProductTypeId = electronicsType.Id, Quantity = 20 },
                    new() { Name = "Sony WH-1000XM5", Description = "Tai nghe chống ồn chủ động với công nghệ Noise Cancelling, pin 30 giờ, hỗ trợ LDAC và Quick Attention.", Price = 8500000, ProductTypeId = electronicsType.Id, Quantity = 50 },
                    new() { Name = "Samsung TV QLED 55 inch", Description = "Smart TV 4K QLED với công nghệ Quantum Dot, HDR10+, hệ điều hành Tizen, hỗ trợ voice control.", Price = 18000000, ProductTypeId = electronicsType.Id, Quantity = 15 },
                    new() { Name = "AirPods Pro 2", Description = "Tai nghe không dây với chống ồn chủ động, Adaptive Audio, pin 6 giờ, case sạc MagSafe.", Price = 6500000, ProductTypeId = electronicsType.Id, Quantity = 80 },
                    
                    // Đồ chơi
                    new() { Name = "Lego Classic 10698", Description = "Bộ xếp hình Lego với 790 mảnh nhiều màu sắc, kích thích sáng tạo cho trẻ từ 4 tuổi trở lên.", Price = 1200000, ProductTypeId = toysType.Id, Quantity = 60 },
                    new() { Name = "Xe đạp trẻ em 16 inch", Description = "Xe đạp 2 bánh cho trẻ 5-8 tuổi, có bánh phụ, phanh tay an toàn, nhiều màu sắc.", Price = 1800000, ProductTypeId = toysType.Id, Quantity = 40 },
                    new() { Name = "Búp bê Barbie Fashionista", Description = "Búp bê Barbie với nhiều trang phục và phụ kiện, kích thích trí tưởng tượng của trẻ.", Price = 450000, ProductTypeId = toysType.Id, Quantity = 100 },
                    new() { Name = "Xe điều khiển từ xa", Description = "Xe RC điều khiển từ xa, tốc độ cao, pin sạc, điều khiển 2.4GHz, phù hợp trẻ 8+ tuổi.", Price = 2500000, ProductTypeId = toysType.Id, Quantity = 45 },
                    new() { Name = "Bộ đồ chơi xếp hình gỗ", Description = "Đồ chơi giáo dục bằng gỗ tự nhiên, an toàn, giúp trẻ phát triển tư duy và kỹ năng vận động.", Price = 550000, ProductTypeId = toysType.Id, Quantity = 70 },
                    new() { Name = "Robot lập trình", Description = "Robot giáo dục có thể lập trình, dạy trẻ về coding và robotics, phù hợp 8-14 tuổi.", Price = 3500000, ProductTypeId = toysType.Id, Quantity = 25 },
                    
                    // Quần áo
                    new() { Name = "Áo sơ mi nam công sở", Description = "Áo sơ mi dài tay, chất liệu cotton cao cấp, form fit, nhiều màu sắc. Phù hợp môi trường công sở.", Price = 450000, ProductTypeId = clothingType.Id, Quantity = 120 },
                    new() { Name = "Quần jean nữ slim fit", Description = "Quần jean nữ form slim, chất liệu denim co giãn, nhiều size. Thiết kế hiện đại, dễ phối đồ.", Price = 750000, ProductTypeId = clothingType.Id, Quantity = 90 },
                    new() { Name = "Áo khoác gió chống nước", Description = "Áo khoác gió chống nước, chống gió, có mũ, nhiều túi. Phù hợp đi du lịch và hoạt động ngoài trời.", Price = 1200000, ProductTypeId = clothingType.Id, Quantity = 65 },
                    new() { Name = "Váy liền thân công sở", Description = "Váy công sở dài đến gối, chất liệu vải cao cấp, form đẹp. Nhiều màu sắc và size.", Price = 850000, ProductTypeId = clothingType.Id, Quantity = 75 },
                    new() { Name = "Áo thun nam cotton", Description = "Áo thun nam chất liệu cotton 100%, thoáng mát, dễ giặt. Nhiều màu sắc và size.", Price = 250000, ProductTypeId = clothingType.Id, Quantity = 150 },
                    new() { Name = "Quần short thể thao", Description = "Quần short thể thao co giãn, thoáng mát, có túi. Phù hợp tập luyện và hoạt động thể thao.", Price = 350000, ProductTypeId = clothingType.Id, Quantity = 110 },
                    
                    // Phụ kiện
                    new() { Name = "Túi xách da thật", Description = "Túi xách da bò thật, thiết kế sang trọng, nhiều ngăn, phù hợp công sở và đi chơi.", Price = 2800000, ProductTypeId = accessoriesType.Id, Quantity = 45 },
                    new() { Name = "Ví da nam cao cấp", Description = "Ví da thật, nhiều ngăn đựng thẻ và tiền, thiết kế tinh tế, bền đẹp.", Price = 650000, ProductTypeId = accessoriesType.Id, Quantity = 100 },
                    new() { Name = "Apple Watch Series 9", Description = "Đồng hồ thông minh với màn hình Always-On, đo nhịp tim, GPS, chống nước. Pin 18 giờ.", Price = 12000000, ProductTypeId = accessoriesType.Id, Quantity = 55 },
                    new() { Name = "Kính mát Ray-Ban", Description = "Kính mát chống tia UV 100%, tròng chống chói, gọng nhựa bền. Nhiều kiểu dáng.", Price = 2500000, ProductTypeId = accessoriesType.Id, Quantity = 60 },
                    new() { Name = "Thắt lưng da nam", Description = "Thắt lưng da thật, khóa kim loại, nhiều size. Phù hợp công sở và thời trang.", Price = 550000, ProductTypeId = accessoriesType.Id, Quantity = 85 },
                    new() { Name = "Balo laptop", Description = "Balo chống sốc, nhiều ngăn, đựng được laptop 15 inch, thiết kế hiện đại.", Price = 1200000, ProductTypeId = accessoriesType.Id, Quantity = 70 },
                    
                    // Thể thao
                    new() { Name = "Giày chạy bộ Nike Air Max", Description = "Giày thể thao chạy bộ với công nghệ Air Max, đế cao su bền, thoáng khí. Nhiều size.", Price = 3200000, ProductTypeId = sportsType.Id, Quantity = 80 },
                    new() { Name = "Vợt cầu lông Yonex", Description = "Vợt cầu lông chuyên nghiệp, trọng lượng nhẹ, cân bằng tốt. Phù hợp người chơi trung bình đến cao cấp.", Price = 1800000, ProductTypeId = sportsType.Id, Quantity = 50 },
                    new() { Name = "Bóng đá Adidas", Description = "Bóng đá chính thức size 5, da tổng hợp, độ nảy tốt. Phù hợp sân cỏ nhân tạo và tự nhiên.", Price = 650000, ProductTypeId = sportsType.Id, Quantity = 100 },
                    new() { Name = "Quần áo thể thao bộ", Description = "Bộ đồ tập gym, chất liệu co giãn, thấm hút mồ hôi. Gồm áo và quần.", Price = 850000, ProductTypeId = sportsType.Id, Quantity = 95 },
                    new() { Name = "Mũ bảo hiểm xe đạp", Description = "Mũ bảo hiểm đạt tiêu chuẩn an toàn, nhẹ, thoáng khí. Nhiều size và màu sắc.", Price = 450000, ProductTypeId = sportsType.Id, Quantity = 65 },
                    new() { Name = "Máy chạy bộ điện", Description = "Máy tập thể dục tại nhà, tốc độ điều chỉnh, màn hình hiển thị, gấp gọn được.", Price = 15000000, ProductTypeId = sportsType.Id, Quantity = 12 },
                    new() { Name = "Tạ tay bộ 20kg", Description = "Bộ tạ tay có thể điều chỉnh trọng lượng, tay cầm chống trượt. Phù hợp tập tại nhà.", Price = 1800000, ProductTypeId = sportsType.Id, Quantity = 40 },
                    new() { Name = "Thảm tập yoga", Description = "Thảm tập yoga chống trượt, dày 6mm, dễ vệ sinh. Kích thước chuẩn 183x61cm.", Price = 350000, ProductTypeId = sportsType.Id, Quantity = 85 },
                    new() { Name = "Bình nước thể thao", Description = "Bình nước 1L, chất liệu nhựa an toàn, có vòi uống, dễ vệ sinh. Nhiều màu sắc.", Price = 180000, ProductTypeId = sportsType.Id, Quantity = 120 },
                    new() { Name = "Balo thể thao", Description = "Balo du lịch thể thao, nhiều ngăn, chống nước nhẹ, phù hợp đi phượt và thể thao.", Price = 950000, ProductTypeId = sportsType.Id, Quantity = 55 }
                };
                
                context.Products.AddRange(products);
                await context.SaveChangesAsync();
            }
            if (!context.StorageTypes.Any())
            {
                var storageTypes = new List<StorageType>
                {
                    new() { Name = "Kho chính TP.HCM", ManagerName = "Nguyễn Văn Anh", StorageLocation = "123 Đường Nguyễn Huệ, Phường Bến Nghé, Quận 1, TP.HCM" },
                    new() { Name = "Kho phụ Bình Dương", ManagerName = "Trần Thị Bình", StorageLocation = "456 Đường Đại Lộ Bình Dương, Phường Chánh Nghĩa, Thủ Dầu Một, Bình Dương" },
                    new() { Name = "Kho trưng bày Quận 7", ManagerName = "Lê Văn Cường", StorageLocation = "789 Đường Nguyễn Thị Thập, Phường Tân Phú, Quận 7, TP.HCM" },
                    new() { Name = "Kho miền Bắc Hà Nội", ManagerName = "Phạm Thị Dung", StorageLocation = "321 Đường Láng, Phường Láng Thượng, Đống Đa, Hà Nội" },
                    new() { Name = "Kho miền Trung Đà Nẵng", ManagerName = "Hoàng Văn Em", StorageLocation = "654 Đường Trần Phú, Phường Hải Châu, Quận Hải Châu, Đà Nẵng" },
                    new() { Name = "Kho Cần Thơ", ManagerName = "Võ Thị Phương", StorageLocation = "987 Đường 30/4, Phường An Khánh, Ninh Kiều, Cần Thơ" },
                    new() { Name = "Kho Đà Lạt", ManagerName = "Đặng Văn Giang", StorageLocation = "147 Đường Trần Hưng Đạo, Phường 10, Đà Lạt, Lâm Đồng" }
                };
                context.StorageTypes.AddRange(storageTypes);
                await context.SaveChangesAsync();
            }

            var roles = new[] { "Admin", "Manager", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            if (await userManager.FindByEmailAsync("admin@local.com") == null)
            {
                var admin = new User
                {
                    UserName = "admin",
                    Email = "admin@local.com",
                    FullName = "Quản trị viên",
                    EmailConfirmed = true,
                    UnitId = "UNIT-001"
                };
                await userManager.CreateAsync(admin, "Admin@123");
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            User? managerUser = null;
            if (await userManager.FindByEmailAsync("manager@local.com") == null)
            {
                managerUser = new User
                {
                    UserName = "manager",
                    Email = "manager@local.com",
                    FullName = "Người quản lý",
                    EmailConfirmed = true,
                    UnitId = "UNIT-001"
                };
                await userManager.CreateAsync(managerUser, "Manager@123");
                await userManager.AddToRoleAsync(managerUser, "Manager");
            }
            else
            {
                managerUser = await userManager.FindByEmailAsync("manager@local.com");
            }
            
            // Đảm bảo managerUser không null trước khi seed storages
            if (managerUser == null)
            {
                throw new Exception("Manager user không thể tạo hoặc tìm thấy!");
            }

            if (await userManager.FindByEmailAsync("user@local.com") == null)
            {
                var user = new User
                {
                    UserName = "user",
                    Email = "user@local.com",
                    FullName = "Người dùng",
                    EmailConfirmed = true,
                    UnitId = "UNIT-001"
                };
                await userManager.CreateAsync(user, "User@123");
                await userManager.AddToRoleAsync(user, "User");
            }

            var adminUser = await userManager.FindByEmailAsync("admin@local.com");
            if (!context.Storages.Any() && adminUser != null)
            {
                var products = await context.Products.ToListAsync();
                var storageTypes = await context.StorageTypes.ToListAsync();
                
                if (products.Any() && storageTypes.Any() && adminUser != null)
                {
                    var mainStorage = storageTypes.FirstOrDefault(s => s.Name.Contains("chính")) ?? storageTypes[0];
                    var backupStorage = storageTypes.FirstOrDefault(s => s.Name.Contains("phụ")) ?? storageTypes[1];
                    var showroomStorage = storageTypes.FirstOrDefault(s => s.Name.Contains("trưng bày")) ?? storageTypes[2];
                    
                    // Seed storages với dữ liệu thực tế hơn
                    var storages = new List<Storage>();
                    
                    // Phân bổ sản phẩm vào các kho một cách hợp lý
                    // Tạo nhiều storages hơn để có dữ liệu phong phú cho biểu đồ
                    var productIndex = 0;
                    foreach (var product in products)
                    {
                        // Mỗi sản phẩm có thể có nhiều lần nhập kho trong các tháng khác nhau
                        var numberOfImports = rnd.Next(1, 4); // Mỗi sản phẩm có 1-3 lần nhập
                        
                        for (int i = 0; i < numberOfImports; i++)
                        {
                            var storageType = rnd.Next(0, storageTypes.Count);
                            
                            // Số lượng phù hợp với từng loại sản phẩm
                            int quantity = product.ProductTypeId switch
                            {
                                _ when product.Name.Contains("Laptop") || product.Name.Contains("MacBook") => rnd.Next(5, 30),
                                _ when product.Name.Contains("iPhone") || product.Name.Contains("Samsung") => rnd.Next(20, 50),
                                _ when product.Name.Contains("TV") || product.Name.Contains("Máy chạy bộ") => rnd.Next(3, 15),
                                _ when product.Name.Contains("Áo") || product.Name.Contains("Quần") => rnd.Next(50, 150),
                                _ when product.Name.Contains("Giày") || product.Name.Contains("Balo") => rnd.Next(30, 100),
                                _ => rnd.Next(20, 80)
                            };
                            
                            // Phân bổ ImportDate rải đều trong 6 tháng qua để biểu đồ xu hướng đẹp hơn
                            // Mỗi tháng có khoảng 15-20% số lượng nhập
                            var monthsAgo = rnd.Next(0, 6); // 0-5 tháng trước
                            var daysInMonth = rnd.Next(1, 28); // Ngày trong tháng
                            var importDate = DateTimeOffset.Now.AddMonths(-monthsAgo).AddDays(-daysInMonth);
                            
                            storages.Add(new Storage
                            {
                                ProductId = product.Id,
                                Quantity = quantity,
                                UserId = adminUser.Id,
                                StorageTypeId = storageTypes[storageType].Id,
                                ImportDate = importDate,
                                ExportDate = rnd.Next(0, 4) == 0 ? null : importDate.AddDays(rnd.Next(1, 30)), // 25% chưa xuất
                                BelongToUnitId = "UNIT-001",
                                ManagerId = managerUser?.Id
                            });
                        }
                        
                        productIndex++;
                        // Giới hạn số lượng để không quá nhiều
                        if (productIndex >= 40) break;
                    }
                    
                    context.Storages.AddRange(storages);
                    await context.SaveChangesAsync();
                }
            }

            // Seed News
            if (!context.News.Any())
            {
                var adminUserForNews = await userManager.FindByEmailAsync("admin@local.com");
                var managerUserForNews = await userManager.FindByEmailAsync("manager@local.com");
                
                if (adminUserForNews != null && managerUserForNews != null)
                {
                    var news = new List<News>
                    {
                        new()
                        {
                            Title = "Chào mừng đến với hệ thống quản lý kho",
                            Content = "Hệ thống quản lý kho mới đã được triển khai thành công. Hệ thống giúp bạn quản lý sản phẩm, kho hàng và người dùng một cách hiệu quả. Vui lòng làm quen với các tính năng mới và liên hệ với quản trị viên nếu có thắc mắc.",
                            Category = "Thông báo",
                            AuthorId = adminUserForNews.Id,
                            AuthorName = adminUserForNews.FullName ?? adminUserForNews.UserName ?? "Admin",
                            CreatedAt = DateTime.UtcNow.AddDays(-10)
                        },
                        new()
                        {
                            Title = "Cập nhật tính năng báo cáo mới",
                            Content = "Chúng tôi đã thêm trang báo cáo mới với nhiều thống kê chi tiết hơn. Bạn có thể xem top sản phẩm, kho hàng có số lượng nhiều nhất, sản phẩm sắp hết hàng và nhiều thông tin hữu ích khác. Hãy khám phá ngay!",
                            Category = "Cập nhật",
                            AuthorId = adminUserForNews.Id,
                            AuthorName = adminUserForNews.FullName ?? adminUserForNews.UserName ?? "Admin",
                            CreatedAt = DateTime.UtcNow.AddDays(-7)
                        },
                        new()
                        {
                            Title = "Hướng dẫn sử dụng hệ thống quản lý kho",
                            Content = "Xem hướng dẫn chi tiết về cách sử dụng các tính năng trong hệ thống quản lý kho. Bao gồm: quản lý sản phẩm, quản lý kho hàng, tạo báo cáo, quản lý người dùng và phân quyền. Tài liệu hướng dẫn đầy đủ có sẵn trong menu Help.",
                            Category = "Hướng dẫn",
                            AuthorId = managerUserForNews.Id,
                            AuthorName = managerUserForNews.FullName ?? managerUserForNews.UserName ?? "Manager",
                            CreatedAt = DateTime.UtcNow.AddDays(-5)
                        },
                        new()
                        {
                            Title = "Thông báo về quy trình nhập xuất kho",
                            Content = "Vui lòng tuân thủ quy trình nhập xuất kho mới: 1) Kiểm tra số lượng sản phẩm trước khi nhập/xuất, 2) Cập nhật thông tin đầy đủ trong hệ thống, 3) Xác nhận với quản lý kho trước khi thực hiện giao dịch lớn. Mọi thắc mắc xin liên hệ phòng quản lý kho.",
                            Category = "Thông báo",
                            AuthorId = managerUserForNews.Id,
                            AuthorName = managerUserForNews.FullName ?? managerUserForNews.UserName ?? "Manager",
                            CreatedAt = DateTime.UtcNow.AddDays(-3)
                        },
                        new()
                        {
                            Title = "Cải tiến giao diện người dùng",
                            Content = "Chúng tôi đã cải tiến giao diện người dùng để dễ sử dụng hơn. Các tính năng mới bao gồm: tìm kiếm nhanh, lọc dữ liệu, xem dạng card trên mobile, và nhiều cải tiến khác. Hãy trải nghiệm và cho chúng tôi biết ý kiến của bạn!",
                            Category = "Cập nhật",
                            AuthorId = adminUserForNews.Id,
                            AuthorName = adminUserForNews.FullName ?? adminUserForNews.UserName ?? "Admin",
                            CreatedAt = DateTime.UtcNow.AddDays(-2)
                        },
                        new()
                        {
                            Title = "Lịch bảo trì hệ thống",
                            Content = "Hệ thống sẽ được bảo trì định kỳ vào cuối tuần. Thời gian bảo trì: Chủ nhật hàng tuần từ 2:00 - 4:00 sáng. Trong thời gian này, hệ thống có thể tạm thời không khả dụng. Vui lòng lưu công việc trước khi hệ thống bảo trì. Xin cảm ơn!",
                            Category = "Thông báo",
                            AuthorId = adminUserForNews.Id,
                            AuthorName = adminUserForNews.FullName ?? adminUserForNews.UserName ?? "Admin",
                            CreatedAt = DateTime.UtcNow.AddDays(-1)
                        },
                        new()
                        {
                            Title = "Tính năng quản lý tin tức mới",
                            Content = "Chúng tôi đã thêm trang quản lý tin tức mới. Admin và Manager có thể tạo, chỉnh sửa và xóa tin tức. Người dùng có thể xem tất cả tin tức công khai. Hãy sử dụng tính năng này để cập nhật thông tin quan trọng cho toàn bộ nhân viên.",
                            Category = "Cập nhật",
                            AuthorId = adminUserForNews.Id,
                            AuthorName = adminUserForNews.FullName ?? adminUserForNews.UserName ?? "Admin",
                            CreatedAt = DateTime.UtcNow
                        }
                    };

                    context.News.AddRange(news);
                    await context.SaveChangesAsync();
                }
            }
        }
    }

}