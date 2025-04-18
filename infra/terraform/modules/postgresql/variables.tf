variable "name" {
  description = "Name of the PostgreSQL server"
  type        = string
}

variable "resource_group_name" {
  description = "Resource group name"
  type        = string
}

variable "location" {
  description = "Azure region"
  type        = string
}

variable "administrator_login" {
  description = "Admin username for PostgreSQL"
  type        = string
  default     = "pgadmin"
}

variable "administrator_password" {
  description = "Admin password"
  type        = string
  sensitive   = true
}

variable "postgresql_version" {
  description = "PostgreSQL version"
  type        = string
  default     = "16"
}

variable "storage_mb" {
  description = "Storage size in MB"
  type        = number
  default     = 32768
}


variable "storage_tier" {
  description = "Storage size in MB"
  type        = string
  default     = "P4"
}
variable "sku_name" {
  description = "SKU name"
  type        = string
  default     = "B_Standard_B1ms"
}

variable "zone" {
  description = "Availability zone"
  type        = string
  default     = "1"
}

variable "database_name" {
  description = "Name of the database to create"
  type        = string
  default     = "goinggreendb"
}