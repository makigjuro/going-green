output "container_app_environment_id" {
  value = azurerm_container_app_environment.main.id
}

output "assessment_api_url" {
  value = module.assessment_api.fqdn
}
