variable "name" {
  description = "Name of the container app"
  type        = string
}
variable "resource_group_name" {
  description = "Name of the resource group"
  type        = string
}

variable "image" {
  description = "Container image to deploy"
  type        = string
}
variable "registry_server" {
  description = "Container registry server (e.g., myregistry.azurecr.io)"
  type        = string
}

variable "container_app_environment_id" {
  description = "ID of the container apps environment"
  type        = string
}

variable "registry_username" {
  description = "Username for container registry"
  type        = string
}

variable "registry_password" {
  description = "Password for container registry"
  type        = string
  sensitive   = true
}
variable "cpu" {
  description = "CPU allocation in vCPU"
  type        = number
  default     = 0.5
}

variable "memory" {
  description = "Memory allocation in GiB"
  type        = number
  default     = 1.0
}

variable "max_replicas" {
  description = "Max Number of Containers that can run in parallel"
  type        = number
  default     = 5  
}

variable "min_replicas" {
  description = "Min Number of Containers that can run"
  type        = number
  default     = 1  
}

variable "env_variables" {
  description = "Environment variables for the container app"
  type = map(string)
  default = {}
}

variable "expose_public_ingress" {
  type    = bool
  default = false
}

variable "key_vault_id" {
  description = "key vault id where data is storred"
  type        = string
}

variable "scaling_rules" {
  description = "Custom scaling rules for the container app"
  type = list(object({
    name = string
    type = string
    metadata = map(string)
    auth = optional(list(object({
      secret_ref        = string
      trigger_parameter = string
    })))
  }))
  default = []
}

variable "http_concurrent_requests" {
  description = "Number of concurrent HTTP requests before scaling up"
  type        = number
  default     = 50
}

variable "cpu_percentage_threshold" {
  description = "CPU percentage threshold for scaling"
  type        = number
  default     = 70
}

variable "memory_percentage_threshold" {
  description = "Memory percentage threshold for scaling"
  type        = number
  default     = 70
}