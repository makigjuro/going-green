resource "azurerm_container_app" "this" {
  name                         = var.name
  container_app_environment_id = var.container_app_environment_id
  resource_group_name          = var.resource_group_name
  revision_mode                = "Single"

  identity {
    type = "SystemAssigned"
  }

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

    max_replicas = var.max_replicas
    min_replicas = var.min_replicas
    
    # HTTP-based scaling rule
    http_scale_rule {
      name                = "http-requests"
      concurrent_requests = var.http_concurrent_requests
    }

    # CPU-based scaling rule
    custom_scale_rule {
      name             = "cpu-utilization"
      custom_rule_type = "cpu"
      metadata = {
        "type"  = "Utilization"
        "value" = tostring(var.cpu_percentage_threshold)
      }
    }

    # Memory-based scaling rule
    custom_scale_rule {
      name             = "memory-utilization"
      custom_rule_type = "memory"
      metadata = {
        "type"  = "Utilization"
        "value" = tostring(var.memory_percentage_threshold)
      }
    }

    # Dynamic custom scaling rules (e.g., Service Bus, custom metrics)
    dynamic "custom_scale_rule" {
      for_each = var.scaling_rules
      content {
        name             = custom_scale_rule.value.name
        custom_rule_type = custom_scale_rule.value.type
        metadata         = custom_scale_rule.value.metadata
        
        dynamic "authentication" {
          for_each = custom_scale_rule.value.auth != null ? custom_scale_rule.value.auth : []
          content {
            secret_name       = authentication.value.secret_ref
            trigger_parameter = authentication.value.trigger_parameter
          }
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
    external_enabled = var.expose_public_ingress
    target_port      = 8080
    transport        = "auto"
    traffic_weight {
      percentage      = 100
      latest_revision = true
    }
  }

}

resource "azurerm_role_assignment" "keyvault_reader" {
  scope                = var.key_vault_id
  role_definition_name = "Key Vault Secrets User"
  principal_id         = azurerm_container_app.this.identity[0].principal_id
}
