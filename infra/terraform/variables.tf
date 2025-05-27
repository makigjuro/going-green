variable "resource_group_name" {
  default = "going-green-rg"
}

variable "administrator_password" {
  description = "Admin password for PostgreSQL server"
  type        = string
  sensitive   = true
}

variable "location" {
  type        = string
  default     = "westeurope"
  description = "Which Azure region should be used?"
}

variable "ghcr_owner" {
  description = "GitHub org or user owning the packages"
  type        = string
}

variable "ghcr_username" {
  description = "GitHub username (or machine user) for GHCR auth"
  type        = string
}

variable "ghcr_token" {
  description = "GitHub PAT with write:packages scope"
  type        = string
  sensitive   = true
}
