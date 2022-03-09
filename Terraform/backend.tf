terraform {
  backend "azurerm" {
    resource_group_name  = "terraformstaterg"
    storage_account_name = "terraformstatepb"
    container_name       = "geolocator"
    key                  = "geolocator.terraform.tfstate"
  }
}