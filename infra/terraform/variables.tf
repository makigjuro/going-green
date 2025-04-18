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