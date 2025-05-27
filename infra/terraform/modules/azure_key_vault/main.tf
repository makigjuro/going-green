resource "azurerm_key_vault" "main" {
  name                        = "goinggreen-keyvault-${random_integer.suffix.result}"
  location                    = var.location
  resource_group_name         = var.resource_group_name
  tenant_id                   = data.azurerm_client_config.current.tenant_id
  sku_name                    = "standard"
  purge_protection_enabled    = true
  soft_delete_retention_days  = 7

  # IMPORTANT: No access policies for developers, use RBAC instead
  enable_rbac_authorization   = true
}

resource "random_integer" "suffix" {
  min = 10000
  max = 99999
}

data "azurerm_client_config" "current" {}

output "key_vault_id" {
  value = azurerm_key_vault.main.id
}