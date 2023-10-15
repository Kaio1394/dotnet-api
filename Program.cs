using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
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
        return Results.Created("/product" + prod.Code, prod.Code);   
    } 
    else
    {
        return Results.StatusCode(409);
    }  
});
app.MapDelete("/products/{code}", ([FromRoute]string code) => {
    lista.RemoveAll(item => item.Code == code);
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
});

app.Run();

public class Product
{
    public string Code{get; set;}
    public string Name {get; set;}
}