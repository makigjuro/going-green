resource "azurerm_servicebus_namespace" "main" {
  name                = "goinggreen-sb-namespace"
  location            = var.location
  resource_group_name = var.resource_group_name
  sku                 = "Standard"
}
resource "azurerm_servicebus_topic" "quote_topic" {
  name         = "quote-topic"
  namespace_id = azurerm_servicebus_namespace.main.id
}

resource "azurerm_servicebus_subscription" "quote_to_ship" {
  name               = "quote-to-ship-subscription"
  topic_id           = azurerm_servicebus_topic.quote_topic.id
  max_delivery_count = 3
}

resource "azurerm_servicebus_topic" "ship_topic" {
  name         = "ship-topic"
  namespace_id = azurerm_servicebus_namespace.main.id
}

resource "azurerm_servicebus_subscription" "ship_to_assess" {
  name               = "ship-to-assess-subscription"
  topic_id           = azurerm_servicebus_topic.ship_topic.id
  max_delivery_count = 3
}

resource "azurerm_servicebus_topic" "assess_topic" {
  name         = "assess-topic"
  namespace_id = azurerm_servicebus_namespace.main.id
}

resource "azurerm_servicebus_subscription" "assess_to_pay" {
  name               = "assess-to-pay-subscription"
  topic_id           = azurerm_servicebus_topic.assess_topic.id
  max_delivery_count = 3

}
