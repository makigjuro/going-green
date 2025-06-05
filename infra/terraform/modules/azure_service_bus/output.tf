output "servicebus_namespace_name" {
  value = azurerm_servicebus_namespace.main.name
}

output "quote_topic_name" {
  value = azurerm_servicebus_topic.quote_topic.name
}

output "ship_topic_name" {
  value = azurerm_servicebus_topic.ship_topic.name
}

output "assess_topic_name" {
  value = azurerm_servicebus_topic.assess_topic.name
}