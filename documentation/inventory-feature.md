# Inventory Management System

## Overview

The Inventory Management System is a comprehensive feature designed to track agricultural supplies, manage stock levels, and provide automated alerts for the Hurudza agricultural management platform. This system enables farmers and farm managers to efficiently manage their inventory of seeds, fertilizers, chemicals, fuel, equipment parts, and other agricultural supplies.

## Key Features

### 1. Comprehensive Inventory Tracking
- **Multi-category Support**: Seeds, fertilizers, chemicals, fuel, equipment parts, tools, and other supplies
- **Detailed Item Information**: Name, SKU, barcode, brand, supplier details, storage location
- **Unit Management**: Support for various units of measure (kg, liters, bags, pieces, etc.)
- **Stock Monitoring**: Real-time quantity tracking with current stock levels

### 2. Automatic Reorder Alerts
- **Minimum Stock Levels**: Configurable minimum stock thresholds per item
- **Reorder Points**: Automated alerts when stock reaches reorder levels
- **Low Stock Dashboard**: Centralized view of items requiring attention
- **Priority-based Alerts**: Critical, warning, and info level notifications

### 3. Expiry Date Management
- **Expiration Tracking**: Monitor expiry dates for chemicals, seeds, and perishable items
- **Automated Alerts**: Notifications for items approaching expiration
- **Safety Compliance**: Ensure safe use of chemicals and fertilizers
- **Waste Reduction**: Prevent inventory loss due to expired products

### 4. Transaction Management
- **Complete Transaction History**: Track all inventory movements
- **Transaction Types**: Purchase, usage, sale, return, adjustment, damage, and expiry
- **Field Integration**: Link inventory usage to specific fields and crops
- **Cost Tracking**: Monitor unit costs and total transaction values
- **Approval Workflow**: Optional approval process for high-value transactions

## Technical Implementation

### Database Schema

#### Inventory Table
- **Id**: Unique identifier (GUID)
- **Name**: Item name
- **Type**: Inventory type (enum)
- **SKU**: Stock Keeping Unit
- **Barcode**: Product barcode
- **Brand**: Manufacturer/brand name
- **Supplier**: Supplier information
- **QuantityInStock**: Current stock level
- **UnitOfMeasure**: Unit type (enum)
- **MinimumStockLevel**: Minimum threshold
- **ReorderLevel**: Reorder point
- **UnitCost**: Cost per unit
- **ExpiryDate**: Expiration date (nullable)
- **StorageLocation**: Storage area/location
- **FarmId**: Associated farm
- **Category**: Item category
- **Audit Fields**: CreatedDate, ModifiedDate, IsActive, etc.

#### InventoryTransaction Table
- **Id**: Transaction identifier (GUID)
- **InventoryId**: Reference to inventory item
- **TransactionType**: Type of transaction (enum)
- **Quantity**: Transaction quantity
- **UnitCost**: Cost per unit (nullable)
- **TotalCost**: Total transaction cost
- **TransactionDate**: When transaction occurred
- **FieldId**: Associated field (nullable)
- **FieldCropId**: Associated crop (nullable)
- **Supplier**: Transaction supplier (nullable)
- **BatchNumber**: Batch tracking (nullable)
- **Notes**: Additional notes
- **ApprovedBy**: Approver (nullable)
- **Audit Fields**: CreatedBy, CreatedDate, etc.

### Enumerations

#### InventoryType
- Seeds
- Fertilizers
- Chemicals
- Fuel
- EquipmentParts
- Tools
- Feed
- Other

#### TransactionType
- Purchase
- Usage
- Sale
- Return
- Adjustment
- Damage
- Expiry

#### UnitOfMeasure
- Kilograms
- Grams
- Liters
- Milliliters
- Bags
- Pieces
- Tons
- Gallons

### API Endpoints

#### Inventory Controller (`/api/Inventory`)
- **GET** `/GetInventory` - Get all inventory items
- **GET** `/GetInventoryDetails/{id}` - Get specific item details
- **GET** `/GetInventoryWithTransactions/{id}` - Get item with transaction history
- **GET** `/GetLowStockItems` - Get items below minimum stock
- **GET** `/GetExpiredItems` - Get expired items
- **GET** `/GetExpiringItems/{days}` - Get items expiring within days
- **GET** `/GetInventoryByFarm/{farmId}` - Get farm-specific inventory
- **POST** `/CreateInventory` - Create new inventory item
- **PUT** `/UpdateInventory/{id}` - Update inventory item
- **POST** `/CreateTransaction` - Record inventory transaction
- **DELETE** `/DeleteInventory/{id}` - Soft delete inventory item

### User Interface Components

#### 1. Main Inventory Page (`Inventory.razor`)
- **Dashboard Cards**: Quick stats (total items, low stock count, expired items)
- **Search and Filters**: Filter by type, farm, status
- **Data Grid**: Sortable, paginated inventory list
- **Action Buttons**: Add, edit, delete, view details
- **Stock Status Indicators**: Visual indicators for stock levels

#### 2. Inventory Add/Edit Pages
- **Comprehensive Form**: All inventory item properties
- **Validation**: Client and server-side validation
- **Dropdown Lists**: Type, unit of measure, farm selection
- **Date Pickers**: Expiry date selection
- **Numeric Controls**: Stock quantities and costs

