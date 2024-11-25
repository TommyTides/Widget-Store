// Parameters with defaults based on resource group
param location string = 'northeurope'
param prefix string = 'widgetstore'
param uniqueSuffix string = uniqueString(resourceGroup().id)

// Security parameters with default secure values
@secure()
param jwtSecretKey string = newGuid()
@secure()
param adminSecretKey string = newGuid()

// Names for resources
var storageAccountName = '${prefix}${uniqueSuffix}'
var cosmosDatabaseName = 'WidgetStore'
var cosmosContainerName = 'Store'
var webAppName = '${prefix}-api-${uniqueSuffix}'
var functionAppName = '${prefix}-func-${uniqueSuffix}'

// Storage Account
resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
    allowBlobPublicAccess: true
    publicNetworkAccess: 'Enabled'
  }
}

// Queue Service and Queues
resource queueService 'Microsoft.Storage/storageAccounts/queueServices@2023-01-01' = {
  parent: storageAccount
  name: 'default'
}

resource ordersQueue 'Microsoft.Storage/storageAccounts/queueServices/queues@2023-01-01' = {
  parent: queueService
  name: 'orders'
}

resource ordersPoisonQueue 'Microsoft.Storage/storageAccounts/queueServices/queues@2023-01-01' = {
  parent: queueService
  name: 'orders-poison'
}

// Table Service
resource tableService 'Microsoft.Storage/storageAccounts/tableServices@2023-01-01' = {
  parent: storageAccount
  name: 'default'
}

resource usersTable 'Microsoft.Storage/storageAccounts/tableServices/tables@2023-01-01' = {
  parent: tableService
  name: 'users'
}

resource reviewsTable 'Microsoft.Storage/storageAccounts/tableServices/tables@2023-01-01' = {
  parent: tableService
  name: 'reviews'
}

// Blob Service and Container
resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2023-01-01' = {
  parent: storageAccount
  name: 'default'
  properties: {
    cors: {
      corsRules: [
        {
          allowedOrigins: ['*']
          allowedMethods: ['GET']
          allowedHeaders: ['*']
          exposedHeaders: ['*']
          maxAgeInSeconds: 3600
        }
      ]
    }
  }
}

resource productImagesContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-01-01' = {
  parent: blobService
  name: 'product-images'
  properties: {
    publicAccess: 'Blob'
  }
}

// Cosmos DB Account
resource cosmosAccount 'Microsoft.DocumentDB/databaseAccounts@2023-04-15' = {
  name: '${prefix}-cosmos-${uniqueSuffix}'
  location: location
  properties: {
    databaseAccountOfferType: 'Standard'
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    locations: [
      {
        locationName: location
        failoverPriority: 0
      }
    ]
    capabilities: [
      {
        name: 'EnableServerless'
      }
    ]
  }
}

// Cosmos DB Database
resource cosmosDatabase 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2023-04-15' = {
  parent: cosmosAccount
  name: cosmosDatabaseName
  properties: {
    resource: {
      id: cosmosDatabaseName
    }
  }
}

// Cosmos DB Container
resource cosmosContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2023-04-15' = {
  parent: cosmosDatabase
  name: cosmosContainerName
  properties: {
    resource: {
      id: cosmosContainerName
      partitionKey: {
        paths: [
          '/type'
        ]
        kind: 'Hash'
      }
    }
  }
}

// Application Insights
resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: '${prefix}-insights-${uniqueSuffix}'
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Request_Source: 'rest'
  }
}

// App Service Plans (separate for Web App and Function App)
resource functionAppServicePlan 'Microsoft.Web/serverfarms@2023-01-01' = {
  name: '${prefix}-func-plan-${uniqueSuffix}'
  location: location
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
  }
}

resource webAppServicePlan 'Microsoft.Web/serverfarms@2023-01-01' = {
  name: '${prefix}-web-plan-${uniqueSuffix}'
  location: location
  sku: {
    name: 'F1'
    tier: 'Free'
  }
}

// Web App (API)
resource webApp 'Microsoft.Web/sites@2023-01-01' = {
  name: webAppName
  location: location
  properties: {
    serverFarmId: webAppServicePlan.id
    httpsOnly: true
    siteConfig: {
      netFrameworkVersion: 'v8.0'
      appSettings: [
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appInsights.properties.InstrumentationKey
        }
        {
          name: 'ApplicationInsights:ConnectionString'
          value: appInsights.properties.ConnectionString
        }
        {
          name: 'AzureStorageConfig:ConnectionString'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${storageAccount.listKeys().keys[0].value}'
        }
        {
          name: 'CosmosDbConfig:ConnectionString'
          value: cosmosAccount.listConnectionStrings().connectionStrings[0].connectionString
        }
        {
          name: 'CosmosDbConfig:DatabaseName'
          value: cosmosDatabaseName
        }
        {
          name: 'CosmosDbConfig:ContainerName'
          value: cosmosContainerName
        }
        {
          name: 'QueueStorageConfig:ConnectionString'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${storageAccount.listKeys().keys[0].value}'
        }
        {
          name: 'QueueStorageConfig:OrdersQueueName'
          value: 'orders'
        }
        {
          name: 'JwtConfig:SecretKey'
          value: jwtSecretKey
        }
        {
          name: 'JwtConfig:Issuer'
          value: 'WidgetStore'
        }
        {
          name: 'JwtConfig:Audience'
          value: 'WidgetStoreClient'
        }
        {
          name: 'AdminConfig:AdminSecretKey'
          value: adminSecretKey
        }
      ]
    }
  }
}

// Function App
resource functionApp 'Microsoft.Web/sites@2023-01-01' = {
  name: functionAppName
  location: location
  kind: 'functionapp'
  properties: {
    serverFarmId: functionAppServicePlan.id
    httpsOnly: true
    siteConfig: {
      netFrameworkVersion: 'v8.0'
      appSettings: [
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${storageAccount.listKeys().keys[0].value}'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${storageAccount.listKeys().keys[0].value}'
        }
        {
          name: 'WEBSITE_CONTENTSHARE'
          value: toLower(functionAppName)
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet-isolated'
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsights.properties.ConnectionString
        }
        {
          name: 'CosmosDbConfig:ConnectionString'
          value: cosmosAccount.listConnectionStrings().connectionStrings[0].connectionString
        }
        {
          name: 'CosmosDbConfig:DatabaseName'
          value: cosmosDatabaseName
        }
        {
          name: 'CosmosDbConfig:ContainerName'
          value: cosmosContainerName
        }
      ]
    }
  }
}

// Outputs
output webAppName string = webApp.name
output functionAppName string = functionApp.name
output storageAccountName string = storageAccount.name
output cosmosAccountName string = cosmosAccount.name
output webAppUrl string = 'https://${webApp.properties.defaultHostName}'
output functionAppUrl string = 'https://${functionApp.properties.defaultHostName}'
