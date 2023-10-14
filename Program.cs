using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

List<Product> lista = new List<Product>();

app.MapPost("/save", (Product prod) => {
    lista.Add(prod);
    return prod.Code + " - " + prod.Name;
});
app.MapPost("/deleteElement/{index}", ([FromRoute]string index) => {
    lista.RemoveAt(Convert.ToInt32(index));
});
app.MapGet("/getLenList", () => {
    return lista.Count;
});
app.MapGet("/getList", () => {
    return lista;
});
app.MapGet("/getElement/{index}", ([FromRoute]string index) => {
    try
    {
        return lista[Convert.ToInt32(index)];
    }
    catch (System.Exception ex)
    {
        throw new Exception($"{ex.Message}");
    }
});

app.Run();

public class Product
{
    public string Code{get; set;}
    public string Name {get; set;}
}