variable "name" {
  description = "Name of the container app"
  type        = string
}

variable "resource_group_name" {
  description = "Name of the resource group"
  type        = string
}

variable "location" {
  description = "Azure region"
  type        = string
}

variable "container_app_environment_id" {
  description = "ID of the container apps environment"
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

variable "env_variables" {
  description = "Environment variables for the container app"
  type = map(string)
  default = {}
}
