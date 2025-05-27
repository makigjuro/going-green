output "appinsights_connection_string" {
  description = "Use this connection string for OpenTelemetry exporters"
  value       = azurerm_application_insights.main.connection_string
}

output "log_analytics_workspace_id" {
  description = "The ID of the shared Log Analytics Workspace"
  value       = azurerm_log_analytics_workspace.main.id
}