#### 3. Transaction Recording (`InventoryTransaction.razor`)
- **Dynamic Form**: Adapts based on transaction type
- **Field Integration**: Link transactions to specific fields/crops
- **Cost Calculation**: Automatic total cost calculation
- **Batch Tracking**: Optional batch number recording
- **Approval Workflow**: Optional approval process

#### 4. Inventory Details Page
- **Item Information**: Complete item details
- **Transaction History**: Chronological transaction list
- **Stock Level Chart**: Visual stock level over time
- **Related Items**: Similar or related inventory items

#### 5. Alerts Dashboard (`InventoryAlerts.razor`)
- **Priority Filtering**: Critical, warning, info alerts
- **Low Stock Alerts**: Items below minimum levels
- **Expiry Alerts**: Items approaching expiration
- **Action Items**: Quick actions from alert dashboard

## Business Logic

### Stock Level Management
```csharp
// Automatic stock updates on transactions
public async Task<bool> ProcessTransactionAsync(CreateInventoryTransactionViewModel transaction)
{
    var inventory = await GetInventoryByIdAsync(transaction.InventoryId);
    
    switch (transaction.TransactionType)
    {
        case TransactionType.Purchase:
        case TransactionType.Return:
            inventory.QuantityInStock += transaction.Quantity;
            break;
        case TransactionType.Usage:
        case TransactionType.Sale:
        case TransactionType.Damage:
        case TransactionType.Expiry:
            inventory.QuantityInStock -= transaction.Quantity;
            break;
        case TransactionType.Adjustment:
            inventory.QuantityInStock = transaction.Quantity;
            break;
    }
    
    await UpdateInventoryAsync(inventory);
    return true;
}
```

### Alert Generation
- **Low Stock**: Items where `QuantityInStock <= MinimumStockLevel`
- **Reorder Alert**: Items where `QuantityInStock <= ReorderLevel`
- **Expiry Alert**: Items where `ExpiryDate <= DateTime.Now.AddDays(configurable_days)`

### Security and Permissions
- **Role-based Access**: Different permissions for different user roles
- **Farm-specific Access**: Users can only access their assigned farms' inventory
- **Transaction Approval**: High-value transactions may require approval
- **Audit Trail**: Complete audit trail for all inventory changes

## Configuration

### Application Settings
```json
{
  "InventorySettings": {
    "DefaultExpiryWarningDays": 30,
    "RequireApprovalThreshold": 1000.00,
    "DefaultMinimumStockPercentage": 10,
    "EnableBatchTracking": true
  }
}
```

### User Permissions
- **InventoryView**: View inventory items
- **InventoryManage**: Create, edit inventory items
- **InventoryTransaction**: Record transactions
- **InventoryApprove**: Approve high-value transactions
- **InventoryReports**: Access inventory reports

## Integration Points

### Field and Crop Integration
- Link inventory transactions to specific fields
- Track which crops consumed which inputs
- Generate field-specific cost reports

### Equipment Integration
- Track equipment parts inventory
- Link maintenance activities to parts usage
- Equipment-specific parts recommendations

### Reporting Integration
- Inventory cost reports by field/crop
- Stock level trend analysis
- Supplier performance reports
- Expiry and waste reports

## Future Enhancements

### Planned Features
1. **Barcode Scanning**: Mobile barcode scanning for quick inventory updates
2. **Predictive Analytics**: AI-based demand forecasting
3. **Supplier Integration**: Direct ordering from suppliers
4. **Mobile App**: Dedicated mobile inventory management
5. **Photo Management**: Item photos and storage location images
6. **Advanced Reporting**: Custom report builder
7. **Multi-location Support**: Track inventory across multiple storage locations
8. **Seasonal Planning**: Seasonal inventory planning tools

### Technical Improvements
1. **Caching Strategy**: Implement caching for frequently accessed data
2. **Bulk Operations**: Bulk import/export functionality
3. **API Versioning**: Implement API versioning for backward compatibility
4. **Performance Optimization**: Database indexing and query optimization
5. **Real-time Updates**: SignalR for real-time inventory updates

## Installation and Setup

### Database Migration
```bash
# Add migration
dotnet ef migrations add AddInventoryTables --project Data/Hurudza.Data.Context --startup-project Apis/Hurudza.Apis.Core

# Update database
dotnet ef database update --project Data/Hurudza.Data.Context --startup-project Apis/Hurudza.Apis.Core
```

### Service Registration
The inventory services are automatically registered in `Program.cs`:
```csharp
builder.Services.AddScoped<IInventoryService, InventoryService>();
```

### Permissions Setup
Ensure the following permissions are configured in the system:
- `inventory.view`
- `inventory.manage`
- `inventory.transaction`
- `inventory.approve`

## Testing

### Unit Tests
- Service layer tests for business logic
- Controller tests for API endpoints
- Validation tests for view models

### Integration Tests
- Database integration tests
- API endpoint integration tests
- Transaction workflow tests

### User Acceptance Tests
- Inventory management workflows
- Alert generation and handling
- Transaction recording and approval
- Reporting functionality

## Support and Maintenance

### Monitoring
- Stock level monitoring dashboards
- Transaction volume metrics
- System performance monitoring
- Error rate tracking

### Maintenance Tasks
- Regular database cleanup of old transactions
- Inventory reconciliation procedures
- Expired item cleanup workflows
- Performance optimization reviews

---

**Last Updated**: September 2025  
**Version**: 1.0  
**Author**: Claude Code Assistant