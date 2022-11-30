using System.Net;
using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Application.Entities;

namespace Infrastructure.Database;

public interface IProductRepository
{
    Task<bool> Insert(Product product);
    Task<Product?> Get(Guid id);
    Task<bool> Update(Product product);
    Task<bool> Delete(Guid id);
}
public class ProductRepository : IProductRepository
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private const string TableName = "products";
    public ProductRepository(IAmazonDynamoDB dynamoDb)
    {
        _dynamoDb = dynamoDb;
    }

    public async Task<bool> Insert( Product product)
    {
        var productAsJson = JsonSerializer.Serialize(product);
        var itemAsDocument = Document.FromJson(productAsJson);
        var itemAsAttribute = itemAsDocument.ToAttributeMap();

        var request = new PutItemRequest(TableName, itemAsAttribute);

        var response = await _dynamoDb.PutItemAsync(request);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<bool> Update(Product product)
    {
        var productAsJson = JsonSerializer.Serialize(product);
        var itemAsDocument = Document.FromJson(productAsJson);
        var itemAsAttribute = itemAsDocument.ToAttributeMap();

        var request = new PutItemRequest(TableName, itemAsAttribute);

        var response = await _dynamoDb.PutItemAsync(request);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<Product?> Get(Guid id)
    {
        var resquest = new GetItemRequest(TableName, 
            new Dictionary<string, AttributeValue>()
        {
            { "pk", new AttributeValue(id.ToString()) },
            { "sk", new AttributeValue(id.ToString()) }
        });

        var response = await _dynamoDb.GetItemAsync(resquest);

        if (response.Item.Count == 0)
        {
            return null;
        }

        var itemAsDocument = Document.FromAttributeMap(response.Item);

        var result = JsonSerializer.Deserialize<Product>(itemAsDocument.ToJson());
        return result;
    }

    public async Task<bool> Delete(Guid id)
    {
        var request = new DeleteItemRequest(TableName, 
            new Dictionary<string, AttributeValue>()
        {
            { "pk", new AttributeValue(id.ToString()) },
            { "sk", new AttributeValue(id.ToString()) }
        });

        var response = await _dynamoDb.DeleteItemAsync(request);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }
}