resource "azurerm_container_app" "this" {
  name                         = var.name
  container_app_environment_id = var.container_app_environment_id
  resource_group_name          = var.resource_group_name
  revision_mode                = "Single"

  template {
    container {
      name   = var.name
      image  = var.image
      cpu    = var.cpu
      memory = "${var.memory}Gi"

      dynamic "env" {
        for_each = var.env_variables
        content {
          name  = env.key
          value = env.value
        }
      }
    }
  }

  registry {
    server   = var.registry_server
    username = var.registry_username
    password_secret_name = "registry-password"
  }

  secret {
    name  = "registry-password"
    value = var.registry_password
  }

  ingress {
    external_enabled = true
    target_port      = 80
    traffic_weight {
      percentage      = 100
      latest_revision = true
    }
  }
}
