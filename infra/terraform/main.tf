terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "4.26.0"
    }
  }

  required_version = ">= 1.3.0"
}

provider "azurerm" {
  features {}
}

# Resource Group
resource "azurerm_resource_group" "main" {
  name     = var.resource_group_name
  location = var.location
}

module "log_analytics_workspace"  {
  source                 = "./modules/log_analytics"
  name                   = "goinggreen-log-analytics"
  resource_group_name    = azurerm_resource_group.main.name
  location               = azurerm_resource_group.main.location
}

module "postgresql" {
  source                 = "./modules/postgresql"
  name                   = "goinggreenpggsql"
  resource_group_name    = azurerm_resource_group.main.name
  location               = azurerm_resource_group.main.location
  administrator_password = var.administrator_password
}

module "keyvault" {
  source                 = "./modules/azure_key_vault"
  resource_group_name    = azurerm_resource_group.main.name
  location               = azurerm_resource_group.main.location
}

module "service_bus" {
  source                 = "./modules/azure_service_bus"
  resource_group_name    = azurerm_resource_group.main.name
  location               = azurerm_resource_group.main.location
}

locals {
  ghcr_server = "ghcr.io/${var.ghcr_owner}"
}

resource "azurerm_container_app_environment" "main" {
  name                       = "goinggreen-container-app-env"
  location                   = var.location
  resource_group_name        = var.resource_group_name
  log_analytics_workspace_id = module.log_analytics_workspace.log_analytics_workspace_id
}

module "gateway" {
  source = "./modules/container_app"

  name                         = "gateway"
  resource_group_name          = azurerm_resource_group.main.name
  container_app_environment_id = azurerm_container_app_environment.main.id
  key_vault_id                 = module.keyvault.key_vault_id  
             
  # pull image from GitHub Container Registry 
  image               = "${local.ghcr_server}/gateway:latest"
  registry_server     = local.ghcr_server
  registry_username   = var.ghcr_username
  registry_password   = var.ghcr_token

  # Gateway scaling configuration - higher thresholds as it's a proxy
  cpu                         = 0.5
  memory                      = 1.0
  min_replicas               = 2
  max_replicas               = 20
  http_concurrent_requests   = 100
  cpu_percentage_threshold   = 80
  memory_percentage_threshold = 80

  env_variables = {
    "ASPNETCORE_ENVIRONMENT" = "Production"
    # Add URLs or service discovery details for downstream microservices
  }

  expose_public_ingress = true  
}

module "payment_api" {
  source                       = "./modules/container_app"
  name                         = "payment-api"
  resource_group_name          = azurerm_resource_group.main.name
  container_app_environment_id = azurerm_container_app_environment.main.id
  key_vault_id                 = module.keyvault.key_vault_id

  # pull image from GitHub Container Registry 
  image               = "${local.ghcr_server}/payment-api:latest"
  registry_server     = local.ghcr_server
  registry_username   = var.ghcr_username
  registry_password   = var.ghcr_token

  # Payment API scaling - critical service, needs high availability
  cpu                         = 0.75
  memory                      = 1.5
  min_replicas               = 2
  max_replicas               = 12
  http_concurrent_requests   = 20
  cpu_percentage_threshold   = 60
  memory_percentage_threshold = 65

  # Service Bus scaling for payment processing
  scaling_rules = [
    {
      name = "payment-processing-queue"
      type = "azure-servicebus"
      metadata = {
        "queueName"     = "payment-processing"
        "messageCount"  = "5"
        "connectionFromEnv" = "SERVICEBUS_CONNECTION_STRING"
      }
      auth = null
    }
  ]
}

module "customer_api" {
  source                       = "./modules/container_app"
  name                         = "customer-api"
  resource_group_name          = azurerm_resource_group.main.name
  container_app_environment_id = azurerm_container_app_environment.main.id
  key_vault_id                 = module.keyvault.key_vault_id

  # pull image from GitHub Container Registry 
  image               = "${local.ghcr_server}/customer-api:latest"
  registry_server     = local.ghcr_server
  registry_username   = var.ghcr_username
  registry_password   = var.ghcr_token

  # Customer API scaling - moderate traffic, read-heavy workload
  cpu                         = 0.5
  memory                      = 1.0
  min_replicas               = 1
  max_replicas               = 8
  http_concurrent_requests   = 40
  cpu_percentage_threshold   = 70
  memory_percentage_threshold = 75

  # Service Bus scaling for customer events
  scaling_rules = [
    {
      name = "customer-events-queue"
      type = "azure-servicebus"
      metadata = {
        "queueName"     = "customer-events"
        "messageCount"  = "8"
        "connectionFromEnv" = "SERVICEBUS_CONNECTION_STRING"
      }
      auth = null
    }
  ]
}

