using LittleberryApi.Models;
using LittleberryApi.Models.StoredProcResults;
using Microsoft.EntityFrameworkCore;

namespace LittleberryApi.Data;

public class PfsaDbContext : DbContext
{
    public PfsaDbContext(DbContextOptions<PfsaDbContext> options) : base(options)
    {
    }

    // Entity Sets
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Catalog> Catalogs { get; set; }
    public DbSet<Council> Councils { get; set; }
    public DbSet<History> Histories { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<ReservationSub> ReservationSubs { get; set; }
    public DbSet<Request> Requests { get; set; }
    public DbSet<Shipment> Shipments { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<VwReservation> VwReservations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Catalog table
        modelBuilder.Entity<Catalog>().ToTable("Catalog").HasKey(c => c.BookId);
        modelBuilder.Entity<Request>().ToView("vwRequest").HasKey(r => r.ReservationSubId);
        modelBuilder.Entity<VwReservation>().ToView("vwReservation").HasKey(v => v.ReservationSubId);

        // Configure keyless entities for stored procedure results
        modelBuilder.Entity<SpBookResult>().HasNoKey().ToView(null);
        modelBuilder.Entity<SpBookHistoryResult>().HasNoKey().ToView(null);
        modelBuilder.Entity<SpAuthorListResult>().HasNoKey().ToView(null);
    }

    // Stored Procedure Methods

    public async Task<Account?> GetUserBySessionIdAsync(string sessionId)
    {
        var result = await Accounts
            .FromSqlInterpolated($"EXEC sp_GetUserBySessionId {sessionId}")
            .AsNoTracking()
            .ToListAsync();
        return result.FirstOrDefault();
    }

    public async Task<List<Catalog>> CatalogSearchAsync(string prefix, string author, string title, string status)
    {
        return await Catalogs
            .FromSqlInterpolated($"EXEC sp_CatalogSearch {prefix}, {author}, {title}, {status}")
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<SpBookResult>> GetBookAsync(string prefix, int bookNumber)
    {
        return await Set<SpBookResult>()
            .FromSqlInterpolated($"EXEC sp_Book {prefix}, {bookNumber}")
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<SpBookHistoryResult>> GetBookHistoryAsync(string prefix, int bookNumber)
    {
        return await Set<SpBookHistoryResult>()
            .FromSqlInterpolated($"EXEC sp_BookHistory {prefix}, {bookNumber}")
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<SpAuthorListResult>> GetAuthorListAsync(int bookCount)
    {
        return await Set<SpAuthorListResult>()
            .FromSqlInterpolated($"EXEC sp_AuthorList {bookCount}")
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Request>> AddRequestAsync(int? bookId, string requestByEmail, DateTime? requestDate, string sessionId)
    {
        return await Requests
            .FromSqlInterpolated($"EXEC sp_AddRequest {bookId}, {requestByEmail}, {requestDate}, {sessionId}")
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Request>> UpdateRequestAsync(int? reservationSubId, string type, bool? onOff, DateTime? statusDate, string sessionId)
    {
        return await Requests
            .FromSqlInterpolated($"EXEC sp_UpdateRequest {reservationSubId}, {type}, {onOff}, {statusDate}, {sessionId}")
            .AsNoTracking()
            .ToListAsync();
    }
}
