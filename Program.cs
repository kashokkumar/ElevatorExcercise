using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddSignalR();
builder.Services.AddSingleton<SimulationState>();
builder.Services.AddHostedService<SimulationService>();

var app = builder.Build();

app.UseStaticFiles();
app.MapRazorPages();
app.MapHub<LiftHub>("/lifthub");

// Outside floor call (Up / Down)
app.MapPost("/call/{floor:int}/{direction}", (int floor, string direction, SimulationState state) =>
{
    var dir = direction.ToLower() == "up" ? Direction.Up : Direction.Down;
    state.DispatchFloorCall(floor, dir);
    return Results.Ok();
});

// Inside lift destination
app.MapPost("/dest/{liftId:int}/{floor:int}", (int liftId, int floor, SimulationState state) =>
{
    state.DispatchDestination(liftId, floor);
    return Results.Ok();
});

app.Run();


// ================= DOMAIN =================

enum Direction { Up, Down, Idle }
enum LiftStatus { Moving, Stopped, Idle }

class Lift
{
    public int Id { get; }
    public int Floor { get; private set; }
    public Direction Direction { get; private set; } = Direction.Idle;
    public LiftStatus Status { get; private set; } = LiftStatus.Idle;

    private readonly SortedSet<int> up = new();
    private readonly SortedSet<int> down =
        new(Comparer<int>.Create((a, b) => b.CompareTo(a)));

    public Lift(int id, int startFloor)
    {
        Id = id;
        Floor = startFloor;
    }

    public void AddDestination(int floor)
    {
        if (floor == Floor)
        {
            Status = LiftStatus.Stopped;
            return;
        }

        if (floor > Floor)
            up.Add(floor);
        else
            down.Add(floor);

        if (Direction == Direction.Idle)
            Direction = floor > Floor ? Direction.Up : Direction.Down;
    }

    public void Tick()
    {
        if (Direction == Direction.Up && up.Any())
        {
            Status = LiftStatus.Moving;
            Floor++;

            if (up.Remove(Floor))
                Status = LiftStatus.Stopped;
        }
        else if (Direction == Direction.Down && down.Any())
        {
            Status = LiftStatus.Moving;
            Floor--;

            if (down.Remove(Floor))
                Status = LiftStatus.Stopped;
        }
        else
        {
            Direction = Direction.Idle;
            Status = LiftStatus.Idle;
        }
    }
}

class SimulationState
{
    public List<Lift> Lifts { get; } =
    [
        new Lift(1, 1),
        new Lift(2, 4),
        new Lift(3, 7),
        new Lift(4, 10)
    ];

    // Outside floor call → naive dispatch
    public void DispatchFloorCall(int floor, Direction direction)
    {
        var lift = Lifts
            .OrderBy(l => Math.Abs(l.Floor - floor))
            .First();

        lift.AddDestination(floor);
    }

    // Inside lift destination → direct assignment
    public void DispatchDestination(int liftId, int floor)
    {
        var lift = Lifts.FirstOrDefault(l => l.Id == liftId);
        lift?.AddDestination(floor);
    }

    public void Tick()
    {
        foreach (var lift in Lifts)
            lift.Tick();
    }
}

class LiftHub : Hub { }


// ================= BACKGROUND LOOP =================

class SimulationService : BackgroundService
{
    private readonly SimulationState state;
    private readonly IHubContext<LiftHub> hub;

    public SimulationService(SimulationState state, IHubContext<LiftHub> hub)
    {
        this.state = state;
        this.hub = hub;
    }

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            state.Tick();

            await hub.Clients.All.SendAsync(
                "update",
                state.Lifts.Select(l => new
                {
                    l.Id,
                    l.Floor,
                    Direction = l.Direction.ToString(),
                    Status = l.Status.ToString()
                }),
                token);

            await Task.Delay(1000, token); // fast demo speed
        }
    }
}
