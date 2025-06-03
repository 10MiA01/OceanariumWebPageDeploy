# 🌊 Oceanarium Web Application
A web-based oceanarium management system built with **ASP.NET Core** and **Razor Pages**, featuring separate interfaces for clients and administrators.

View: https://skyoceanarium.onrender.com/

## 🎯 Features

### 🧑‍🤝‍🧑 Client Side
- Browse current **exhibitions** and **events**
- View **ticket prices** and make reservations
- Manage orders via special **email links** (no registration required)
- Receive **QR code** for entrance upon order

### 🛠️ Admin Side
- **CRUD** operations for events, exhibitions, and orders
- Filter/search tools for managing large datasets
- Simple **key-code login** for secure access  
  🔑 Default admin key: `superadmin123`

## 💡 Highlight Features

- 🔐 **Key-Code Admin Login**  --  Simple admin access without user accounts

- 📧 **Email-Driven Order Management**  --  Manage ticket orders via secure email links

- 🎟️ **QR Code Ticketing** --   Automatically generate entrance QR codes

- 📊 **Dynamic Admin Filters**  --  Quickly search and filter orders and events

- 🐳 **Dockerized with Volume Support**  --   Container-friendly with persistent SQLite storage

- 🚀 **Live Deployment on Render** --    Cloud-hosted with automatic port configuration



## 🗄️ Tech Stack

| Category       | Tech Used                                                  |
|----------------|------------------------------------------------------------|
| Backend        | C#, ASP.NET Core, Razor Pages, Entity Framework Core       |
| Auth           | Custom key-code login                                      |
| DB             | SQLite                                                     |
| Email          | MailKit(Development); SendGrid(Production)                 |
| QR Code        | QRCoder                                                    |
| Frontend       | HTML, CSS (Bootstrap), JavaScript, jQuery, DataTables.js   |
| Containerization| Docker                                                    |
| Deployment     | Render                                                     |



## 📝 Final Note

This project was a great learning experience — a bit of coding, a lot of coffee ☕, and lots of fun building something real.  
Feel free to explore, test, and reach out if you have any questions or feedback!

## 📬 PS

If you want to test the site, orders can be accessed via the URL pattern `/OrderCancel?code={orderCode}`.  
Emails with the link may sometimes go to spam or get blocked, but you can always find the order code in the admin panel to test access directly.
