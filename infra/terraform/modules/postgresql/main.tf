resource "azurerm_postgresql_flexible_server" "this" {
  name                   = var.name
  resource_group_name    = var.resource_group_name
  location               = var.location
  version                = var.postgresql_version
  administrator_login    = var.administrator_login
  administrator_password = var.administrator_password
  storage_mb             = var.storage_mb
  storage_tier           = var.storage_tier
  sku_name               = var.sku_name
  zone                   = var.zone
  public_network_access_enabled = false
}

resource "azurerm_postgresql_flexible_server_database" "this" {
  name      = var.database_name
  server_id = azurerm_postgresql_flexible_server.this.id
  collation = "en_US.utf8"
  charset   = "UTF8"
}

resource "azurerm_postgresql_flexible_server_configuration" "disable_ssl" {
  name      = "require_secure_transport"
  value     = "OFF"
  server_id = azurerm_postgresql_flexible_server.this.id
}
