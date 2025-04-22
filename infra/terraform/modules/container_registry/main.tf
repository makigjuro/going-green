variable "resource_group_name" {
  description = "Name of the resource group"
  type        = string
}
variable "location" {
  description = "Azure region"
  type        = string
}

resource "random_integer" "acr_suffix" {
  min = 10000
  max = 99999
}

resource "azurerm_container_registry" "acr" {
  name                = "goinggreenacr${random_integer.acr_suffix.result}"
  resource_group_name = var.resource_group_name
  location            = var.location
  sku                 = "Basic"     
  admin_enabled       = true        
}
