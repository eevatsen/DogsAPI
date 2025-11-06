# Dogs API

Simple REST API for managing dogs. Built with ASP.NET Core and Entity Framework.

## What it does

- Check if API is working (ping endpoint)
- Get list of dogs with sorting and pagination
- Add new dogs with validation
- Limit too many requests (10 per second)

## How to run

1. Install .NET 8.0 SDK
2. Open `appsettings.json` and update connection string if needed
3. Run these commands:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
```

The API will start at `http://localhost:5000`

## API Endpoints

### 1. Check if working

```bash
curl http://localhost:5000/ping
```

Returns: `Dogshouseservice.Version1.0.1`

### 2. Get dogs

Basic:
```bash
curl http://localhost:5000/dogs
```

With sorting:
```bash
curl http://localhost:5000/dogs?attribute=weight&order=desc
```

With pagination:
```bash
curl http://localhost:5000/dogs?pageNumber=1&pageSize=5
```

All together:
```bash
curl http://localhost:5000/dogs?attribute=name&order=asc&pageNumber=1&pageSize=10
```

**Parameters:**
- `attribute` - sort by: name, color, tail_length, weight
- `order` - asc or desc (default: asc)
- `pageNumber` - page number (default: 1)
- `pageSize` - how many per page (default: 10)

**Response example:**
```json
[
  {
    "name": "Neo",
    "color": "red&amber",
    "tail_length": 22,
    "weight": 32
  }
]
```

### 3. Create dog

```bash
curl -X POST http://localhost:5000/dog \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Buddy",
    "color": "brown",
    "tail_length": 15,
    "weight": 25
  }'
```

**Rules:**
- Name must be unique
- tail_length must be 0 or positive
- weight must be positive
- All fields required

**Errors:**

If name already exists:
```
"Dog with this name already exists"
```

If tail_length is negative:
```
"Tail length must be zero or positive"
```

## Rate Limiting

API allows max 10 requests per second per IP address.

If you send too many requests, you get:
```
HTTP 429 Too Many Requests
Too many requests. Please try again later.
```

You can change this in `appsettings.json`:
```json
{
  "RateLimit": {
    "RequestsPerSecond": 10
  }
}
```


## Running Tests

```bash
dotnet test
```

All features are covered by tests.

## Database

The app uses Entity Framework with Code First approach.

Initial dogs in database:
- Neo (red&amber, tail: 22, weight: 32)
- Jessy (black&white, tail: 7, weight: 14)

## Configuration

Edit `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=DogsHouseDb;Trusted_Connection=True;"
  },
  "RateLimit": {
    "RequestsPerSecond": 10
  }
}
```

## Common Errors

**"Dog with this name already exists"** - Try different name

**"Tail length must be zero or positive"** - Use 0 or positive number

**"Too many requests"** - Wait 1 second and try again

**Database error** - Check connection string in appsettings.json
