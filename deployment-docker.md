# Docker Deployment

## Overview

The entire E-Commerce stack runs in Docker with three services:

| Service | Image | Port | Purpose |
|---|---|---|---|
| `sqlserver` | mcr.microsoft.com/mssql/server:2022-latest | 1433 | Database |
| `redis` | redis:7-alpine | 6379 | Distributed cache |
| `webapi` | built from `src/ECommerce.WebApi/Dockerfile` | 7007 → 8080 | REST API |

All three are connected via a bridge network called `ecommerce-network`.

## Prerequisites

- Docker Desktop for Windows (with Linux containers)
- At least 4 GB RAM allocated to Docker

## Run with One Command

```bash
cd 01-AspNetCoreRestApiECommerce
docker compose up --build