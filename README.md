💬 ChatApp – Real-Time Chat Application

A full-stack real-time chat application built with ASP.NET Core, SignalR, and JavaScript, featuring private chats, group chats, live online status, and instant message delivery.

🚀 Key Features

🔐 JWT-based authentication (Login, Register, Google Login)

💬 Real-time private & group messaging (SignalR)

🟢 Live online / offline user status

📩 Message delivery & read receipts

🔄 Multi-device & multi-tab support

📜 Paginated message history

👥 Group chat creation

🔑 Forgot & Reset Password with Email OTP

🛠️ Technologies Used
Backend

ASP.NET Core Web API

SignalR (Real-time communication)

Entity Framework Core

SQL Server

JWT Authentication

Google OAuth

SendGrid / SMTP for Emails

Frontend

HTML, CSS, JavaScript

SignalR JavaScript Client

🧠 Architecture

Clean layered architecture:

API

Application

Infrastructure

Domain

Repository Pattern

Service Layer for business logic

Centralized Error Handling

Real-time presence tracking using in-memory + database sync

⚡ Real-Time Engine

Users are tracked using SignalR connection IDs

Online status is maintained using a heartbeat system

Messages are delivered instantly via SignalR Groups

Works across multiple devices & browser tabs
