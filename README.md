# Going Green Project

![MIT License](https://img.shields.io/badge/License-MIT-green)

**Going Green** is an example .NET microservices solution showcasing a well-architected microservice architecture with Azure Container Apps deployment. The idea is inspired by Neal Ford’s Going Green architecture kata ([https://nealford.com/katas/kata?id=GoingGreen](https://nealford.com/katas/kata?id=GoingGreen)), this project demonstrates best practices with .NET 9 and .NET Aspire, and is still a work in progress.

## Table of Contents

- [About](#about)
- [Architecture](#architecture)
- [Features](#features)
- [Technologies](#technologies)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Infrastructure](#infrastructure)
- [CI/CD](#cicd)
- [Project Structure](#project-structure)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

## About

Going Green demonstrates how to build, deploy, and manage containerized microservices on Azure Container Apps, following Microsoft’s Well-Architected Framework.

## Architecture

Below is the high-level architecture diagram for the solution:

![Architecture Diagram](./docs/architecture.png)

## Features

- **Containerized Microservices**: Developed in C#/.NET 9
- **Serverless Containers**: Deployed on Azure Container Apps
- **Infrastructure as Code**: Managed with Terraform
- **CI/CD Pipelines**: Automated via GitHub Actions
- **Monitoring & Logging**: Integrated with Azure Monitor and Log Analytics

## Technologies

- [.NET 9 SDK](https://dotnet.microsoft.com/)
- [.NET Aspire](https://github.com/dotnet-architecture/aspire)
- [Azure Container Apps](https://azure.microsoft.com/en-us/services/container-apps/)
- [Terraform](https://www.terraform.io/)
- [GitHub Actions](https://github.com/features/actions)
- [Azure CLI](https://docs.microsoft.com/cli/azure/)

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [.NET Aspire tools](https://github.com/dotnet-architecture/aspire)
- [Azure CLI](https://docs.microsoft.com/cli/azure/install-azure-cli)
- [Terraform](https://www.terraform.io/downloads.html)
- Git

## Getting Started

1. **Clone the repository**

   ```bash
   git clone https://github.com/makigjuro/going-green.git
   cd going-green
