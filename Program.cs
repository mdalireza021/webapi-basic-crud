using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var products = new List<Product>
{ };

// Configure Swagger for development environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//get all product
app.MapGet("/api/products", () => products);


//get product by id
app.MapGet("/api/products/{id}", (Guid id) =>
{
    var product = products.Find(item => item.Id == id);
    return product is not null ? Results.Ok(product) : Results.NotFound("no product found!");
});


//create new product
app.MapPost("/api/products", (Product p) =>
{
    var product = products.Find(item => item.Id == p.Id);
    if (product is not null)
    {
        return Results.BadRequest("existing id is not allowed");
    }
    products.Add(p);
    return Results.Created($"/api/products/{p.Id}", p);
});

//update an existing product
app.MapPut("/products/{id:guid}", (Guid id, Product updatedProduct) =>
{


    var newProduct = products.Find(item => item.Id == id);

    if (newProduct is null)
    {
        return Results.BadRequest("no product found on specific id");
    }

    newProduct.Id = updatedProduct.Id;
    newProduct.Name = updatedProduct.Name;
    newProduct.Price = updatedProduct.Price;

    return Results.Ok(newProduct);
});


//delete product
app.MapDelete("api/products/{id}", (Guid id) =>
{
    var product = products.Find(item => item.Id == id);

    if (product is not null)
    {
        products.Remove(product);
        return Results.Ok($"Product with ID {id} deleted.");
    }

    return Results.NotFound($"Product with ID {id} not found.");
});


app.Run();
