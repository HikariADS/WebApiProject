# Hướng dẫn cấu hình Email Service

## Cấu hình SMTP trong appsettings.json

Để sử dụng tính năng gửi email xác thực, bạn cần cấu hình thông tin SMTP trong file `appsettings.json`:

```json
"Email": {
  "SmtpHost": "smtp.gmail.com",
  "SmtpPort": "587",
  "SmtpUsername": "your-email@gmail.com",
  "SmtpPassword": "your-app-password",
  "FromEmail": "noreply@warehouse.com",
  "FromName": "Warehouse System",
  "BaseUrl": "http://localhost:3000"
}
```

## Cấu hình Gmail

1. **Tạo App Password cho Gmail:**
   - Vào Google Account → Security
   - Bật 2-Step Verification (nếu chưa bật)
   - Tạo App Password mới
   - Sử dụng App Password này cho `SmtpPassword`

2. **Các SMTP server phổ biến:**
   - **Gmail:** smtp.gmail.com, port 587
   - **Outlook:** smtp-mail.outlook.com, port 587
   - **Yahoo:** smtp.mail.yahoo.com, port 587

## Lưu ý

- `BaseUrl` phải trỏ đến frontend URL để link verify email hoạt động đúng
- Trong môi trường production, nên sử dụng biến môi trường thay vì hardcode trong appsettings.json
- Token verify email có thời hạn 24 giờ

