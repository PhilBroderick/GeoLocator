resource "azurerm_resource_group" "rg" {
  location = var.location
  name     = var.rg_name
}

resource "azurerm_app_service_plan" "appserviceplan" {
  name                = var.app_service_plan_name
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name

  kind = "Linux"

  sku {
    tier = "Basic"
    size = "B1"
  }

  reserved = true # Mandatory for linux
}

resource "azurerm_app_service" "appservice" {
  name                = var.app_service_name
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  app_service_plan_id = azurerm_app_service_plan.appserviceplan.id

  site_config {
    dotnet_framework_version = "v6.0"
    app_command_line         = "dotnet GeoLocator.Web.dll"
  }

  identity {
    type = "SystemAssigned"
  }

  app_settings = {
    "UseOnlyInMemoryDatabase" = "false"
    "IpStack__AccessKey": "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault.kv.vault_uri}secrets/ip-stack-access-key)"
  }

  connection_string {
    name  = "GeoLocatorConnection"
    type  = "SQLServer"
    value = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault.kv.vault_uri}secrets/geo-locator-connection-string)"
  }
}

data "azurerm_client_config" "current" {}

resource "azurerm_key_vault" "kv" {
  name                       = var.kv_name
  depends_on                 = [azurerm_resource_group.rg]
  resource_group_name        = azurerm_resource_group.rg.name
  location                   = azurerm_resource_group.rg.location
  tenant_id                  = data.azurerm_client_config.current.tenant_id 
  soft_delete_retention_days = 7
  sku_name                   = "standard"
}

resource "azurerm_key_vault_access_policy" "read_access_policy" {

  key_vault_id = azurerm_key_vault.kv.id
  tenant_id    = data.azurerm_client_config.current.tenant_id 
  object_id    = azurerm_app_service.appservice.identity.0.principal_id

  key_permissions = [
    "Get",
  ]

  secret_permissions = [
    "Get",
    "List"
  ]

  certificate_permissions = [
    "Get"
  ]
}