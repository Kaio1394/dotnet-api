using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>();
var app = builder.Build();

List<Product> lista = new List<Product>();

app.MapPost("/products", ([FromBody]Product prod) => { 
    bool equal = false;   
    foreach (var item in lista)
    {
        if(item.Code == prod.Code && item.Name == item.Name)
            equal = true;
    }
    if(equal == false || lista.Count == 0)
    {
        lista.Add(prod);
        return Results.Created("/product" + prod.Code, prod);   
    } 
    else
    {
        return Results.StatusCode(409);
    }  
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

public class Product
{
    public int Id { get; set; }
    public string Code{get; set;}
    public string Name {get; set;}
    public string Description { get; set; }
}

public class ApplicationDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)   
        => optionsBuilder.UseSqlServer("Server=localhost;Database=Products;Trusted_Connection=True;Encrypt=NO;");
}