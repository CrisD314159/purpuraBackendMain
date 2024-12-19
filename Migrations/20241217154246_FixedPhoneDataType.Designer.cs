﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using purpuraMain.DbContext;

#nullable disable

namespace purpuraMain.Migrations
{
    [DbContext(typeof(PurpuraDbContext))]
    [Migration("20241217154246_FixedPhoneDataType")]
    partial class FixedPhoneDataType
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ArtistSong", b =>
                {
                    b.Property<string>("ArtistsId")
                        .HasColumnType("text");

                    b.Property<string>("SongsId")
                        .HasColumnType("text");

                    b.HasKey("ArtistsId", "SongsId");

                    b.HasIndex("SongsId");

                    b.ToTable("ArtistSong");
                });

            modelBuilder.Entity("GenreSong", b =>
                {
                    b.Property<string>("GenresId")
                        .HasColumnType("text");

                    b.Property<string>("SongsId")
                        .HasColumnType("text");

                    b.HasKey("GenresId", "SongsId");

                    b.HasIndex("SongsId");

                    b.ToTable("GenreSong");
                });

            modelBuilder.Entity("LibraryPlaylist", b =>
                {
                    b.Property<string>("LibrariesId")
                        .HasColumnType("text");

                    b.Property<string>("PlaylistsId")
                        .HasColumnType("text");

                    b.HasKey("LibrariesId", "PlaylistsId");

                    b.HasIndex("PlaylistsId");

                    b.ToTable("LibraryPlaylist");
                });

            modelBuilder.Entity("LibrarySong", b =>
                {
                    b.Property<string>("LibrariesId")
                        .HasColumnType("text");

                    b.Property<string>("SongsId")
                        .HasColumnType("text");

                    b.HasKey("LibrariesId", "SongsId");

                    b.HasIndex("SongsId");

                    b.ToTable("LibrarySong");
                });

            modelBuilder.Entity("PlaylistSong", b =>
                {
                    b.Property<string>("PlaylistsId")
                        .HasColumnType("text");

                    b.Property<string>("SongsId")
                        .HasColumnType("text");

                    b.HasKey("PlaylistsId", "SongsId");

                    b.HasIndex("SongsId");

                    b.ToTable("PlaylistSong");
                });

            modelBuilder.Entity("purpuraMain.Model.Album", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ArtistId")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PictureUrl")
                        .HasColumnType("text");

                    b.Property<string>("ProducerName")
                        .HasColumnType("text");

                    b.Property<string>("RecordLabel")
                        .HasColumnType("text");

                    b.Property<DateTime>("ReleaseDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("WriterName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ArtistId");

                    b.ToTable("Albums");
                });

            modelBuilder.Entity("purpuraMain.Model.Artist", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PictureUrl")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Artists");
                });

            modelBuilder.Entity("purpuraMain.Model.Enums.Countries", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Countries");
                });

            modelBuilder.Entity("purpuraMain.Model.Genre", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Genres");
                });

            modelBuilder.Entity("purpuraMain.Model.Library", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Libraries");
                });

            modelBuilder.Entity("purpuraMain.Model.PlayHistory", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<int>("PlayCount")
                        .HasColumnType("integer");

                    b.Property<DateTime>("PlayedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("SongId")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("SongId");

                    b.HasIndex("UserId");

                    b.ToTable("PlayHistories");
                });

            modelBuilder.Entity("purpuraMain.Model.Playlist", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<bool>("IsPublic")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Playlists");
                });

            modelBuilder.Entity("purpuraMain.Model.Song", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("AlbumId")
                        .HasColumnType("text");

                    b.Property<string>("AudioUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("Duration")
                        .HasColumnType("double precision");

                    b.Property<string>("Lyrics")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AlbumId");

                    b.ToTable("Songs");
                });

            modelBuilder.Entity("purpuraMain.Model.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<int>("CountryId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)");

                    b.Property<string>("ProfilePicture")
                        .HasColumnType("text");

                    b.Property<int>("State")
                        .HasColumnType("integer");

                    b.Property<int>("VerifyCode")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CountryId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ArtistSong", b =>
                {
                    b.HasOne("purpuraMain.Model.Artist", null)
                        .WithMany()
                        .HasForeignKey("ArtistsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("purpuraMain.Model.Song", null)
                        .WithMany()
                        .HasForeignKey("SongsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GenreSong", b =>
                {
                    b.HasOne("purpuraMain.Model.Genre", null)
                        .WithMany()
                        .HasForeignKey("GenresId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("purpuraMain.Model.Song", null)
                        .WithMany()
                        .HasForeignKey("SongsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("LibraryPlaylist", b =>
                {
                    b.HasOne("purpuraMain.Model.Library", null)
                        .WithMany()
                        .HasForeignKey("LibrariesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("purpuraMain.Model.Playlist", null)
                        .WithMany()
                        .HasForeignKey("PlaylistsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("LibrarySong", b =>
                {
                    b.HasOne("purpuraMain.Model.Library", null)
                        .WithMany()
                        .HasForeignKey("LibrariesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("purpuraMain.Model.Song", null)
                        .WithMany()
                        .HasForeignKey("SongsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PlaylistSong", b =>
                {
                    b.HasOne("purpuraMain.Model.Playlist", null)
                        .WithMany()
                        .HasForeignKey("PlaylistsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("purpuraMain.Model.Song", null)
                        .WithMany()
                        .HasForeignKey("SongsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("purpuraMain.Model.Album", b =>
                {
                    b.HasOne("purpuraMain.Model.Artist", "Artist")
                        .WithMany()
                        .HasForeignKey("ArtistId");

                    b.Navigation("Artist");
                });

            modelBuilder.Entity("purpuraMain.Model.Library", b =>
                {
                    b.HasOne("purpuraMain.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("purpuraMain.Model.PlayHistory", b =>
                {
                    b.HasOne("purpuraMain.Model.Song", "Song")
                        .WithMany()
                        .HasForeignKey("SongId");

                    b.HasOne("purpuraMain.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("Song");

                    b.Navigation("User");
                });

            modelBuilder.Entity("purpuraMain.Model.Playlist", b =>
                {
                    b.HasOne("purpuraMain.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("purpuraMain.Model.Song", b =>
                {
                    b.HasOne("purpuraMain.Model.Album", "Album")
                        .WithMany()
                        .HasForeignKey("AlbumId");

                    b.Navigation("Album");
                });

            modelBuilder.Entity("purpuraMain.Model.User", b =>
                {
                    b.HasOne("purpuraMain.Model.Enums.Countries", "Country")
                        .WithMany()
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Country");
                });
#pragma warning restore 612, 618
        }
    }
}
