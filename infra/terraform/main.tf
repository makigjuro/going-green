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

resource "azurerm_log_analytics_workspace" "main" {
  name                = "goinggreen-log-analytics"
  location            = var.location
  resource_group_name = var.resource_group_name
  sku                 = "PerGB2018"
  retention_in_days   = 30
}

resource "azurerm_container_app_environment" "main" {
  name                       = "goinggreen-container-app-env"
  location                   = var.location
  resource_group_name        = var.resource_group_name
  log_analytics_workspace_id = azurerm_log_analytics_workspace.main.id
}

module "postgresql" {
  source                 = "./modules/postgresql"
  name                   = "goinggreenpggsql"
  resource_group_name    = azurerm_resource_group.main.name
  location               = azurerm_resource_group.main.location
  administrator_password = var.administrator_password
}

# module container_registry {
#   source                 = "./modules/container_registry"
#   resource_group_name    = azurerm_resource_group.main.name
#   location               = azurerm_resource_group.main.location
# }

module "gateway" {
  source = "./modules/container_app"

  name                         = "gateway"
  resource_group_name          = azurerm_resource_group.main.name
  container_app_environment_id = azurerm_container_app_environment.main.id

  image                        =  "${module.container_registry.acr_login_server}/gateway:latest"
  registry_server              = module.container_registry.acr_login_server
  registry_username            = module.container_registry.acr_username
  registry_password            = module.container_registry.acr_password

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
  image                        =  "${module.container_registry.acr_login_server}/payment-api:latest"
  registry_server              = module.container_registry.acr_login_server
  registry_username            = module.container_registry.acr_username
  registry_password            = module.container_registry.acr_password
}

module "customer_api" {
  source                       = "./modules/container_app"
  name                         = "customer-api"
  resource_group_name          = azurerm_resource_group.main.name
  container_app_environment_id = azurerm_container_app_environment.main.id
  image                        =  "${module.container_registry.acr_login_server}/customer-api:latest"
  registry_server              = module.container_registry.acr_login_server
  registry_username            = module.container_registry.acr_username
  registry_password            = module.container_registry.acr_password
}

module "quote_api" {
  source                       = "./modules/container_app"
  name                         = "quote-api"
  resource_group_name          = azurerm_resource_group.main.name
  container_app_environment_id = azurerm_container_app_environment.main.id
  image                        =  "${module.container_registry.acr_login_server}/quote-api:latest"
  registry_server              = module.container_registry.acr_login_server
  registry_username            = module.container_registry.acr_username
  registry_password            = module.container_registry.acr_password
}

module "shipping_api" {
  source                       = "./modules/container_app"
  name                         = "shipping-api"
  resource_group_name          = azurerm_resource_group.main.name
  container_app_environment_id = azurerm_container_app_environment.main.id
  image                        =  "${module.container_registry.acr_login_server}/shipping-api:latest"
  registry_server              = module.container_registry.acr_login_server
  registry_username            = module.container_registry.acr_username
  registry_password            = module.container_registry.acr_password
}

module "device_registry_api" {
  source                       = "./modules/container_app"
  name                         = "device-registry-api"
  resource_group_name          = azurerm_resource_group.main.name
  container_app_environment_id = azurerm_container_app_environment.main.id
  image                        =  "${module.container_registry.acr_login_server}/device-registry-api:latest"
  registry_server              = module.container_registry.acr_login_server
  registry_username            = module.container_registry.acr_username
  registry_password            = module.container_registry.acr_password
}