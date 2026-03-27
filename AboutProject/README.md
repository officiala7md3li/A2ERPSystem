# A2ERP — Modular Invoicing ERP System

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com)
[![Clean Architecture](https://img.shields.io/badge/Architecture-Clean-blue)](https://github.com/officiala7md3li/A2ERPSystem)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED)](docker-compose.yml)

A production-grade, modular **Invoicing ERP System** built on **.NET 8** with Clean Architecture, CQRS, and Domain-Driven Design. Designed for multi-tenant deployment with configurable engines for tax, discount, journaling, and invoicing orchestration.

---

## Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Prerequisites](#prerequisites)
- [Quick Start](#quick-start)
- [Environment Setup](#environment-setup)
- [Running the Project](#running-the-project)
- [Migrations](#migrations)
- [Testing](#testing)
- [Module Documentation](#module-documentation)
- [Contribution Guidelines](#contribution-guidelines)

---

## Overview

A2ERP is built around a core invoicing pipeline that coordinates specialized engines:

| Engine | Responsibility |
|--------|---------------|
| **Discount Engine** | Line-level + Invoice-level discounts, Promo Codes, Stacking |
| **Tax Engine** | Dependency-graph-based tax calculation (inclusive/exclusive) |
| **Validation Engine** | Delegate-based async validation |
| **Sequence Engine** | Concurrency-safe document numbering |
| **Journals Engine** | GL posting and double-entry accounting |
| **Invoicing Orchestrator** | Configurable pipeline coordinating all engines |
| **Installment Engine** | Multi-tranche installment plans |

---

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                        A2ERP System                         │
├──────────────────┬──────────────────┬───────────────────────┤
│   Presentation   │   Application    │       Domain          │
│  (API + Angular) │  (CQRS/MediatR)  │  (Entities/Engines)   │
├──────────────────┴──────────────────┴───────────────────────┤
│                     Infrastructure                           │
│          (EF Core + Dapper + Redis + RabbitMQ)              │
└─────────────────────────────────────────────────────────────┘
```

**Patterns used:**
- Clean Architecture (4 layers)
- CQRS with MediatR
- Repository + Unit of Work
- Outbox Pattern (event reliability)
- Decorator Pattern (caching)
- Specification Pattern (queries)
- Rich Domain Model + Aggregates

---

## Project Structure

```
src/
├── DomainDrivenERP.Domain/          # Core domain — entities, value objects, interfaces
├── DomainDrivenERP.Application/     # Use cases — commands, queries, handlers, DTOs
├── DomainDrivenERP.Persistence/     # EF Core, Dapper, Redis, repositories
├── DomainDrivenERP.Infrastructure/  # Email, localization, external services
├── DomainDrivenERP.Identity/        # ASP.NET Identity, JWT, roles, claims
├── DomainDrivenERP.Presentation/    # API controllers, middleware
├── DomainDrivenERP.API/             # Entry point, DI composition, Serilog
├── DomainDrivenERP.Web/             # MVC frontend (legacy/admin panel)
└── test/
    └── DomainDrivenERP.Application.UnitTests/
```

---

## Prerequisites

| Tool | Version | Notes |
|------|---------|-------|
| .NET SDK | 8.0+ | [Download](https://dotnet.microsoft.com/download) |
| SQL Server | 2019+ | Or use Docker |
| Redis | 6.0+ | Optional (falls back to in-memory) |
| Docker Desktop | Latest | For containerized dev |
| Node.js | 18+ | For Angular frontend |

---

## Quick Start

```bash
# 1. Clone
git clone https://github.com/officiala7md3li/A2ERPSystem.git
cd A2ERPSystem/src

# 2. Copy environment file
cp .env.example .env
# Edit .env with your connection strings

# 3. Start infrastructure
docker-compose up -d sqlserver redis seq

# 4. Apply migrations
dotnet ef database update --project DomainDrivenERP.Persistence --startup-project DomainDrivenERP.API
dotnet ef database update --project DomainDrivenERP.Identity    --startup-project DomainDrivenERP.API

# 5. Run the API
dotnet run --project DomainDrivenERP.API

# 6. Open Swagger
# https://localhost:7124/swagger
```

---

## Environment Setup

Copy `.env.example` to `.env` and fill in your values:

```bash
cp .env.example .env
```

Key variables:

| Variable | Description | Example |
|----------|-------------|---------|
| `SQLSERVER_CONNECTION` | Main DB connection string | `Server=.;Database=ERPSystemDB;...` |
| `IDENTITY_CONNECTION` | Identity DB connection string | `Server=.;Database=ERPSystemIdentityDB;...` |
| `REDIS_CONNECTION` | Redis connection string | `localhost:6379` |
| `JWT_KEY` | JWT signing key (min 32 chars) | `your-256-bit-secret` |
| `JWT_ISSUER` | JWT issuer URL | `https://localhost:7124/` |
| `EMAIL_HOST` | SMTP host | `smtp.gmail.com` |
| `EMAIL_PASSWORD` | SMTP password/app password | `xxxx xxxx xxxx xxxx` |

> ⚠️ **Never commit `.env` or `appsettings.Development.json` with real secrets.**

---

## Running the Project

### Development (Local)

```bash
# API only
dotnet run --project src/DomainDrivenERP.API

# Full stack with Docker
docker-compose up --build
```

### Docker Compose Services

| Service | Port | Purpose |
|---------|------|---------|
| `api` | 8080/8081 | .NET API |
| `sqlserver` | 14330 | SQL Server 2019 |
| `redis` | 6379 | Caching |
| `seq` | 5341 | Structured log viewer |

---

## Migrations

```bash
# Main application DB
dotnet ef migrations add <MigrationName> \
  --project src/DomainDrivenERP.Persistence \
  --startup-project src/DomainDrivenERP.API

dotnet ef database update \
  --project src/DomainDrivenERP.Persistence \
  --startup-project src/DomainDrivenERP.API

# Identity DB
dotnet ef migrations add <MigrationName> \
  --project src/DomainDrivenERP.Identity \
  --startup-project src/DomainDrivenERP.API \
  --context IdentityDbContext

dotnet ef database update \
  --project src/DomainDrivenERP.Identity \
  --startup-project src/DomainDrivenERP.API \
  --context IdentityDbContext
```

---

## Testing

```bash
# Unit tests
dotnet test src/test/DomainDrivenERP.Application.UnitTests

# With coverage
dotnet test --collect:"XPlat Code Coverage" \
  src/test/DomainDrivenERP.Application.UnitTests

# Benchmarks
dotnet run --project src/benchmark/DomainDrivenERP.RepositoriesPerformance -c Release
```

---

## Module Documentation

| Module | README |
|--------|--------|
| Domain Layer | [docs/modules/domain.md](docs/modules/domain.md) |
| Application Layer | [docs/modules/application.md](docs/modules/application.md) |
| Persistence Layer | [docs/modules/persistence.md](docs/modules/persistence.md) |
| Identity & Auth | [docs/modules/identity.md](docs/modules/identity.md) |
| Invoice Redesign | [docs/modules/invoice-redesign.md](docs/modules/invoice-redesign.md) |
| Tax Engine | [docs/modules/tax-engine.md](docs/modules/tax-engine.md) |
| Discount Engine | [docs/modules/discount-engine.md](docs/modules/discount-engine.md) |
| Localization | [docs/modules/localization.md](docs/modules/localization.md) |

---

## Contribution Guidelines

1. **Branch naming:** `feature/`, `fix/`, `refactor/`, `docs/`
2. **Commit style:** Conventional Commits (`feat:`, `fix:`, `docs:`, `refactor:`)
3. **PR requirements:**
   - All tests pass
   - No compiler warnings
   - Updated module README if applicable
   - Migration included if schema changed
4. **Code style:** Follow existing patterns (Result Pattern, Guard Clauses, Rich Domain)

```bash
# Before pushing
dotnet build src/DomainDrivenERP.sln
dotnet test src/test/DomainDrivenERP.Application.UnitTests
```

---

*Last updated: March 2026 | Version: 1.0.0-alpha*
