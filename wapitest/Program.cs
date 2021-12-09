using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<Db>(opt => opt.UseSqlServer(@"Server=localhost;Database=ggwplarin_db;Trusted_Connection=True;"));

// Add services to the container.
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

app.MapGet("/cringes", async (Db db) =>
    await db.Cringes.ToListAsync());

app.MapGet("/cringes/{id}", async (int id, Db db) =>
     await db.Cringes.FindAsync(id) is Cringe cringe ? Results.Ok(cringe) : Results.NotFound());

app.MapPost("/cringes", async (Cringe cringe, Db db) =>
{
    db.Cringes.Add(cringe);
    await db.SaveChangesAsync();
    return Results.Created($"/cringes/{cringe.id}", cringe);
});

app.MapPut("/cringes/{id}", async (int id, Cringe inputCringe, Db db) =>
{
    var cringe = await db.Cringes.FindAsync(id);
    if (cringe is null) return Results.NotFound();

    cringe.name = inputCringe.name;
    cringe.lvl = inputCringe.lvl;
    cringe.gg = inputCringe.gg;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/cringes/{id}", async (int id, Db db) =>
{
    if (await db.Cringes.FindAsync(id) is Cringe cringe)
    {
        db.Cringes.Remove(cringe);
        await db.SaveChangesAsync();
        return Results.Ok(cringe);
    }
    return Results.NotFound();
});


using (var scope = app.Services.CreateScope())
using (var context = scope.ServiceProvider.GetService<Db>())
{
    context.Database.EnsureCreated();
    context.Database.Migrate();
}


app.Run();


[Table("Cringes")]
class Cringe
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int id { get; set; }
    public string? name { get; set; }
    public int? lvl { get; set; }
    public int gg { get; set; }

}

class Db : DbContext
{
    public Db(DbContextOptions<Db> options) : base(options)
    {

    }
    public DbSet<Cringe> Cringes => Set<Cringe>();
}

