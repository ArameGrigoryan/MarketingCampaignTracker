# MarketingCampaignTracker

**MarketingCampaignTracker** is a microservice-based backend system for managing and analyzing digital marketing campaigns.  
It provides REST APIs to define campaigns, launch them, collect performance events (views, clicks, conversions), and read live metrics.  

The system follows modern backend design principles: **Clean Architecture**, **Domain-Driven Design (DDD)**, **service-to-service communication**, **Redis caching**, and **Docker-based deployment**.

---

## 🚀 Features

- Campaign CRUD operations with JWT-based authentication  
- Real-time analytics ingestion service  
- Custom middleware for bot traffic filtering  
- Redis-based metrics caching for high performance  
- Atomic campaign launch transactions  
- Service-to-service communication with caching fallback  
- Comprehensive Swagger API documentation  
- Full Docker orchestration (PostgreSQL + Redis + APIs)  
- Clean Architecture layering: Domain, Application, Infrastructure, API  

---

## 🧠 System Overview

The project is divided into **three main services** and one shared library layer:

| Service | Description | Port |
|----------|--------------|------|
| **Core.Api** | Gateway service that proxies requests and aggregates analytics data | `5000` |
| **Campaign.Api** | Campaign management microservice (CRUD, Auth, Launch) | `5005` |
| **Analytics.Api** | Analytics and metrics ingestion microservice | `5006` |
| **PostgreSQL** | Primary database for both Campaign and Analytics services | `5432` |
| **Redis** | Real-time cache for metrics and S2S lookups | `6379` |

### 🧩 Inter-Service Communication
- `AnalyticsSvc` contacts `CampaignSvc` via `HEAD /campaigns/{id}/exists` to verify campaign existence.  
- This endpoint is **[AllowAnonymous]** to avoid JWT dependency in backend-to-backend calls.  
- Redis caches positive (1) and negative (0) existence results for efficiency.

---

## 🏗️ Architecture Overview

### Layers

Each microservice follows Clean Architecture principles:

