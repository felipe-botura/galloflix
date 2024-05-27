using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GalloFlix.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace GalloFlix.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<MovieGenre> MovieGenres { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        #region Configuração do Muitos para Muitos do MovieGenre
        builder.Entity<MovieGenre>()
            .HasKey(mg => new { mg.MovieId, mg.GenreId });

        builder.Entity<MovieGenre>()
            .HasOne(mg => mg.Movie)
            .WithMany(m => m.Genres)
            .HasForeignKey(mg => mg.MovieId);

        builder.Entity<MovieGenre>()
            .HasOne(mg => mg.Genre)
            .WithMany(g => g.Movies)
            .HasForeignKey(mg => mg.GenreId);
        #endregion


        #region Popular os dados do usuário
        List<IdentityRole> roles = new()
    {
        new IdentityRole()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Administrador",
            NormalizedName = "ADMINISTRADOR"
        },
        new IdentityRole()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Moderador",
            NormalizedName = "MODERADOR"
        },
        new IdentityRole()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Usuário",
            NormalizedName = "USUÁRIO"
        }
    };
        builder.Entity<IdentityRole>().HasData(roles);

        List<IdentityUser> users = new()
        {
            new IdentityUser()
            {
                Id = Guid.NewGuid().ToString(),
                Email = "admin@gallo.com",
                NormalizedEmail = "ADMIN@GALLO.COM",
                UserName = "Admin",
                NormalizedUserName = "ADMIN",
                EmailConfirmed = true,
                LockoutEnabled = false
            },
            new IdentityUser()
            {
                Id = Guid.NewGuid().ToString(),
                Email = "giovana@gmail.com",
                NormalizedEmail = "GIOVANA@GMAIL.COM",
                UserName = "Giovana",
                NormalizedUserName = "GIOVANA",
                EmailConfirmed = true,
                LockoutEnabled = true
            }
        };
        foreach (var user in users)
        {
            PasswordHasher<IdentityUser> pass = new();
            pass.HashPassword(user, "@Etec123");
        }
        builder.Entity<IdentityUser>().HasData(users);

        // Dados pessoais do usuário - AppUser
        List<AppUser> appUsers = new()
        {
            new AppUser()
            {
            AppUserId = users[0].Id,
            Name = "Botura & Giovana",
            Birthday = DateTime.Parse("01/01/2000"),
            Photo = "/img/users/avatar.png"
            },
            new AppUser()
            {
            AppUserId = users[1].Id,
            Name = "Fulano",
            Birthday = DateTime.Parse("05/03/1997"),
            }
        };
        builder.Entity<AppUser>().HasData(appUsers);

        // Perfis dos Usuários - IndentityUSerRole
        List<IdentityUserRole<string>> userRoles = new() 
        {
            new IdentityUserRole<string>()
            {
                UserId = users[0].Id,
                RoleId = roles[0].Id
            },
            new IdentityUserRole<string>()
            {
                UserId = users[0].Id,
                RoleId = roles[2].Id
            },
            new IdentityUserRole<string>()
            {
                UserId = users[1].Id,
                RoleId = roles[2].Id
            }
        };
        builder.Entity<IdentityUserRole<string>>().HasData(userRoles);
        #endregion
    }
}