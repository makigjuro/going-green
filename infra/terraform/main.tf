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
}