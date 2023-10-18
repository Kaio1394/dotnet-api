using Azure.Identity;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration["Database:SqlServer"]);
var app = builder.Build();

List<Product> lista = new List<Product>();

app.MapPost("/products", (ProductRequest productRequest, ApplicationDbContext context) => { 
    var category = context.Categories.Where(c => c.Id == productRequest.CategoryId).First();
    var product = new Product
    {
        Code = productRequest.Code,
        Name = productRequest.Name,
        Description = productRequest.Description,
        Category = category,
    };

    context.Products.Add(product);
    context.SaveChanges();
    return Results.Created($"/products/{product.Id}", product.Id);
});

app.MapDelete("/products/{code}", ([FromRoute]string code) => {
    
    lista.RemoveAll(item => item.Code == code);
    return Results.Ok();
});
app.MapGet("/products/{code}", ([FromRoute]string code) => {
    var itemFound = lista.Find(item => item.Code == code);
    if(itemFound is not null)
        return Results.Ok();
    return Results.NotFound();
});
app.MapGet("/products", () => {
    return lista;
});
app.MapPut("/products", (Product prod) =>{
    bool found = false;
    foreach (var item in lista)
    {
        if(found == true)
            break;

        if(item.Code.ToLower().Equals(prod.Code.ToLower()))
        {
            item.Code = prod.Code;
            item.Name = prod.Name;
            found = true;
        }
    }
    if(found == false)
        lista.Add(prod);
        return Results.Ok(prod);
});

if(app.Environment.IsStaging())
{
    app.MapGet("/configuration/database", (IConfiguration config) => 
    {
        return Results.Ok(config["database:connection"]);
    });
}

app.Run();