module "quote_api" {
  source                       = "./modules/container_app"
  name                         = "quote-api"
  resource_group_name          = azurerm_resource_group.main.name
  container_app_environment_id = azurerm_container_app_environment.main.id
  key_vault_id                 = module.keyvault.key_vault_id

  # pull image from GitHub Container Registry 
  image               = "${local.ghcr_server}/quote-api:latest"
  registry_server     = local.ghcr_server
  registry_username   = var.ghcr_username
  registry_password   = var.ghcr_token

  # Quote API scaling - high traffic expected for quotes
  cpu                         = 0.75
  memory                      = 1.5
  min_replicas               = 2
  max_replicas               = 15
  http_concurrent_requests   = 30
  cpu_percentage_threshold   = 65
  memory_percentage_threshold = 70

  # Service Bus scaling for quote events
  scaling_rules = [
    {
      name = "quote-events-queue"
      type = "azure-servicebus"
      metadata = {
        "queueName"     = "quote-events"
        "messageCount"  = "5"
        "connectionFromEnv" = "SERVICEBUS_CONNECTION_STRING"
      }
      auth = null
    }
  ]
}

module "shipping_api" {
  source                       = "./modules/container_app"
  name                         = "shipping-api"
  resource_group_name          = azurerm_resource_group.main.name
  container_app_environment_id = azurerm_container_app_environment.main.id
  key_vault_id                 = module.keyvault.key_vault_id

  # pull image from GitHub Container Registry 
  image               = "${local.ghcr_server}/shipping-api:latest"
  registry_server     = local.ghcr_server
  registry_username   = var.ghcr_username
  registry_password   = var.ghcr_token

  # Shipping API scaling - moderate traffic but processing-intensive
  cpu                         = 1.0
  memory                      = 2.0
  min_replicas               = 1
  max_replicas               = 10
  http_concurrent_requests   = 25
  cpu_percentage_threshold   = 60
  memory_percentage_threshold = 65

  # Service Bus scaling for shipping events and package delivery
  scaling_rules = [
    {
      name = "shipping-events-queue"
      type = "azure-servicebus"
      metadata = {
        "queueName"     = "shipping-events"
        "messageCount"  = "3"
        "connectionFromEnv" = "SERVICEBUS_CONNECTION_STRING"
      }
      auth = null
    }
  ]
}

module "device_registry_api" {
  source                       = "./modules/container_app"
  name                         = "device-registry-api"
  resource_group_name          = azurerm_resource_group.main.name
  container_app_environment_id = azurerm_container_app_environment.main.id
  key_vault_id                 = module.keyvault.key_vault_id

  # pull image from GitHub Container Registry 
  image               = "${local.ghcr_server}/device-registry-api:latest"
  registry_server     = local.ghcr_server
  registry_username   = var.ghcr_username
  registry_password   = var.ghcr_token

  # Device Registry API scaling - catalog service, mostly read operations
  cpu                         = 0.5
  memory                      = 1.0
  min_replicas               = 1
  max_replicas               = 6
  http_concurrent_requests   = 60
  cpu_percentage_threshold   = 75
  memory_percentage_threshold = 80

  # Service Bus scaling for device catalog updates
  scaling_rules = [
    {
      name = "device-catalog-updates"
      type = "azure-servicebus"
      metadata = {
        "queueName"     = "device-catalog-updates"
        "messageCount"  = "10"
        "connectionFromEnv" = "SERVICEBUS_CONNECTION_STRING"
      }
      auth = null
    }
  ]
}

module "assessment_api" {
  source                       = "./modules/container_app"
  name                         = "assessment-api"
  resource_group_name          = azurerm_resource_group.main.name
  container_app_environment_id = azurerm_container_app_environment.main.id
  key_vault_id                 = module.keyvault.key_vault_id

  # pull image from GitHub Container Registry 
  image               = "${local.ghcr_server}/assessment-api:latest"
  registry_server     = local.ghcr_server
  registry_username   = var.ghcr_username
  registry_password   = var.ghcr_token

  # Assessment API scaling - CPU intensive for device inspection and classification
  cpu                         = 1.5
  memory                      = 3.0
  min_replicas               = 1
  max_replicas               = 8
  http_concurrent_requests   = 15
  cpu_percentage_threshold   = 55
  memory_percentage_threshold = 60

  # Service Bus scaling for package delivery events from shipping
  scaling_rules = [
    {
      name = "package-delivered-queue"
      type = "azure-servicebus"
      metadata = {
        "queueName"     = "package-delivered"
        "messageCount"  = "2"
        "connectionFromEnv" = "SERVICEBUS_CONNECTION_STRING"
      }
      auth = null
    }
  ]
}