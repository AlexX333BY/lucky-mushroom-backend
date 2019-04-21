using Microsoft.EntityFrameworkCore;

namespace LuckyMushroom.Models
{
    public class LuckyMushroomContext : DbContext
    {
        public LuckyMushroomContext()
        {
        }

        public LuckyMushroomContext(DbContextOptions<LuckyMushroomContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Article> Articles { get; set; }
        public virtual DbSet<ArticleGpsTag> ArticlesGpsTags { get; set; }
        public virtual DbSet<EdibleStatus> EdibleStatuses { get; set; }
        public virtual DbSet<GpsTag> GpsTags { get; set; }
        public virtual DbSet<RecognitionRequest> RecognitionRequests { get; set; }
        public virtual DbSet<RecognitionStatus> RecognitionStatuses { get; set; }
        public virtual DbSet<RequestPhoto> RequestPhotos { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<UserCredentials> UserCredentials { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity<Article>(entity =>
            {
                entity.HasKey(e => e.ArticleId);

                entity.ToTable("articles", "lucky_mushroom");

                entity.HasIndex(e => e.ArticleId)
                    .HasName("IX_article_id");

                entity.Property(e => e.ArticleId)
                    .HasColumnName("article_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.ArticleText)
                    .IsRequired()
                    .HasColumnName("article_text")
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ArticleGpsTag>(entity =>
            {
                entity.HasKey(e => new { e.TagId, e.ArticleId });

                entity.ToTable("articles_gps_tags", "lucky_mushroom");

                entity.HasIndex(e => e.TagId)
                    .HasName("IXFK_articles_gps_tags_gps_tags");

                entity.Property(e => e.TagId)
                    .HasColumnName("tag_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.ArticleId)
                    .HasColumnName("article_id")
                    .HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.Article)
                    .WithMany(p => p.ArticlesGpsTags)
                    .HasForeignKey(d => d.ArticleId)
                    .HasConstraintName("FK_articles_gps_tags_articles");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.ArticlesGpsTags)
                    .HasForeignKey(d => d.TagId)
                    .HasConstraintName("FK_articles_gps_tags_gps_tags");
            });

            modelBuilder.Entity<EdibleStatus>(entity =>
            {
                entity.HasKey(e => e.EdibleStatusId);

                entity.ToTable("edible_statuses", "lucky_mushroom");

                entity.HasIndex(e => e.EdibleStatusAlias)
                    .HasName("IX_edible_status_alias");

                entity.HasIndex(e => e.EdibleStatusId)
                    .HasName("IX_edible_status_id");

                entity.Property(e => e.EdibleStatusId)
                    .HasColumnName("edible_status_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.EdibleDescription)
                    .IsRequired()
                    .HasColumnName("edible_description")
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.EdibleStatusAlias)
                    .IsRequired()
                    .HasColumnName("edible_status_alias")
                    .HasColumnType("enum('edible','partial-edible','non-edible')");
            });

            modelBuilder.Entity<GpsTag>(entity =>
            {
                entity.HasKey(e => e.TagId);

                entity.ToTable("gps_tags", "lucky_mushroom");

                entity.HasIndex(e => e.Latitude)
                    .HasName("IX_latitude");

                entity.HasIndex(e => e.Longitude)
                    .HasName("IX_longitude");

                entity.HasIndex(e => e.TagId)
                    .HasName("IX_gps_tags");

                entity.Property(e => e.TagId)
                    .HasColumnName("tag_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.Latitude)
                    .HasColumnName("latitude")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Longitude)
                    .HasColumnName("longitude")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<RecognitionRequest>(entity =>
            {
                entity.HasKey(e => e.RequestId);

                entity.ToTable("recognition_requests", "lucky_mushroom");

                entity.HasIndex(e => e.EdibleStatusId)
                    .HasName("IXFK_recognition_requests_edible_statuses");

                entity.HasIndex(e => e.RequestDatetime)
                    .HasName("IX_request_datetime");

                entity.HasIndex(e => e.RequestId)
                    .HasName("IX_request_id");

                entity.HasIndex(e => e.RequesterId)
                    .HasName("IXFK_recognition_request_users");

                entity.HasIndex(e => e.StatusId)
                    .HasName("IXFK_recognition_requests_recognition_statuses");

                entity.Property(e => e.RequestId)
                    .HasColumnName("request_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.EdibleStatusId)
                    .HasColumnName("edible_status_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.RequestDatetime).HasColumnName("request_datetime");

                entity.Property(e => e.RequesterId)
                    .HasColumnName("requester_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.StatusId)
                    .HasColumnName("status_id")
                    .HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.EdibleStatus)
                    .WithMany(p => p.RecognitionRequests)
                    .HasForeignKey(d => d.EdibleStatusId)
                    .HasConstraintName("FK_recognition_requests_edible_statuses");

                entity.HasOne(d => d.Requester)
                    .WithMany(p => p.RecognitionRequests)
                    .HasForeignKey(d => d.RequesterId)
                    .HasConstraintName("FK_recognition_request_users");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.RecognitionRequests)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_recognition_requests_recognition_statuses");
            });

            modelBuilder.Entity<RecognitionStatus>(entity =>
            {
                entity.HasKey(e => e.StatusId);

                entity.ToTable("recognition_statuses", "lucky_mushroom");

                entity.HasIndex(e => e.StatusAlias)
                    .HasName("IX_status_alias");

                entity.HasIndex(e => e.StatusId)
                    .HasName("IX_status_id");

                entity.Property(e => e.StatusId)
                    .HasColumnName("status_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.StatusAlias)
                    .IsRequired()
                    .HasColumnName("status_alias")
                    .HasColumnType("enum('recognized','not-recognized')");

                entity.Property(e => e.StatusName)
                    .IsRequired()
                    .HasColumnName("status_name")
                    .HasMaxLength(16)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<RequestPhoto>(entity =>
            {
                entity.HasKey(e => e.PhotoId);

                entity.ToTable("request_photos", "lucky_mushroom");

                entity.HasIndex(e => e.RequestId)
                    .HasName("IXFK_request_photo_recognition_request");

                entity.Property(e => e.PhotoId)
                    .HasColumnName("photo_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.PhotoFilename)
                    .IsRequired()
                    .HasColumnName("photo_filename")
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.Property(e => e.RequestId)
                    .HasColumnName("request_id")
                    .HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.Request)
                    .WithMany(p => p.RequestPhotos)
                    .HasForeignKey(d => d.RequestId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_request_photo_recognition_request");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.RoleId);

                entity.ToTable("roles", "lucky_mushroom");

                entity.HasIndex(e => e.RoleAlias)
                    .HasName("IX_role_alias");

                entity.HasIndex(e => e.RoleId)
                    .HasName("IX_role_id");

                entity.Property(e => e.RoleId)
                    .HasColumnName("role_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.RoleAlias)
                    .IsRequired()
                    .HasColumnName("role_alias")
                    .HasColumnType("enum('user','admin')");

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasColumnName("role_name")
                    .HasMaxLength(16)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UserCredentials>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.ToTable("user_credentials", "lucky_mushroom");

                entity.HasIndex(e => e.UserId)
                    .HasName("IXFK_user_credentials_users");

                entity.HasIndex(e => e.UserMail)
                    .HasName("IX_user_mail");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(10) unsigned")
                    .ValueGeneratedNever();

                entity.Property(e => e.UserMail)
                    .IsRequired()
                    .HasColumnName("user_mail")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserPasswordHash)
                    .IsRequired()
                    .HasColumnName("user_password_hash")
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithOne(p => p.UserCredentials)
                    .HasForeignKey<UserCredentials>(d => d.UserId)
                    .HasConstraintName("FK_user_credentials_users");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.ToTable("users", "lucky_mushroom");

                entity.HasIndex(e => e.RoleId)
                    .HasName("IXFK_users_roles");

                entity.HasIndex(e => e.UserId)
                    .HasName("IX_user_id");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.RoleId)
                    .HasColumnName("role_id")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValueSql("0");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_users_roles");
            });
        }
    }
}
