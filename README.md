# PoC .NET ETCD

A proof of concept demonstrating how to integrate [etcd](https://etcd.io/) — a strongly consistent, distributed key-value store — with .NET (C#).

This PoC evaluates the feasibility of using etcd in .NET-based infrastructure and services, covering basic CRUD operations, change watching, and atomic transactions.

---

## Why etcd?

Unlike Redis, etcd is **strongly consistent** (via the Raft consensus algorithm), making it a better choice for storing critical distributed-system data such as configuration, service discovery state, and distributed locks.

---

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) with Compose support

---

## Running Locally

### 1. Start etcd via Docker Compose

```bash
docker compose up -d
```

This starts a single etcd node on port `2379` with no authentication and a persistent volume.

### 2. Run the .NET PoC

```bash
dotnet run --project Src/POCTemplate
```

### Expected output

```text
=== etcd PoC with .NET ===

--- Basic CRUD ---
PUT name = Guilherme
GET name = Guilherme
GET name (after update) = Updated
GET name (after delete) = "" (empty string means key not found)

--- Watch ---
  Watch event: dog -> "sits" (Put)
  Watch event: dog -> "runs" (Put)
  Watch event: dog -> "" (Delete)

--- Transaction (atomic multi-key write) ---
  Watch received 2 event(s) in one response:
    animals/cow -> "moo" (Put)
    animals/chicken -> "cluck" (Put)
GET animals/cow     = moo
GET animals/chicken = cluck

Done.
```

### 3. Stop etcd

```bash
docker compose down
```

To also remove the persistent volume:

```bash
docker compose down -v
```

---

## etcd CLI Quick Reference

If you have `etcdctl` installed (or via the container), you can interact with the store directly:

```bash
# Set a key
etcdctl put mykey "myvalue"

# Get a key
etcdctl get mykey

# Delete a key
etcdctl del mykey

# Watch a key for changes
etcdctl watch mykey

# List all keys with a prefix
etcdctl get animals/ --prefix

# Run via the Docker container
docker exec -it <container-name> etcdctl put foo bar
```

---

## Project Structure

```text
Src/
  POCTemplate/
    Program.cs          # etcd demo: CRUD, Watch, Transaction
    POCTemplate.csproj
Tests/
  POCTemplate.Tests/    # Placeholder test project
compose.yml             # Docker Compose for local etcd
```

---

## Key Concepts Demonstrated

| Feature | Description |
| --- | --- |
| **Basic CRUD** | Put, Get, Delete a key-value pair |
| **Watch** | Subscribe to changes on a key or key prefix |
| **Transaction** | Atomically write multiple keys in a single request |
| **Range watch** | Watch all keys under a "folder" prefix (e.g. `animals/`) |

---

## References

- [etcd documentation](https://etcd.io/docs/)
- [dotnet-etcd NuGet package](https://www.nuget.org/packages/dotnet-etcd)
- [Tutorial: etcd with .NET](https://medium.com/@vosarat1995/etcd-with-net-a65db4d5fe49)
