output "fqdn" {
  value = azurerm_postgresql_flexible_server.this.fqdn
}

output "username" {
  value = azurerm_postgresql_flexible_server.this.administrator_login
}

output "id" {
  value = azurerm_postgresql_flexible_server.this.id
}