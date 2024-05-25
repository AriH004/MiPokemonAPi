
using System.Text.Json;

namespace MiPokemonFavorito
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
           
            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapGet("/Pokemon", async () =>
            {
                HttpClient client = new HttpClient();
                var data = await client.GetAsync("https://pokeapi.co/api/v2/pokemon/spiritomb");
                string datos = await data.Content.ReadAsStringAsync();

                var pokemonData = JsonDocument.Parse(datos).RootElement;

                var poke = new
                {
                    Name = pokemonData.GetProperty("name").GetString(),
                    Tipo = pokemonData.GetProperty("types")[0].GetProperty("type").GetProperty("name").GetString(),
                    UrlSprite = pokemonData.GetProperty("sprites").GetProperty("front_default").GetString(),
                    Moves = pokemonData.GetProperty("moves").EnumerateArray().Select(move => move.GetProperty("move").GetProperty("name").GetString()).ToList()
                };

                return Results.Json(poke);
            });
            

            app.Run();
        }
    }
}
