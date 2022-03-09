variable "rg_name" {
  description = "The name of the resource group in which to host the resources"
  type        = string
}

variable "location" {
  description = "The location in which to host the resources"
  type        = string
  default     = "uksouth"
}

variable "app_service_plan_name" {
  description = "The name for the app service plan"
  type        = string
}

variable "app_service_name" {
  description = "The name for the app service"
  type        = string
}

variable "kv_name" {
  description = "The name for the key vault"
  type        = string
}