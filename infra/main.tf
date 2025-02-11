terraform {
    required_version = ">= 1.0"

    required_providers {
        azurerm = {
        source  = "hashicorp/azurerm"
        version = "~> 3.0"  
        }
    }
}

provider "azurerm" {
    features {} 
}

resource "azurerm_resource_group" "rg" {
    name = "clone-havoc"
    location = "Poland Central"
}

resource "azurerm_container_registry" "acr" {
    resource_group_name = azurerm_resource_group.rg.name
    location = azurerm_resource_group.rg.location
    sku = "Basic"
    admin_enabled = true
    name = "cloneHavocAcr"
}

resource "azurerm_service_plan" "asp" {
    location = azurerm_resource_group.rg.location
    resource_group_name = azurerm_resource_group.rg.name
    name = "clone_havoc_plan"
    os_type = "Linux"
    sku_name = "F1"
}

resource "azurerm_linux_web_app" "spa" {
    name                = "clone-havoc-spa"
    location            = azurerm_resource_group.rg.location
    resource_group_name = azurerm_resource_group.rg.name
    service_plan_id = azurerm_service_plan.asp.id
    site_config {   always_on = false   }
}

resource "azurerm_linux_web_app" "api" {
    name                = "clone-havoc-api"
    location            = azurerm_resource_group.rg.location
    resource_group_name = azurerm_resource_group.rg.name
    service_plan_id = azurerm_service_plan.asp.id

    site_config {
        cors {
            allowed_origins = [azurerm_linux_web_app.spa.default_hostname]
        }
        always_on = false  
    }
}

resource "azurerm_mssql_server" "server" {
    name                         = "clone-havoc-sqlserver"
    resource_group_name          = azurerm_resource_group.rg.name
    location                     = azurerm_resource_group.rg.location
    version                      = "12.0"
    administrator_login          = "clone-havoc-admin"
    administrator_login_password = "Alamakota123"
}

resource "azurerm_mssql_database" "db" {
    name         = "clone-havoc-db"
    server_id    = azurerm_mssql_server.server.id
    collation    = "SQL_Latin1_General_CP1_CI_AS"
    license_type = "LicenseIncluded"
    max_size_gb  = 2
    sku_name = "Basic"
    storage_account_type = "Local"
    geo_backup_enabled = false
  # prevent the possibility of accidental data loss
    lifecycle {
        prevent_destroy = true
    }
}

