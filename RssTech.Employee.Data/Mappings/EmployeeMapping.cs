using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RssTech.Employee.Infrastructure.Mappings;

public class EmployeeMapping : IEntityTypeConfiguration<Domain.Entities.Employee>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Employee> builder)
    {
        builder.ToTable("Employees");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.OwnsOne(e => e.Email, email =>
        {
            email.Property(e => e.Address)
                .IsRequired()
                .HasColumnName("Email")
                .HasMaxLength(100);
        });

        builder.Property(e => e.Password)
            .IsRequired()
            .HasMaxLength(255);

        builder.OwnsOne(e => e.Document, document =>
        {
            document.Property(d => d.DocumentNumber)
                .IsRequired()
                .HasColumnName("Document")
                .HasMaxLength(20);
        });

        builder.Property(e => e.DateOfBirth)
            .IsRequired();

        builder.Property(e => e.ManagerName)
            .IsRequired(false);

        builder.OwnsMany(e => e.Phones, phone =>
        {
            phone.ToTable("EmployeePhones");

            phone.WithOwner().HasForeignKey("EmployeeId");

            phone.HasKey("Id");

            phone.Property(p => p.Number)
                .IsRequired()
                .HasColumnName("PhoneNumber")
                .HasMaxLength(15);

            phone.HasIndex("EmployeeId", "Number").IsUnique();
        });

        builder.HasIndex(e => e.Email.Address).IsUnique();
        builder.HasIndex(e => e.Document.DocumentNumber).IsUnique();
    }
}
